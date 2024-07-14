using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

public class RollResultSet // Was a struct, but pass-by-value clashed with List properties
{
    public static int TotalNumRolls = 0;

    public RollResultSet(List<int> original)
    {
        Original = original;
        Rerolled = null;
        TotalNumRolls++;
        RollId = TotalNumRolls;
    }

    public List<int> Original { get; init; }
    public List<int> Rerolled { get; set; }
    public int RollId { get; init; }
}

public static partial class Utils
{
    public static int GetSingleDieResult(int min, int max, RandomNumberGenerator rng = null)
    {
        if (rng is null)
        {
            rng = new RandomNumberGenerator();
            rng.Randomize();
        }
        return (int)(rng.Randi() % max + min);
    }

    public static RandomNumberGenerator SetRng(RandomNumberGenerator rng, bool randomize = true)
    {
        if (rng is null || rng is default(RandomNumberGenerator))
        {
            rng = new RandomNumberGenerator();
            randomize = true;
        }
        if (randomize)
        {
            rng.Randomize();
        }
        return rng;
    }

    public static RollResultSet RollBones(int numDice, int dSize, RandomNumberGenerator rng = null)
    {
        if (rng is null)
        {
            rng = new RandomNumberGenerator();
            rng.Randomize();
        }

        int min = V5Roll.DIE_MIN;
        List<int> result = new List<int>();
        for (int i = 0; i < numDice; i++)
        {
            int dieResult = GetSingleDieResult(min, dSize, rng);
            // was: (int)(rng.Randi() % numSides + min);
            result.Add(dieResult);
        }
        return new RollResultSet(result);
    }
}

public class V5Roll
{
    public const int DIE_MIN = 1;
    public const int DIE_MAX = 10;
    public const int ANKH_THRESHOLD = 6;
    public const int CRIT_GROUPING = 2;
    public const int WILL_REROLL_MAX_DICE = 3;

    private int _blackPool, _redPool;
    private string _separator;
    private bool _rerolledAlready, _will2Win, _will2Restrain;
    private RandomNumberGenerator _rng;

    public V5Entity Actor { get; init; }
    public bool UsingHunger { get; init; }
    public int? HungerOverride { get; init; }
    public int RollHunger { get; private set; }

    public RollResultSet BlackDiceResults { get; private set; }
    public RollResultSet RedDiceResults { get; private set; }
    public int Ankhs { get; private set; }
    public bool CanReroll
    {
        get
        {
            if (_rerolledAlready) { return false; }
            // TODO: Check Actor's willpower here!
            return true;
        }
    }
    public int BlackTens { get; private set; }
    public int RedTens { get; private set; }
    public int RedOnes { get; private set; }

    public bool PossibleWillRerollGain
    {
        get => BlackDiceResults is not default(RollResultSet) &&
            BlackDiceResults.Original.Count > 0 &&
            BlackDiceResults.Original.Count > BlackTens;
    }
    public bool PossibleMessyCrit { get => RedTens > 0 && RedTens + BlackTens > 1; }
    public bool AvoidableMessyCrit { get => PossibleMessyCrit && BlackTens > 0; }

    public V5Contest.VERDICT Verdict { get; set; }

    public bool formatToString = false;

    public V5Roll(
        V5Entity actor, int pool,
        bool useHunger = false, int? hungerOverride = null,
        bool will2Win = false, bool will2Restrain = false,
        RandomNumberGenerator rng = null,
        string separatorStr = ", "
    )
    {
        Actor = actor;
        UsingHunger = useHunger; HungerOverride = hungerOverride;
        Ankhs = 0;
        _blackPool = pool; _redPool = 0;
        _will2Win = will2Win; _will2Restrain = will2Restrain;
        BlackDiceResults = null; RedDiceResults = null;
        _rng = rng is null ? Utils.SetRng(new RandomNumberGenerator()) : rng;
        _separator = separatorStr;
        Roll();
    }

    // protected int GetSingleDieRoll()
    // {
    //     return (int)_rng.Randi() % (V5Roll.DIE_MAX + 1) + V5Roll.DIE_MIN;
    // }

