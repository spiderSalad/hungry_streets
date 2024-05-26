using Godot;
using System;
using System.Collections.Generic;
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
    public static RollResultSet RollBones(int numDice, int numSides, RandomNumberGenerator rng = null)
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
            //int dieResult = (int)ldr;//rng.Randi() % (numSides + 1) + min;
            int dieResult = (int)(rng.Randi() % numSides + min);
            GD.Print($"RollBones: die #{i + 1}, range {min}-{numSides}, face => {dieResult}");
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
    private string _verdict;
    private RandomNumberGenerator _rng;

    public V5Entity Actor { get; init; }
    public bool UsingHunger { get; init; }
    public int? DefaultHunger { get; init; }

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

    public string Verdict
    { // TODO: change type from string
        get => _verdict;
        set
        {
            GD.Print("Verdict set!");
            _verdict = value;
        }
    }

    public V5Roll(
        V5Entity actor,
        int pool,
        bool useHunger = false,
        bool will2Win = false,
        bool will2Restrain = false,
        int? defaultHunger = null,
        RandomNumberGenerator rng = null,
        string separatorStr = ", " 
    )
    {
        Actor = actor;
        UsingHunger = useHunger; DefaultHunger = defaultHunger;
        Ankhs = 0;
        _blackPool = pool; _redPool = 0;
        _will2Win = will2Win; _will2Restrain = will2Restrain;
        BlackDiceResults = null; RedDiceResults = null;
        _rng = rng;
        _separator = separatorStr;
        if (_rng is null)
        {
            SetRng(new RandomNumberGenerator());
        }
        Roll();
    }

    public void SetRng(RandomNumberGenerator rng, bool randomize = true)
    {
        _rng = rng;
        if (randomize)
        {
            _rng.Randomize();
        }
    }

    protected int GetSingleDieRoll()
    {
        return (int)_rng.Randi() % (V5Roll.DIE_MAX + 1) + V5Roll.DIE_MIN;
    }

    public void Roll()
    {
        int hunger = 0; int actorHunger;
        if (UsingHunger)
        {
            try { actorHunger = Actor.Hunger; }
            catch (NullReferenceException) { actorHunger = 0; }
            hunger = DefaultHunger is null ? actorHunger : (int)DefaultHunger;
            _redPool = Math.Min(_blackPool, hunger);
            RedDiceResults = Utils.RollBones(_redPool, V5Roll.DIE_MAX, rng: _rng);
        }
        _blackPool = Math.Max(_blackPool - hunger, 0);
        BlackDiceResults = Utils.RollBones(_blackPool, V5Roll.DIE_MAX, rng: _rng);
        EvaluateRoll();
    }

    public void EvaluateRoll()
    {
        Ankhs = 0; BlackTens = 0; RedTens = 0; RedOnes = 0;
        for (int i = 0; i < _redPool; i++)
        {
            EvaluateDieResult(RedDiceResults.Original[i], true);
        }
        for (int i = 0; i < _blackPool; i++)
        {
            EvaluateDieResult(BlackDiceResults.Original[i], false);
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
    }

    private void EvaluateDieResult(int dieResult, bool isRed)
    {
        if (dieResult >= V5Roll.DIE_MAX)
        {
            RedTens++;
        }
        else if (isRed && dieResult == V5Roll.DIE_MIN)
        {
            RedOnes++;
        }
        else if (dieResult < V5Roll.DIE_MIN)
        {
            throw new ArgumentException(
                $"{GetType()}.EvaluateDieResult(): '{dieResult}' is below minimum ({V5Roll.DIE_MIN})!"
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
            if (will2Win && BlackDiceResults.Rerolled[i] < V5Roll.ANKH_THRESHOLD)
            {
                BlackDiceResults.Rerolled[i] = GetSingleDieRoll();
                diceRerolled++;
            }
            else if (will2Restrain && BlackDiceResults.Rerolled[i] >= V5Roll.DIE_MAX)
            {
                BlackDiceResults.Rerolled[i] = GetSingleDieRoll();
                diceRerolled++;
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

    public override string ToString()
    {
        string rollStr = "", sep = _separator;
        List<int> blackNums = CanReroll ? BlackDiceResults.Original : BlackDiceResults.Rerolled;
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

    public string ToBBCodeString()
    {
        return V5Roll.BBCodeFormatRollResults(this.ToString(), _separator);
    }

    public static string BBCodeFormatRollResults(string unformattedV5RollStr, string sep = ", ")
    {
        Regex rgx = new("<r[ ]*(.+?)[ ]*>");
        string formattedV5RollStr = rgx.Replace(unformattedV5RollStr, "[color=#f00]< $1 >[/color]");
        int nthComma, placeholder = 0;
        string newline = "\n";
        do
        {
            nthComma = Utils.FindNthOccurrence(formattedV5RollStr.Substring(placeholder), sep, 5);
            if (nthComma > -1) {
                formattedV5RollStr = formattedV5RollStr.Substring(0, placeholder + nthComma + sep.Length)
                + newline
                + formattedV5RollStr.Substring(placeholder + nthComma + sep.Length);
                placeholder += newline.Length;
            }
            placeholder += nthComma + sep.Length;
        } while (nthComma > -1 && placeholder < formattedV5RollStr.Length);
        return formattedV5RollStr;
    }
}