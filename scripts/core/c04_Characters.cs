using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class PronounSet
{
    public PronounSet(
        string shehethey, string herhimthem, string herhistheir,
        string sheshestheyre, string sheisheistheyare,
        string shellhelltheyll, string herselfhimselfthemself,
        string strange_adult, string strange_child,
        string sheshestheyve, string shehashehastheyhave
    )
    {
        SheHeThey = Id = shehethey;
        HerHimThem = herhimthem;
        HerHisTheir = herhistheir;
        ShesHesTheyre = sheshestheyre;
        SheIsHeIsTheyAre = sheisheistheyare;
        ShellHellTheyll = shellhelltheyll;
        HerselfHimselfThemself = herselfhimselfthemself;
        StrangeAdult = strange_adult;
        StrangeChild = strange_child;
        ShesHesTheyve = sheshestheyve;
        SheHasHeHasTheyHave = shehashehastheyhave;
    }

    public string Id { get; init; }
    public string SheHeThey { get; init; }
    public string HerHimThem { get; init; }
    public string HerHisTheir { get; init; }
    public string ShesHesTheyre { get; init; }
    public string SheIsHeIsTheyAre { get; init; }
    public string ShellHellTheyll { get; init; }
    public string HerselfHimselfThemself { get; init; }
    public string StrangeAdult { get; init; }
    public string StrangeChild { get; init; }
    public string ShesHesTheyve { get; init; }
    public string SheHasHeHasTheyHave { get; init; }

    public string Subjective { get => this.SheHeThey; }
    public string Objective { get => this.HerHimThem; }
    public string Possessive { get => this.HerHisTheir; }
    public string Descriptive { get => this.ShesHesTheyre; }
    public string DescriptiveLong { get => this.SheIsHeIsTheyAre; }
    public string Predictive { get => this.ShellHellTheyll; }
    public string Reflexive { get => this.HerselfHimselfThemself; }
}

public partial class CharBackground : Godot.Resource
{
    public readonly string Name;
    public readonly V5StatBlock BaseStatBlock;
    public readonly string Desc;

    public CharBackground(string name)
    {
        Name = name; Desc = "Empty background; all stats set to 1 or 0";
        BaseStatBlock = V5StatBlock.GetBlankBlock();
    }

    public CharBackground(
        string name, Godot.Collections.Dictionary<string, int> baseStats, string desc
    )
    {
        Name = name; BaseStatBlock = new(baseStats); Desc = desc;
    }
}

public partial class Cfg
{
    public readonly static PronounSet Woman = new PronounSet(
        "she", "her", "her", "she's", "she is", "she'll", "herself",
        "woman", "girl", "she's", "she has"
    );

    public readonly static PronounSet Man = new PronounSet(
        "he", "him", "his", "he's", "he is", "he'll", "himself",
        "man", "boy", "he's", "he has"
    );

    public readonly static PronounSet Person = new PronounSet(
        "they", "them", "their", "they're", "they are", "they'll", "themself",
        "person", "kid", "they've", "they have"
    );

    public readonly static PronounSet Thing = new PronounSet(
        "it", "it", "its", "it's", "it is", "it'll", "itself",
        "adult", "juvenile", "it's", "it has"
    );

    // Woman, Man, Person, Camera
    public readonly static PronounSet[] PcGenders = { Woman, Man, Person };
    public readonly static PronounSet[] PronounSetsAll = { Woman, Man, Person, Thing };
    public readonly static System.Collections.Generic.Dictionary<string, PronounSet> PronounSetsDict = new();

    public static readonly CharBackground BgEmpty = new("None");

