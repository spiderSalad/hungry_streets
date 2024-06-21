using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

public readonly struct PronounSet
{
    public PronounSet(
        string shehethey, string herhimthem, string herhistheir,
        string sheshestheyre, string sheisheistheyare,
        string shellhelltheyll, string herselfhimselfthemself,
        string strange_adult, string strange_child,
        string sheshestheyve, string shehashehastheyhave
    )
    {
        SheHeThey = shehethey;
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

public readonly struct CharBackground
{
    public readonly string Name;
    public readonly V5StatBlock BaseStatBlock;
    public readonly string Desc;

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

    public static CharBackground BgBartender = new CharBackground(
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

    public static CharBackground BgInfluencer = new CharBackground(
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

    public static CharBackground BgMedStudent = new CharBackground(
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

    public static CharBackground BgStarAthlete = new CharBackground(
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

    public static CharBackground BgVeteran = new CharBackground(
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

    public static CharBackground BgContractor = new CharBackground( // Still working on this one,
                                                                    // may change completely
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

    public readonly static CharBackground[] PcBackgrounds =
    {
        BgBartender, BgInfluencer, BgMedStudent, BgStarAthlete, BgVeteran
    };
}

public readonly struct V5Clan
{
    public V5Clan(
        string name, string epithet, string informal, string slur,
        V5Stat[] inclans
    )
    {
        Name = name; Epithet = epithet; Informal = informal; Slur = slur;
        InClanDisciplines = inclans;
    }

    public readonly string Name { get; init; }
    public readonly string Informal { get; init; }
    public readonly string Epithet { get; init; }
    public readonly string Slur { get; init; }

    public readonly V5Stat[] InClanDisciplines { get; init; }
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

    [Signal]
    public delegate void CharacterUpdateEventHandler(string opt, Variant val);

    public V5Entity()
    {
        Godot.Collections.Dictionary<string, int> baseStats = new();
        Godot.Collections.Dictionary<string, int> xpCounts = new();
        foreach (V5Stat stat in Cfg.AllStats)
        {
            baseStats[stat.Id] = stat.Min;
            xpCounts[stat.Id] = 0;
        }
        Block = new V5StatBlock(baseStats, xpCounts);
        Initialize();
    }

    public V5Entity(Godot.Collections.Dictionary<string, int> baseStats, Godot.Collections.Dictionary<string, int> xpCounts)
    {
        Block = new V5StatBlock(baseStats, xpCounts);
        Initialize();
    }

    private int _hunger = 0;

    public V5StatBlock Block { get; init; }

    public string GdtResourceId { get; private set; }
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

    protected void Initialize()
    {
        GdtResourceId = base.ToString();
        Name ??= $"-Ent-{V5Entity._allEntities.Count}";
        Pns = Cfg.Person;
        _initialized = true;
        AddNewEntity();
    }

    private void AddNewEntity() // TODO: Get rid of this later.
    {
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

    // TODO: Change these to trackers.
    //public int MaxHp { get => Sta + 3; }
    //public int MaxWill { get => Com + Res; }

    public bool Initialized { get => _initialized; }

    public override string ToString()
    {
        // return base.ToString();
        return $"'{Name}'";
    }
}

public partial class PlayerChar : V5Entity
{
    private CharBackground _Background;

    public PlayerChar() : base() { GD.Print("Empty PlayerChar constructor called."); }

    public PlayerChar(
        Godot.Collections.Dictionary<string, int> baseStats,
        Godot.Collections.Dictionary<string, int> xpCounts
    ) : base(baseStats, xpCounts)
    {
        //Block = new V5StatBlock(baseStats, xpCounts);
        GD.Print("Second PlayerChar constructor called.");
    }

    public CharBackground Background
    {
        get => _Background;
        set
        {
            _Background = value;
            GD.Print($"Background changed to {Background.Name}");
            foreach (V5Stat stat in Cfg.AllStats)
            {
                Tuple<int?, int?> statPair = Background.BaseStatBlock.GetStatPair(stat.Id);
                int newStatValue = statPair.Item1 is int ? (int)statPair.Item1 : stat.Min;
                // GD.Print($"Setting {stat.Id} to {newStatValue} b/c statPair.Item1 = {statPair.Item1}");
                this.Block.SetBaseStat(stat.Id, newStatValue);
                // TO-DID (kinda): Start here, or actually fix/refactor stat block
                // should we use dictionary, hashset or hashtable for stats?
                // need to get a tuple<> for basestat and xpcount, but what if stat is missing?
                // do we want null?
            }
        }
    }
}