    protected void PreRollSetup()
    {
        if (Actor is V5Entity)
        {
            Actor.numDiceTests++;
        }
    }

    public void Roll()
    {
        PreRollSetup();
        RollHunger = 0; int actorHunger;
        // UsingHunger trumps HungerOverride, which in turn trumps Actor.Hunger, which defaults to 0
        if (UsingHunger)
        {
            try { actorHunger = Actor.Hunger; }
            catch (NullReferenceException) { actorHunger = 0; }
            RollHunger = HungerOverride is null ? actorHunger : (int)HungerOverride;
            _redPool = Math.Min(_blackPool, RollHunger);
            RedDiceResults = Utils.RollBones(_redPool, V5Roll.DIE_MAX, rng: _rng);
        }
        _blackPool = Math.Max(_blackPool - RollHunger, 0);
        BlackDiceResults = Utils.RollBones(_blackPool, V5Roll.DIE_MAX, rng: _rng);
        EvaluateRoll();
    }

    public void EvaluateRoll()
    {
        List<int> blackResultsList = _rerolledAlready ?
            BlackDiceResults.Rerolled : BlackDiceResults.Original;
        Ankhs = 0; BlackTens = 0; RedTens = 0; RedOnes = 0;
        for (int i = 0; i < _redPool; i++)
        {
            EvaluateDieResult(RedDiceResults.Original[i], true);
        }
        for (int i = 0; i < _blackPool; i++)
        {
            EvaluateDieResult(blackResultsList[i], false);
        }

        int anyColorTens = BlackTens + RedTens;
        Ankhs += (int)Math.Floor((decimal)anyColorTens / 2) * 2;
        bool isOverkill = RedTens > 0 && RedTens + BlackTens >= V5Roll.CRIT_GROUPING;
        if (CanReroll)
        {
            if (_will2Win)
            {
                Reroll(will2Win: true, false);
            }
            else if (isOverkill && _will2Restrain)
            {
                Reroll(false, will2Restrain: true);
            }
        }
        GD.Print($"{(_rerolledAlready ? "re-" : "")}evaluate: ({GetHashCode()}): {Ankhs} ankhs!");
    }

    private void EvaluateDieResult(int dieResult, bool isRed)
    {
        if (dieResult >= V5Roll.DIE_MAX)
        {
            if (isRed)
            {
                RedTens++;
            }
            else
            {
                BlackTens++;
            }
        }
        else if (isRed && dieResult == V5Roll.DIE_MIN)
        {
            RedOnes++;
        }
        else if (dieResult < V5Roll.DIE_MIN)
        {
            throw new ArgumentException(
                $"{GetType()}.EvaluateDieResult(): '{dieResult}' below minimum ({V5Roll.DIE_MIN})!"
            );
        }

        if (dieResult >= V5Roll.ANKH_THRESHOLD)
        {
            Ankhs++;
        }
    }

    public void Reroll(bool will2Win, bool will2Restrain)
    {
        if (will2Win && will2Restrain)
        {
            throw new ArgumentException(
                "Can't use willpower to both enhance and restrain the same roll."
            );
        }
        else if (!will2Win && !will2Restrain)
        {
            throw new ArgumentException(
                "A willpower reroll must either enhance or restrain the roll."
            );
        }

        int diceRerolled = 0;
        BlackDiceResults.Rerolled = new List<int>(BlackDiceResults.Original);
        for (int i = 0; i < BlackDiceResults.Original.Count; i++)
        {
            int temp = BlackDiceResults.Rerolled[i];
            // will2Win currently only tries to reroll black dice below success threshold (6)
            if (will2Win && BlackDiceResults.Rerolled[i] < V5Roll.ANKH_THRESHOLD)
            {
                BlackDiceResults.Rerolled[i] =
                    Utils.GetSingleDieResult(V5Roll.DIE_MIN, V5Roll.DIE_MAX, _rng);
                diceRerolled++;
                GD.Print($"reroll+ #{diceRerolled}: {temp} -> {BlackDiceResults.Rerolled[i]}");
            }
            else if (will2Restrain && BlackDiceResults.Rerolled[i] >= V5Roll.DIE_MAX)
            {
                BlackDiceResults.Rerolled[i] =
                    Utils.GetSingleDieResult(V5Roll.DIE_MIN, V5Roll.DIE_MAX, _rng);
                diceRerolled++;
                GD.Print($"reroll-MC #{diceRerolled}: {temp} -> {BlackDiceResults.Rerolled[i]}");

            }
            if (diceRerolled >= V5Roll.WILL_REROLL_MAX_DICE)
            {
                break;
            }
        }
        _rerolledAlready = true;
        EvaluateRoll();
    }