    public static readonly CharBackground BgBartender = new(
        "Bartender",
        // Jack of all Trades (one at 3, five at 2, six at 1)
        new Godot.Collections.Dictionary<string, int>
        {
            [Cfg.AttrStr.Id] = 2,
            [Cfg.AttrDex.Id] = 3,
            [Cfg.AttrSta.Id] = 2,
            [Cfg.AttrCha.Id] = 2,
            [Cfg.AttrMan.Id] = 2,
            [Cfg.AttrCom.Id] = 4,
            [Cfg.AttrInt.Id] = 1,
            [Cfg.AttrWit.Id] = 3,
            [Cfg.AttrRes.Id] = 3,

            // Observation | Craft, Diplomacy, Intrigue, Praxis, Streetwise | Combat, Infiltration, Intimidation, Medicine, Performance, Technology
            [Cfg.SkillObse.Id] = 3,
            [Cfg.SkillCraf.Id] = 2,
            [Cfg.SkillDipl.Id] = 2,
            [Cfg.SkillIntr.Id] = 2,
            [Cfg.SkillPrax.Id] = 2,
            [Cfg.SkillStre.Id] = 2,
            [Cfg.SkillComb.Id] = 1,
            [Cfg.SkillInfi.Id] = 1,
            [Cfg.SkillInti.Id] = 1,
            [Cfg.SkillMedi.Id] = 1,
            [Cfg.SkillPerf.Id] = 1,
            [Cfg.SkillTech.Id] = 1
        },
        "Cool and confident, if lacking in formal education. Great at listening, decent at most other things."
    );

    public static readonly CharBackground BgInfluencer = new(
        "Influencer",
        // Specialist (One at 4, two at 3, one at 2, two at 1)
        new Godot.Collections.Dictionary<string, int>
        {
            [Cfg.AttrStr.Id] = 2,
            [Cfg.AttrDex.Id] = 2,
            [Cfg.AttrSta.Id] = 3,
            [Cfg.AttrCha.Id] = 3,
            [Cfg.AttrMan.Id] = 4,
            [Cfg.AttrCom.Id] = 2,
            [Cfg.AttrInt.Id] = 2,
            [Cfg.AttrWit.Id] = 1,
            [Cfg.AttrRes.Id] = 3,

            // Performance | Intrigue, Technology | Praxis | Diplomacy, Occult
            [Cfg.SkillPerf.Id] = 4,
            [Cfg.SkillIntr.Id] = 3,
            [Cfg.SkillTech.Id] = 3,
            [Cfg.SkillPrax.Id] = 2,
            [Cfg.SkillDipl.Id] = 1,
            [Cfg.SkillOccu.Id] = 1
        },
        "Driven, focused, brimming with superficial cheek and charm. Laser-focused on the grind."
    );

    public static readonly CharBackground BgMedStudent = new(
        "Med Student",
        // Specialist (One at 4, two at 3, one at 2, two at 1)
        new Godot.Collections.Dictionary<string, int>
        {
            [Cfg.AttrStr.Id] = 2,
            [Cfg.AttrDex.Id] = 2,
            [Cfg.AttrSta.Id] = 1,
            [Cfg.AttrCha.Id] = 2,
            [Cfg.AttrMan.Id] = 2,
            [Cfg.AttrCom.Id] = 3,
            [Cfg.AttrInt.Id] = 4,
            [Cfg.AttrWit.Id] = 3,
            [Cfg.AttrRes.Id] = 3,

            // Medicine | Academics, Observation | Technology | Animal Ken, Diplomacy
            [Cfg.SkillMedi.Id] = 4,
            [Cfg.SkillAcad.Id] = 3,
            [Cfg.SkillObse.Id] = 3,
            [Cfg.SkillTech.Id] = 2,
            [Cfg.SkillAnim.Id] = 1,
            [Cfg.SkillDipl.Id] = 1
        },
        "Studious and thoughtful, but long nights have taken a toll on physical health."
    );

    public static readonly CharBackground BgStarAthlete = new(
        "Star Athlete",
        // Balanced (Two at 3, four at 2, three at 1)
        new Godot.Collections.Dictionary<string, int>
        {
            [Cfg.AttrStr.Id] = 3,
            [Cfg.AttrDex.Id] = 3,
            [Cfg.AttrSta.Id] = 4,
            [Cfg.AttrCha.Id] = 2,
            [Cfg.AttrMan.Id] = 1,
            [Cfg.AttrCom.Id] = 2,
            [Cfg.AttrInt.Id] = 2,
            [Cfg.AttrWit.Id] = 3,
            [Cfg.AttrRes.Id] = 2,

            // Athletics, Performance | Academics, Combat, Observation, Traversal | Diplomacy, Intimidation, Medicine
            [Cfg.SkillAthl.Id] = 3,
            [Cfg.SkillPerf.Id] = 3,
            [Cfg.SkillAcad.Id] = 2,
            [Cfg.SkillComb.Id] = 2,
            [Cfg.SkillObse.Id] = 2,
            [Cfg.SkillTrav.Id] = 2,
            [Cfg.SkillDipl.Id] = 1,
            [Cfg.SkillInti.Id] = 1,
            [Cfg.SkillMedi.Id] = 1
        },
        "Strong and stately, with an easy smile. Tireless with a goal in mind, but surprisingly unassertive."
    );

    public static readonly CharBackground BgVeteran = new(
        "Veteran",
        // Balanced (Two at 3, four at 2, three at 1)
        new Godot.Collections.Dictionary<string, int>
        {
            [Cfg.AttrStr.Id] = 2,
            [Cfg.AttrDex.Id] = 3,
            [Cfg.AttrSta.Id] = 3,
            [Cfg.AttrCha.Id] = 1,
            [Cfg.AttrMan.Id] = 2,
            [Cfg.AttrCom.Id] = 2,
            [Cfg.AttrInt.Id] = 3,
            [Cfg.AttrWit.Id] = 4,
            [Cfg.AttrRes.Id] = 2,

            // Combat, Firearms | Intimidation, Medicine, Observation, Traversal | Animal Ken, Athletics, Streetwise
            [Cfg.SkillComb.Id] = 3,
            [Cfg.SkillFire.Id] = 3,
            [Cfg.SkillInti.Id] = 2,
            [Cfg.SkillMedi.Id] = 2,
            [Cfg.SkillObse.Id] = 2,
            [Cfg.SkillTrav.Id] = 2,
            [Cfg.SkillAnim.Id] = 1,
            [Cfg.SkillAthl.Id] = 1,
            [Cfg.SkillStre.Id] = 1
        },
        "Quick-witted and tough, but standoffish and mistrustful due to trial and trauma."
    );

    public static readonly CharBackground BgContractor = new(
        // Still working on this one, may change completely
        "Contractor",
        // Jack of all Trades (one at 3, five at 2, six at 1)
        new Godot.Collections.Dictionary<string, int>
        {
            [Cfg.AttrStr.Id] = 3,
            [Cfg.AttrDex.Id] = 3,
            [Cfg.AttrSta.Id] = 2,
            [Cfg.AttrCha.Id] = 2,
            [Cfg.AttrMan.Id] = 1,
            [Cfg.AttrCom.Id] = 2,
            [Cfg.AttrInt.Id] = 3,
            [Cfg.AttrWit.Id] = 2,
            [Cfg.AttrRes.Id] = 4,

            // Investigation | Craft, Diplomacy, Intrigue, Streetwise. Sciences | Combat, Praxis, Infiltration, Intimidation, Performance, Technology
            [Cfg.SkillObse.Id] = 3,
            [Cfg.SkillCraf.Id] = 2,
            [Cfg.SkillDipl.Id] = 2,
            [Cfg.SkillIntr.Id] = 2,
            [Cfg.SkillStre.Id] = 2,
            [Cfg.SkillMedi.Id] = 2,
            [Cfg.SkillComb.Id] = 1,
            [Cfg.SkillInfi.Id] = 1,
            [Cfg.SkillInti.Id] = 1,
            [Cfg.SkillPerf.Id] = 1,
            [Cfg.SkillPrax.Id] = 1,
            [Cfg.SkillTech.Id] = 1
        },
        "dsfsdfdsfdsfd."
    );

    // Currently the list in the character creation UI isn't autogenerated, so it must match this order.
    public readonly static CharBackground[] PcBackgrounds =
    {
        BgBartender, BgInfluencer, BgMedStudent, BgStarAthlete, BgVeteran
    };