    private string DieResToStrV1(int res, bool isRed = false)
    {
        return isRed ? $"<r {res} >" : $"< {res} >";
    }

    public string ToUnformattedString()
    {
        string rollStr = "", sep = _separator;
        List<int> blackNums = _rerolledAlready ?
            BlackDiceResults.Rerolled : BlackDiceResults.Original;
        List<int> redNums = RedDiceResults is null ? new List<int>(0) : RedDiceResults.Original;
        string redMarks =
            String.Join(sep, redNums.ConvertAll<string>((res) => DieResToStrV1(res, true)));
        string blackMarks =
            String.Join(sep, blackNums.ConvertAll<string>((res) => DieResToStrV1(res)));
        if (redMarks.Length > 0)
        {
            rollStr = $"{redMarks}" + (blackMarks.Length > 0 ? sep : "");
        }
        return $"{rollStr}{blackMarks}";
    }

    public string UnfStr()
    {
        return ToUnformattedString();
    }

    public string ToBBCodeString()
    {
        return V5Roll.BBCodeFormatRollResults(this.ToUnformattedString(), true);
    }

    public override string ToString()
    {
        return formatToString ? ToBBCodeString() : ToUnformattedString();
    }

    public static string BBCodeFormatRollResults(
        string unfRollStr,
        bool injectNewline = false,
        string sep = ", ",
        int everyN = 7
    )
    {
        Regex rgx = new("<r[ ]*(.+?)[ ]*>");
        string formattedV5RollStr = rgx.Replace(unfRollStr, "[color=#f00]< $1 >[/color]");
        if (injectNewline)
        {
            return Utils.InjectSepEveryNthMark(formattedV5RollStr, sep, everyN, "\n");
        }
        return formattedV5RollStr;
    }
}

public class V5Contest
{
    public enum VERDICT
    {
        NONE,
        MESSYCRIT,
        CRIT,
        WIN,
        FAIL,
        BEASTFAIL
    }

    public V5Entity Actor { get; init; }
    private int _actorNumDice;
    public bool UseActorHunger { get; private set; } = true;
    public bool ActorWill2Win { get; private set; } = false;
    public bool ActorWill2Restrain { get; private set; } = false;
    private int? _actorHungerOverride;
    private V5PoolParser.ResolvedPool _actorPool;

    public V5Entity Reactor { get; init; }
    private int _reactorNumDice;
    public bool UseReactorHunger { get; private set; } = true;
    public bool ReactorWill2Win { get; private set; } = false;
    public bool ReactorWill2Restrain { get; private set; } = false;
    private int? _reactorHungerOverride;
    private V5PoolParser.ResolvedPool _reactorPool;

    private RandomNumberGenerator _rng;
    private int _timesChecked = 0;

    public bool IsContest { get; init; }
    public bool Enacted { get; private set; }
    public int Margin { get; private set; }
    public V5Roll ActorRoll { get; private set; }
    public V5PoolParser.ResolvedPool ActorPool
    {
        get => _actorPool;
        set { CheckPoolAlignment(value, _actorNumDice); _actorPool = value; }
    }
    public VERDICT ActorVerdict { get; private set; }
    public int FlatDifficulty { get; init; }
    public V5Roll ReactorRoll { get; private set; }
    public VERDICT ReactorVerdict { get; private set; }
    public V5PoolParser.ResolvedPool ReactorPool
    {
        get => _reactorPool;
        set { CheckPoolAlignment(value, _reactorNumDice); _reactorPool = value; }
    }

    public string NarrativeContext { get; init; }
    public Godot.Collections.Array<Variant> SpecialRules { get; init; }

    public V5Contest(
        int actorPool, V5Entity actor, int reactorPool, V5Entity reactor = null,
        int? actorHungerOverride = null, int? reactorHungerOverride = null,
        RandomNumberGenerator rng = null,
        string narrativeContext = "", Godot.Collections.Array<Variant> specialRules = null
    )
    {
        _actorNumDice = actorPool; Actor = actor; /*_useActorHunger = useActorHunger;*/
        _reactorNumDice = reactorPool; Reactor = reactor;
        IsContest = Reactor is not default(V5Entity);
        // _actorWill2Win = actorWill2Win; _actorWill2Restrain = actorWill2Restrain;
        _actorHungerOverride = actorHungerOverride;
        if (IsContest)
        {
            // _useReactorHunger = useReactorHunger;
            // _reactorWill2Win = reactorWill2Win; _reactorWill2Restrain = reactorWill2Restrain;
            _reactorHungerOverride = reactorHungerOverride;
        }
        else
        {
            // _useReactorHunger = false;
            // _reactorWill2Win = false; _reactorWill2Restrain = false;
            _reactorHungerOverride = null;
            FlatDifficulty = _reactorNumDice;
            ReactorRoll = null; ReactorVerdict = VERDICT.NONE;
        }

        _rng = rng is null ? Utils.SetRng(new RandomNumberGenerator(), true) : rng;
        NarrativeContext = narrativeContext;
        SpecialRules = specialRules;
    }

    protected static void CheckPoolAlignment(V5PoolParser.ResolvedPool pool, int diceTotal)
    {
        if (pool.Total != diceTotal)
        {
            throw new ArgumentException(
                $"ResolvedPool.Total ({pool.Total}) != _actorNumDice ({diceTotal})!"
            );
        }
    }

    public V5Contest Enact()
    {
        ActorRoll = new V5Roll(
            Actor, _actorNumDice, UseActorHunger, _actorHungerOverride,
            ActorWill2Win, ActorWill2Restrain, _rng
        )
        { formatToString = true };
        if (IsContest)
        {
            ReactorRoll = new V5Roll(
                Reactor, _reactorNumDice, UseReactorHunger, _reactorHungerOverride,
                ReactorWill2Win, ReactorWill2Restrain, _rng
            )
            { formatToString = true };
        }
        Enacted = true;
        return GetOutcome();
    }

    public V5Contest GetOutcome()
    {
        if (!Enacted)
        {
            throw new InvalidOperationException("Contest not enacted; can't get outcome.");
        }
        _timesChecked++;
        int threshold = IsContest ? ReactorRoll.Ankhs : FlatDifficulty;
        Margin = ActorRoll.Ankhs - threshold;
        V5Roll winningRoll, losingRoll;
        if (Margin >= 0) // Tie goes to active party.
        {
            winningRoll = ActorRoll;
            losingRoll = IsContest ? ReactorRoll : null;
        }
        else
        {
            losingRoll = ActorRoll;
            winningRoll = IsContest ? ReactorRoll : null;
        }

        int wRedTens = winningRoll is not null ? winningRoll.RedTens : 0;
        int wBlackTens = winningRoll is not null ? winningRoll.BlackTens : 0;
        int lRedOnes = losingRoll is not null ? losingRoll.RedOnes : 0;

        VERDICT winnersResult = VERDICT.WIN, losersResult = VERDICT.FAIL;
        if (wRedTens > 0 && wRedTens + wBlackTens >= V5Roll.CRIT_GROUPING)
        {
            winnersResult = VERDICT.MESSYCRIT;
        }
        else if (wBlackTens >= V5Roll.CRIT_GROUPING)
        {
            winnersResult = VERDICT.CRIT;
        }

        if (lRedOnes > 0)
        {
            losersResult = VERDICT.BEASTFAIL;
        }

        if (winningRoll is not null)
        {
            winningRoll.Verdict = winnersResult;
            if (Margin >= 0)
            {
                ActorVerdict = winnersResult;
            }
            else
            {
                ReactorVerdict = winnersResult;
            }
        }

        if (losingRoll is not null)
        {
            losingRoll.Verdict = losersResult;
            if (Margin < 0)
            {
                ActorVerdict = losersResult;
            }
            else
            {
                ReactorVerdict = losersResult;
            }
        }

        return this;
    }