    public readonly static System.Collections.Generic.Dictionary<string, CharBackground> PcBgsDict = new();
}

public class V5Clan
{
    public V5Clan(
        string name, string epithet, string informal, string slur,
        V5Stat[] inclans
    )
    {
        Name = name; Epithet = epithet; Informal = informal; Slur = slur;
        InClanDisciplines = inclans;
    }

    public string Name { get; init; }
    public string Informal { get; init; }
    public string Epithet { get; init; }
    public string Slur { get; init; }

    public V5Stat[] InClanDisciplines { get; init; }
}

public partial class Cfg
{
    public static V5Clan ClanBrujah = new V5Clan(
        "Brujah", "the Learned Clan", "Hellene", "Rabble",
        new V5Stat[] { Cfg.DiscCelerity, Cfg.DiscPotence, Cfg.DiscPresence }
    );

    public static V5Clan ClanNosferatu = new V5Clan(
        "Nosferatu", "the Hidden Clan", "Orlok", "Sewer Rat",
        new V5Stat[] { Cfg.DiscAnimalism, Cfg.DiscObfuscate, Cfg.DiscPotence }
    );

    public static V5Clan ClanRavnos = new V5Clan(
        "Ravnos", "the Doomed Clan", "Daredevil", "Raven",
        new V5Stat[] { Cfg.DiscAnimalism, Cfg.DiscObfuscate, Cfg.DiscPresence }
    );

    public static V5Clan ClanVentrue = new V5Clan(
        "Ventrue", "the Clan of Kings", "Blue-Blood", "Tyrant",
        new V5Stat[] { Cfg.DiscDominate, Cfg.DiscFortitude, Cfg.DiscPresence }
    );

    public readonly static V5Clan[] PcClans = {
        ClanBrujah,
        ClanNosferatu,
        ClanRavnos,
        ClanVentrue
    };
}

public partial class TestThing : RefCounted
{

}

public partial class V5Entity : Godot.Resource
{
    private static readonly List<V5Entity> _allEntities = new() { null };

    public static readonly V5Entity Dummy = new() { Name = "Dummy" };

    protected bool _initialized = false;

    public int numDiceTests = 0;

    [Signal] public delegate void PcUpdateEventHandler(Cfg.UI_KEY source, PlayerChar pc);

    public V5Entity()
    {
        Block = V5StatBlock.GetBlankBlock();
        Hp = new(Tracker.TRACKER_TYPE.HEALTH, this);
        Will = new(Tracker.TRACKER_TYPE.WILLPOWER, this);
        Initialize();
    }

    public V5Entity(
        Godot.Collections.Dictionary<string, int> baseStats,
        Godot.Collections.Dictionary<string, int> xpCounts
    )
    {
        Block = new V5StatBlock(baseStats, xpCounts);
        Hp = new(Tracker.TRACKER_TYPE.HEALTH, this);
        Will = new(Tracker.TRACKER_TYPE.WILLPOWER, this);
        Initialize();
    }

    private int _hunger = 0;
    private int _aggDmgMitigationPoints = 0;

    public V5StatBlock Block { get; private set; }
    public Tracker Hp { get; init; }
    public Tracker Will { get; init; }

    public string GdtResourceId { get; private set; }
    public string Id { get; private set; }
    public string Name { get; set; }
    public PronounSet Pns { get; set; }
    public bool HasHunger
    {
        get => true; // TODO: Implement creature types.
    }
    public int Hunger
    {
        get => HasHunger ? _hunger : 0;
        set => _hunger = value;
    }
    public bool IsDead { get; private set; } = false;
    public bool CanThink { get; private set; } = true;
    public int UnspentXp { get; private set; } = 0;

    // Currently in dollars, no further granularity.
    public int Cash { get; set; } = 100; // TODO: Dollars-to-cents thing later, maybe?

    protected void Initialize()
    {
        GdtResourceId = base.ToString();
        Id = $"-Ent-{V5Entity._allEntities.Count}"; Name ??= Id;
        Pns = Cfg.Person;
        _initialized = true;
        AddNewEntity();
    }