    public static V5Contest Test(
        V5PoolParser.ResolvedPool actorPool, V5Entity actor, int testDifficulty,
        bool useActorHunger = true, int? actorHungerOverride = null,
        bool actorWill2Win = false, bool actorWill2Restrain = false,
        RandomNumberGenerator rng = null
    )
    {
        V5Contest v5Test = new(
            actorPool.Total, actor, testDifficulty, null, actorHungerOverride, null, rng
        )
        {
            ActorPool = actorPool,
            UseActorHunger = useActorHunger,
            ActorWill2Win = actorWill2Win,
            ActorWill2Restrain = actorWill2Restrain
        };
        return v5Test.Enact();
    }

    public static V5Contest Contest(
        V5PoolParser.ResolvedPool actorPool, V5Entity actor, bool useActorHunger,
        V5PoolParser.ResolvedPool reactorPool, V5Entity reactor, bool useReactorHunger = false,
        int? actorHungerOverride = null,
        bool actorWill2Win = false, bool actorWill2Restrain = false,
        int? reactorHungerOverride = null,
        bool reactorWill2Win = false, bool reactorWill2Restrain = false,
        RandomNumberGenerator rng = null
    )
    {
        V5Contest v5Contest = new(
            actorPool.Total, actor, reactorPool.Total, reactor,
            actorHungerOverride, reactorHungerOverride, rng
        )
        {
            ActorPool = actorPool,
            ReactorPool = reactorPool,
            UseActorHunger = useActorHunger,
            UseReactorHunger = useReactorHunger,
            ActorWill2Win = actorWill2Win,
            ActorWill2Restrain = actorWill2Restrain,
            ReactorWill2Win = reactorWill2Win,
            ReactorWill2Restrain = reactorWill2Restrain
        };
        return v5Contest.Enact();
    }

    public override string ToString()
    {
        if (!Enacted)
        {
            return $"This {(IsContest ? "Contest" : "Test")} has not been run yet.";
        }

        string repStr = $"{Actor} ";
        repStr += ActorRoll.UsingHunger ? $"({ActorRoll.RollHunger} Hngr) " : "(0 Hngr) ";
        string aSuccStr = $"success{(ActorRoll.Ankhs == 1 ? "" : "es")}";
        repStr += $"  ::  ({ActorRoll.Ankhs} {aSuccStr} -> {ActorRoll.Verdict})";
        repStr += $"\n {ActorRoll}\n";

        if (IsContest)
        {
            repStr += $"vs {(Reactor is not null ? Reactor : "Opponent")}";
            repStr += ReactorRoll.UsingHunger ? $"({ReactorRoll.RollHunger} Hngr) " : "(0 Hngr) ";
            string rSuccStr = $"success{(ReactorRoll.Ankhs == 1 ? "" : "es")}";
            repStr += $"  ::  ({ReactorRoll.Ankhs} {rSuccStr} -> {ReactorRoll.Verdict})";
            repStr += $"\n {ReactorRoll}\n";
        }
        else
        {
            repStr += $"vs Difficulty {FlatDifficulty}\n";
        }

        repStr += $"(Margin was {Margin})";
        return repStr;
    }
}

public static partial class V5PoolParser
{
    public enum TokenParseMethod
    {
        INIT_INT_PARSE,
        STAT_LOOKUP,
        FAILED_LOOKUP_INVALID_STAT,
        FAILED_LOOKUP_STAT_NOT_FOUND_IN_ACTOR,
        FAILED_PARSE_UNKNOWN_CAUSE
    }

    public static readonly List<string> SpecialTokens = new() {
        "handle special",
        "cases here",
        "won't you?"
    };

    public readonly struct ResolvedToken
    {
        public string OriginalToken { get; init; }
        public int? Value { get; init; }
        public TokenParseMethod ParseMethod { get; init; }

        public ResolvedToken(
            int? value,
            string originalToken,
            bool checkedActor = true,
            TokenParseMethod failureType = TokenParseMethod.FAILED_PARSE_UNKNOWN_CAUSE
        )
        {
            Value = value; OriginalToken = originalToken;
            if (value is null)
            {
                ParseMethod = failureType;
            }
            else
            {
                ParseMethod = checkedActor ?
                    TokenParseMethod.STAT_LOOKUP : TokenParseMethod.INIT_INT_PARSE;
            }
        }

        public override string ToString()
        {
            string toStr = $"('{OriginalToken}' -> ";
            toStr += Value is null ? $"err-{(int)ParseMethod})" : $"{Value})";
            return toStr;
        }
    }

    public class ResolvedPool
    {
        public string OriginalPool { get; init; }
        public List<ResolvedToken> Tokens { get; private set; }
        public int Total
        {
            get
            {
                int total = 0;
                foreach (ResolvedToken tok in Tokens) { total += tok.Value ?? 0; }
                return total;
            }
        }

        public ResolvedPool(string pool, params ResolvedToken[] tokens)
        {
            OriginalPool = pool; Tokens = new(tokens);
        }

        public ResolvedPool(string pool, List<ResolvedToken> tokens)
        {
            OriginalPool = pool; Tokens = new(tokens);
        }

        public bool AnyTokenHasError()
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                var parseMethod = Tokens[i].ParseMethod;
                switch (parseMethod)
                {
                    case TokenParseMethod.FAILED_LOOKUP_INVALID_STAT:
                        return true;
                    case TokenParseMethod.FAILED_LOOKUP_STAT_NOT_FOUND_IN_ACTOR:
                        return true;
                    case TokenParseMethod.FAILED_PARSE_UNKNOWN_CAUSE:
                        return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            string toStr = $"[ {Total} <= ";
            toStr += Tokens.Select((tok) => $"{tok}").ToArray().Join(" + ");
            toStr += " ]";
            return toStr;
        }
    }

    public static ResolvedToken ParseToken(string token, V5Entity actor)
    {
        bool parsed = Int32.TryParse(token, out int tokenIntParseValue);
        if (parsed)
        {
            return new ResolvedToken(tokenIntParseValue, token, false);
        }

        if (SpecialTokens.Contains(token.ToLower()))
        {
            return ParseSpecialToken(token, actor);
        }

        try
        {
            int tokenLookupValue = actor.Block.GetStat(token);
            return new ResolvedToken(tokenLookupValue, token);
        }
        catch (ArgumentNullException)
        {
            GD.Print($"ParseToken(): Stat '{token}' not found in actor {actor}!");
            return new ResolvedToken(
                null, token, failureType: TokenParseMethod.FAILED_LOOKUP_STAT_NOT_FOUND_IN_ACTOR
            );
        }
        catch (MissingMemberException)
        {
            GD.Print($"ParseToken(): Stat '{token}' not valid/not found in stat books!");
            return new ResolvedToken(
                null, token, failureType: TokenParseMethod.FAILED_LOOKUP_INVALID_STAT
            );
        }
        catch (Exception ex)
        {
            GD.Print($"ParseToken(): Stat '{token}' not found for {actor}; cause unknown!\n{ex}");
            return new ResolvedToken(null, token);
        }
    }

    public static ResolvedToken ParseSpecialToken(string token, V5Entity actor)
    {
        throw new NotImplementedException(
            "Special tokens like Blood Surge have yet to be implemented!"
        );
    }

    public static List<string> ParsePoolIntoTokenStrings(string poolText)
    {
        string sanitizedPoolText = poolText.Replace(" ", "");
        string[] toks = sanitizedPoolText.Split('+');
        return new List<string>(toks);
    }

    public static ResolvedPool ParsePoolText(
        string poolText, V5Entity actor,
        List<string> mods = null, bool skipInvalidOperands = true
    )
    {
        // bool parsed = Int32.TryParse(poolText, out int parsedPoolTotal);
        // return parsed ? parsedPoolTotal : null;
        List<ResolvedToken> resolvedTokens = new();
        foreach (string token in ParsePoolIntoTokenStrings(poolText))
        {
            resolvedTokens.Add(ParseToken(token, actor));
        }

        return new ResolvedPool(poolText, resolvedTokens);
    }
}