    private void AddNewEntity() // TODO: Get rid of this later.
    {
        // GD.Print($"\n -- AddNewEntity() called: {this}/'{Id}' --\n");
        _allEntities.Add(this);
    }

    public static List<V5Entity> GetAllEntities()
    {
        return _allEntities;
    }

    public static V5Entity GetEntityFromList(int index, V5Entity fallbackEnt = null)
    {
        V5Entity ent = GetAllEntities()[index];
        ent ??= fallbackEnt ?? V5Entity.Dummy;
        return ent;
    }

    public int Str { get => Block.GetAttr(Cfg.AttrStr.Id); }
    public int Dex { get => Block.GetAttr(Cfg.AttrDex.Id); }
    public int Sta { get => Block.GetAttr(Cfg.AttrSta.Id); }
    public int Cha { get => Block.GetAttr(Cfg.AttrCha.Id); }
    public int Man { get => Block.GetAttr(Cfg.AttrMan.Id); }
    public int Com { get => Block.GetAttr(Cfg.AttrCom.Id); }
    public int Int { get => Block.GetAttr(Cfg.AttrInt.Id); }
    public int Wit { get => Block.GetAttr(Cfg.AttrWit.Id); }
    public int Res { get => Block.GetAttr(Cfg.AttrRes.Id); }

    public bool Initialized { get => _initialized; }

    // TODO: Change this to either class or struct that includes prerequisites
    public readonly List<V5Power> Powers = new();
    public readonly List<string> Buffs = new();

    public int GetStatOrZero(string statField)
    {
        return Block?.GetStatOrZero(statField) ?? 0;
    }

    public bool HasDisc(string discField)
    {
        return Block.GetStatOrZero(discField) > 0;
    }

    // TODO: Implement these, along with structs for powers/buffs
    public bool HasPower(V5Power power)
    {
        bool pwrCmpr(V5Power pwr)
        {
            return pwr == power || pwr.Id == power.Id || pwr.Name == power.Name;
        }
        V5Power[] matchingPowers = Powers.Where(pwrCmpr).ToArray();
        return matchingPowers.Length > 0;
    }

    public bool HasEffect(string effectName)
    {
        return Buffs.Contains(effectName);
    }

    public int SpfDmgReduction
    {
        get
        {
            int fortLevel = Block.GetStatOrZero(Cfg.DiscFortitude.Id);
            if (fortLevel > 0 && HasEffect("Toughness"))
            {
                return fortLevel;
            }
            return 0;
        }
    }
    public int AggDmgMitigation
    {
        get => HasEffect("Defy Bane") ? _aggDmgMitigationPoints : 0;
        set => _aggDmgMitigationPoints = Math.Max(0, value);
    }

    public void SetImpairment(Tracker.TRACKER_TYPE trackerType, bool impaired = true)
    {
        // TODO: Should Impairment be Buff or internal bool value?
        GD.Print($"Impairment ({trackerType}) for {this} set to {impaired}.");
    }

    public void Collapse(Tracker.TRACKER_TYPE trackerType, V5Entity lastSource = null)
    {
        if (trackerType == Tracker.TRACKER_TYPE.HEALTH)
        {
            if (this is PlayerChar)
            {
                HandlePcDeath();
            }
            else
            {
                IsDead = true;
            }
        }
        else if (trackerType == Tracker.TRACKER_TYPE.WILLPOWER)
        {
            CanThink = false;
        }
        else
        {
            throw new NotImplementedException("Humanity and Rage Trackers not implemented yet!");
        }
    }

    public void HandlePcDeath()
    {
        GD.Print("\n\n --- LMAO u ded (if implemented) ---\n\n");
    }

    public override string ToString()
    {
        // return base.ToString();
        return $"{Name}";
    }