public partial class Tracker
{
    public enum TRACKER_TYPE
    {
        HEALTH,
        WILLPOWER,
        HUMANITY,
        RAGE
    }

    public enum DMG_TYPE
    {
        NONE,
        SUPERFICIAL,
        SPF_UNHALVED,
        AGGRAVATED
    }

    public static readonly string[] TT_NAMES = { "HP", "Will", "Human", "Rage" };

    public TRACKER_TYPE TrackerType { get; init; }
    public V5Entity Owner { get; init; }
    public int Boxes
    {
        get
        {
            switch (TrackerType)
            {
                case TRACKER_TYPE.HEALTH:
                    int hp = Owner.Sta + 3;
                    if (Owner.HasPower(Cfg.Pwrs.FortHpBonus))
                    {
                        hp += Owner.Block.GetStatOrZero(Cfg.DiscFortitude.Id);
                    }
                    return hp;
                case TRACKER_TYPE.WILLPOWER:
                    return Owner.Com + Owner.Res;
                default:
                    GD.PrintErr($"TrackerType '{TrackerType}' not implemented.");
                    return 3;
            }
        }
    }
    public int SpfDamage { get; private set; }
    public int AggDamage { get; private set; }

    public Tracker(TRACKER_TYPE type, V5Entity owner, bool startClear = true)
    {
        if (type == TRACKER_TYPE.HUMANITY || type == TRACKER_TYPE.RAGE)
        {
            throw new NotImplementedException("Humanity and Rage Trackers not implemented yet.");
        }
        TrackerType = type;
        if (owner is null)
        {
            throw new ArgumentNullException($"{GetType()} 'Owner' must be a valid entity!");
        }
        Owner = owner;
        if (startClear)
        {
            Reset();
        }
    }

    protected void ProcessOnePointSpf()
    {
        if (SpfDamage > 0 && SpfDamage + AggDamage >= Boxes)
        {
            SpfDamage--; AggDamage++;
        }
        else
        {
            SpfDamage++;
        }
    }

    protected void ProcessOnePointAgg()
    {
        if (Owner.AggDmgMitigation > 0)
        {
            Owner.AggDmgMitigation--; SpfDamage++;
        }
        else
        {
            AggDamage++;
        }
    }

    public void SetAggDamage(int amount) { AggDamage = amount; }
    public void SetSpfDamage(int amount) { SpfDamage = amount; }

    public void TakeDamage(int amount, DMG_TYPE type, V5Entity source = null)
    {
        int totalDamage = type == DMG_TYPE.NONE ? 0 : amount;
        if (type == DMG_TYPE.SUPERFICIAL || type == DMG_TYPE.SPF_UNHALVED)
        {
            totalDamage = Math.Max(1, totalDamage - Owner.SpfDmgReduction);
            if (type == DMG_TYPE.SUPERFICIAL)
            {
                totalDamage = (int)Math.Ceiling((decimal)totalDamage / 2);
            }
        }

        for (int i = 0; i < totalDamage; i++)
        {
            if (type == DMG_TYPE.AGGRAVATED)
            {
                ProcessOnePointAgg();
            }
            else if (type == DMG_TYPE.SUPERFICIAL || type == DMG_TYPE.SPF_UNHALVED)
            {
                ProcessOnePointSpf();
            }

            if (AggDamage >= Boxes)
            {
                Owner.Collapse(TrackerType, source); break;
            }
            else if (SpfDamage + AggDamage >= Boxes)
            {
                Owner.SetImpairment(TrackerType, true);
            }
        }
    }

    public void Reset()
    {
        SpfDamage = 0; AggDamage = 0;
    }

    public override string ToString()
    {
        string toStr = $"{TT_NAMES[(int)TrackerType]}: |";
        for (int i = 0; i < AggDamage; i++)
        {
            toStr += "x|";
        }
        for (int i = 0; i < SpfDamage; i++)
        {
            toStr += "/|";
        }
        for (int i = 0; i < Boxes - SpfDamage - AggDamage; i++)
        {
            toStr += "_|";
        }
        return toStr;
    }
}

public partial class V5Effect
{
}