    public virtual V5EntBundle Bundle()
    {
        return new()
        {
            Name = Name,
            PronounSetId = Pns.Id,
            BackgroundName = "None (V5Entity)",
            StatVals = Block.GetStatBasesDict(),
            XpVals = Block.GetStatXpCountsDict(),
            Hunger = Hunger,
            HpDmgSpf = Hp.SpfDamage,
            HpDmgAgg = Hp.AggDamage,
            WillDmgSpf = Will.SpfDamage,
            WillDmgAgg = Will.AggDamage,
            NumDiceTests = numDiceTests,
            Cash = Cash
        };
    }

    public static V5Entity BuildFromBundle(V5Entity ent, V5EntBundle bundle)
    {
        ent.Name = bundle.Name; ent.Pns = Cfg.PronounSetsDict[bundle.PronounSetId];
        ent.Block = new(bundle.StatVals, bundle.XpVals);
        // Note: Check specs for 'private set' and make sure this ^ works.
        ent.Hp.SetAggDamage(bundle.HpDmgAgg); ent.Hp.SetSpfDamage(bundle.HpDmgSpf);
        ent.Will.SetAggDamage(bundle.WillDmgAgg); ent.Will.SetSpfDamage(bundle.WillDmgSpf);
        ent.Hunger = ent.HasHunger ? bundle.Hunger : 0;
        ent.Cash = bundle.Cash;
        ent.numDiceTests = bundle.NumDiceTests;
        return ent;
    }
}

public partial class PlayerChar : V5Entity
{
    private CharBackground _Background;

    public PlayerChar() : base()
    {
        GD.Print("Empty PlayerChar constructor called.");
        Background = Cfg.BgEmpty;
    }

    public PlayerChar(
        Godot.Collections.Dictionary<string, int> baseStats,
        Godot.Collections.Dictionary<string, int> xpCounts
    ) : base(baseStats, xpCounts)
    {
        GD.Print("Second PlayerChar constructor called.");
    }

    public CharBackground Background
    {
        get => _Background;
        set
        {
            _Background = value;
            foreach (V5Stat stat in Cfg.AllStats)
            {
                Tuple<int?, int?> statPair = Background.BaseStatBlock.GetStatPair(stat.Id);
                int newStatValue = statPair.Item1 is int ? (int)statPair.Item1 : stat.Min;
                // GD.Print($"Setting {stat.Id} to {newStatValue} for {this} (w/ Block {Block}) b/c statPair.Item1 = {statPair.Item1}");
                Block.SetBaseStat(stat.Id, newStatValue);
            }
        }
    }

    public MapLoc PreviousLocation { get; private set; }
    private MapLoc _currentLocation;
    public MapLoc CurrentLocation
    {
        get => _currentLocation;
        set
        {
            // TODO, maybe: Signal?
            if (_currentLocation != value)
            {
                PreviousLocation = _currentLocation;
                _currentLocation = value;
                GD.Print(
                    $"PC: Moving from {PreviousLocation?.LocName ?? "an unknown location"} " +
                    // $"PC: Moving from {_currentLocation?.LocName ?? "an unknown location"} " +
                    $"to {value}..."
                );
            }
            else
            {
                GD.PrintErr($"PC: Already at '{_currentLocation}' ('{value}')!");
            }
        }
    }

    public bool Sheltered { get; set; }
    public bool Traveling { get; set; }
    public bool InPublic { get; set; }

    public override V5EntBundle Bundle()
    {
        V5EntBundle bundle = base.Bundle();
        bundle.BackgroundName = Background.Name;
        bundle.Sheltered = Sheltered;
        bundle.Traveling = Traveling;
        bundle.InPublic = InPublic;
        return bundle;
    }

    public static PlayerChar BuildFromBundle(PlayerChar pc, V5EntBundle bundle)
    {
        pc = (PlayerChar)V5Entity.BuildFromBundle(pc, bundle);
        string bgKey = bundle.BackgroundName;
        pc.Background = Cfg.PcBgsDict.ContainsKey(bgKey) ? Cfg.PcBgsDict[bgKey] : Cfg.BgEmpty;
        pc.Sheltered = bundle.Sheltered;
        pc.Traveling = bundle.Traveling;
        pc.InPublic = bundle.InPublic;
        return pc;
    }
}
