using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static partial class Cfg
{
    public const bool DEV_MODE = true; // TODO: Find a way to export this/move it out of here

    public const int ATTR_MULT = 5;
    public const int SKILL_MULT = 3;
    public const int DISC_MULT_IN_CLAN = 5;
    public const int DISC_MULT_CAITIFF = 6;
    public const int DISC_MULT_OUT_CLAN = 7;
    public const int BG_MULT = 3; // was 5 for some reason?

    public const int DOT_MAX = 5;
    public const int NPC_SKILL_MAX = 15;
    public const int DOT_MIN = 0;
    public const int ATTR_MIN = 1;

    public static readonly string[] NUM_WORDS = {
        "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"
    };

    // Check these
    public const string KEY_ID = "Id";
    public const string KEY_NAME = "Name";
    public const string KEY_STATVALUE = "Stat_Value";
    public const string KEY_DESC = "Description";
    public const string KEY_BACKGROUND = "Background";
    public const string KEY_PRONOUNS = "Pronouns/Gender";

    public const string SCENE_ROOT = "root_scene";
    public const string SCENE_CCUI = "char_creation_ui_scene";
    public const string SCENE_OWUI = "overworld_ui";

    public enum GROUP_NAMES
    {
        DPUI_INPUTS,
        DPUI_DISPLAYS,
        CCUI_FORM_INPUTS,
        CCUI_FORM_DISPLAYS,
        OWUI_CONTROLS,
        OWUI_DISPLAYS,

        UI_ACTIVE_ROLL,
        UI_REACTING_ROLL
    }

    public enum UI_KEY
    {
        FORM_SUBMIT,
        SWITCH_SCENE,
        DICE_TEST_1_TEST,
        DICE_TEST_2_REROLL_WIN,
        DICE_TEST_3_REROLL_RESTRAIN,
        DICE_TEST_4_CONTEST,
        NAME_ENTRY,
        PRONOUN_SELECT,
        BACKGROUND_SELECT,
        SELECT_ACTIVE_ENTITY_ALL,
        SELECT_REACTING_ENTITY_ALL
    }

    public const string NODEPATH_ABS_GAMEMANAGER = "/root/GameManager";

    public const string PATH_SOUND = "res://audio/sounds/";
}

public class V5Stat
{
    public V5Stat(string id, string name, int min = Cfg.DOT_MIN, string desc = "...")
    {
        Id = id; Name = name; Min = min;
        Max = 15;// Cnfg.AttrsDict.ContainsKey(id) ? Cnfg.DOT_MAX : Cnfg.NPC_SKILL_MAX;
        Desc = desc == "..." ? $"V5 attribute, skill, discipline, trait or background \"{name}\"" : desc;
    }

    public string Id { get; init; }
    public string Name { get; init; }
    public int Min { get; init; }
    public int Max { get; init; }
    public string Desc { get; init; }

    public override string ToString()
    {
        // return base.ToString();
        return $"{Id}";
    }
}

public partial class Cfg
{
    public static V5Stat AttrPhys = new V5Stat("npc_physical", "Physical", 1, "...");
    public static V5Stat AttrSoci = new V5Stat("npc_social", "Social", 1, "...");
    public static V5Stat AttrMent = new V5Stat("npc_mental", "Mental", 1, "...");

    public static V5Stat AttrStr = new V5Stat("str", "Strength", 1,
        "Lifting, pulling, pushing, punching, kicking, etc. Bodily power.");
    public static V5Stat AttrDex = new V5Stat("dex", "Dexterity", 1,
        "Coordination, acuity, speed. Everything from sprinting to APM to aiming a gun. Bodily finesse.");
    public static V5Stat AttrSta = new V5Stat("sta", "Stamina", 1,
        "Bodily resilience and anatomical fortitude. How much punishment you can take.");
    public static V5Stat AttrCha = new V5Stat("cha", "Charisma", 1,
        "Getting people to like you, fear you, want you - making them feel about you. Social power.");
    public static V5Stat AttrMan = new V5Stat("man", "Manipulation", 1,
        "Getting people to do what you want, regardless of how they feel about you. Social finesse.");
    public static V5Stat AttrCom = new V5Stat("com", "Composure", 1,
        "Social resilience, or the ability to stay cool in the moment.");
    public static V5Stat AttrInt = new V5Stat("int", "Intelligence", 1,
        "Learning, reasoning, problem-solving, memory - mental power.");
    public static V5Stat AttrWit = new V5Stat("wit", "Wits", 1,
        "Reaction speed, intuition, perception, the ability to think on your feet. Mental finesse.");
    public static V5Stat AttrRes = new V5Stat("res", "Resolve", 1,
        "Focus and determination sustained over time. The ability to ignore distractions. Mental resilience.");

    public readonly static V5Stat[] Attrs = {
        AttrStr, AttrDex, AttrSta, AttrCha, AttrMan, AttrCom, AttrInt, AttrWit, AttrRes
    };
    public readonly static System.Collections.Generic.Dictionary<string, V5Stat> AttrsDict =
        new System.Collections.Generic.Dictionary<string, V5Stat>();

    public static V5Stat SkillAthl = new V5Stat("athl", "Athletics",
        desc: "Experience, form, and training for coordinated physical exertion of various kinds.");
    public static V5Stat SkillComb = new V5Stat("comb", "Combat",
        desc: "Throwing hands - or wielding the kind of weapons that you bash, cut, or stab with."); // Brawl, Melee
    public static V5Stat SkillCraf = new V5Stat("craf", "Craft",
        desc: "Working with your hands or tools to create a physical product or result, from artisanship to plastic surgery.");
    public static V5Stat SkillFire = new V5Stat("fire", "Firearms",
        desc: "The proper handling and safe (for you, anyway) usage of firearms.");
    public static V5Stat SkillInfi = new V5Stat("infi", "Infiltration",
        desc: "Understand of physical security, and the subversion thereof. Breaking and entering without being noticed."); // Larceny, Stealth
    public static V5Stat SkillTrav = new V5Stat("trav", "Traversal",
        desc: "Making your way around urban environments - braving their dangers and adapting to them."); // Drive, Survival

    public static V5Stat SkillAnim = new V5Stat("anim", "Animal Ken",
        desc: "Intuitive understand of animal behavior and perception. Getting animals to trust you, or at least tolerate your presence.");
    public static V5Stat SkillDipl = new V5Stat("dipl", "Diplomacy",
        desc: "Getting people to see things your way, and delegating responsibilities effectively."); // Persuasion, Leadership
    public static V5Stat SkillInti = new V5Stat("inti", "Intimidation",
        desc: "Threats, overt or implicit. Getting others to back down or fall in line, without resorting to powers of the Blood.");
    public static V5Stat SkillIntr = new V5Stat("intr", "Intrigue",
        desc: "Dissembling, sophistry, subterfuge, and straight-up lies - how to recognize them and how to emnploy them."); // Insight, Subterfuge
    public static V5Stat SkillPerf = new V5Stat("perf", "Performance",
        desc: "Understanding and meeting expectations in matters of art or etiquette. Putting on a show, literally or figuratively."); // Etiquette, Performance
    public static V5Stat SkillStre = new V5Stat("stre", "Streetwise",
        desc: "Understanding how things work on the margins of society. Physical shortcuts, legal grey areas, metaphorical seedy underbellies.");

    public static V5Stat SkillAcad = new V5Stat("acad", "Academics",
        desc: "History, philosophy, art, literature, logic, science, mathematics. Research skills."); // Academics, Science
    public static V5Stat SkillMedi = new V5Stat("medi", "Medicine",
        desc: "Practical medical expertise, however it was gained. Anatomical knowledge and first aid skills.");
    public static V5Stat SkillObse = new V5Stat("obse", "Observation",
        desc: "Noticing the right thing at the right time. Situational awareness and drawn-out analysis."); // Awareness, Investigation
    public static V5Stat SkillOccu = new V5Stat("occu", "Occult",
        desc: "Knowledge of - and facility with - the supernatural world. Mystic lore, real and fake.");
    public static V5Stat SkillPrax = new V5Stat("prax", "Praxis",
        desc: "Practical understanding - and therefore effective exploitation - of political and financial systems."); // Finance, Politics
    public static V5Stat SkillTech = new V5Stat("tech", "Technology",
        desc: "Secure use of information technology - apps, services, physical devices. Operating safely online.");

    public readonly static V5Stat[] Skills = {
        SkillAthl, SkillComb, SkillCraf, SkillFire, SkillInfi, SkillTrav,
        SkillAnim, SkillDipl, SkillInti, SkillIntr, SkillPerf, SkillStre,
        SkillAcad, SkillMedi, SkillObse, SkillOccu, SkillPrax, SkillTech
    };
    public readonly static System.Collections.Generic.Dictionary<string, V5Stat> SkillsDict =
        new System.Collections.Generic.Dictionary<string, V5Stat>();

    public static V5Stat DiscAnimalism = new V5Stat("animalism", "Animalism",
        desc: "Communion with the Beast within - yours, and others'.");
    public static V5Stat DiscAuspex = new V5Stat("auspex", "Auspex");
    public static V5Stat DiscCelerity = new V5Stat("celerity", "Celerity");
    public static V5Stat DiscDominate = new V5Stat("dominate", "Dominate");
    public static V5Stat DiscFortitude = new V5Stat("fortitude", "Fortitude");
    public static V5Stat DiscObfuscate = new V5Stat("obfuscate", "Obfuscate");
    public static V5Stat DiscOblivion = new V5Stat("oblivion", "Oblivion");
    public static V5Stat DiscPotence = new V5Stat("potence", "Potenace");
    public static V5Stat DiscPresence = new V5Stat("presence", "Presence");
    public static V5Stat DiscProtean = new V5Stat("protean", "Protean");
    public static V5Stat DiscSorcery = new V5Stat("sorcery", "Blood Sorcery");
    public static V5Stat DiscAlchemy = new V5Stat("alchemy", "Thin-Blood Alchemy");

    public readonly static V5Stat[] Disciplines = {
        DiscAnimalism, DiscAuspex, DiscCelerity, DiscDominate, DiscFortitude, DiscObfuscate,
        DiscOblivion, DiscPotence, DiscPresence, DiscProtean, DiscSorcery, DiscAlchemy
    };
    public readonly static System.Collections.Generic.Dictionary<string, V5Stat> DisciplinesDict =
        new System.Collections.Generic.Dictionary<string, V5Stat>();

    public static V5Stat TraitLooks = new V5Stat("looks", "Looks");

    public readonly static V5Stat[] Backgrounds = { TraitLooks }; // TODO: Look (hehe) into this.
    public readonly static System.Collections.Generic.Dictionary<string, V5Stat> BackgroundsDict =
        new System.Collections.Generic.Dictionary<string, V5Stat>();

    public static List<V5Stat> AllStats = new List<V5Stat>(
        Cfg.Attrs.Concat(Cfg.Skills).Concat(Cfg.Disciplines).Concat(Cfg.Backgrounds)
    );
}

public class V5StatBlock
{
    protected Godot.Collections.Dictionary<string, int> Bases;
    protected Godot.Collections.Dictionary<string, int> XpCounts;

    public static int NumBlocksMade { get; private set; } = 0;
    public string SbId { get; init; }

    public V5StatBlock()
    {
        SbId = $"StatBlock-type-1-Id-{++NumBlocksMade}";
        Bases = new Godot.Collections.Dictionary<string, int>();
        XpCounts = new Godot.Collections.Dictionary<string, int>();
    }

    public V5StatBlock(Godot.Collections.Dictionary<string, int> bases)
    {
        // GD.Print($"V5StatBlock Constructor - 2 - called, with bases dict:\n{bases}");
        SbId = $"StatBlock-type-2-Id-{++NumBlocksMade}";
        Bases = bases;
        XpCounts = new Godot.Collections.Dictionary<string, int>();
        foreach ((string statName, int baseVal) in bases)
        {
            XpCounts[statName] = 0;
        }
    }

    public V5StatBlock(
        Godot.Collections.Dictionary<string, int> bases,
        Godot.Collections.Dictionary<string, int> xpCounts
    )
    {
        SbId = $"StatBlock-type-3-{++NumBlocksMade}";
        Bases = bases; XpCounts = xpCounts;
    }

    public static V5StatBlock GetBlankBlock()
    {
        Godot.Collections.Dictionary<string, int> bs = new();
        Godot.Collections.Dictionary<string, int> xpc = new();
        V5Stat[] allODemStats = Cfg.AllStats.ToArray();
        foreach (V5Stat stat in allODemStats)
        {
            bs[stat.Id] = stat.Min; xpc[stat.Id] = 0;
            // GD.Print($"BlankBlock setup: bs[{stat.Id}] = {bs[stat.Id]}, xpc[{stat.Id}] = {xpc[stat.Id]}");
        }
        return new V5StatBlock(bs, xpc);
    }

    public static int CalculateStatWithXpBonus(int baseStat, int xpInvested, int statMultiplier, int max = 5)
    {
        int nextDotVal = baseStat + 1;
        int xpBonus = 0;
        while (nextDotVal <= max && xpInvested >= statMultiplier * nextDotVal)
        {
            xpBonus++;
            xpInvested -= statMultiplier * nextDotVal;
            nextDotVal++;
        }
        return baseStat + xpBonus;
    }

    public static bool IsStatMatch(V5Stat stat, string query)
    {
        string queryLc = query.ToLower();
        if (stat.Id.ToLower() == queryLc || stat.Name.ToLower() == queryLc)
        {
            return true;
        }
        return false;
    }

    public Tuple<int?, int?> GetStatPair(string statId)
    {
        Tuple<int?, int?> retval = new(
            Bases.ContainsKey(statId) ? Bases[statId] : null,
            XpCounts.ContainsKey(statId) ? XpCounts[statId] : null
        );
        return retval;
    }

    public void SetBaseStat(string statId, int value) { Bases[statId] = value; }
    public void SetXpCount(string statId, int value) { XpCounts[statId] = value; }
    public void InvestXp(string statId, int xpInvestiture) { XpCounts[statId] += xpInvestiture; }

    public void Reset()
    {
        foreach (V5Stat stat in Cfg.AllStats)
        {
            this.SetBaseStat(stat.Id, stat.Min);
            this.SetXpCount(stat.Id, 0);
        }
    }

    // Meant to be used with "StatPair" Tuples and as a standalone calculation helper func.
    public static int CalculateStat(int? baseVal, int? xpCount, int xpMult = Cfg.ATTR_MULT)
    {
        if (baseVal is not int || xpCount is not int)
        {
            throw new ArgumentNullException(
                $"CalculateStat: Base value ({baseVal}) and XP count ({xpCount}) must be integers!"
            );
        }
        return V5StatBlock.CalculateStatWithXpBonus((int)baseVal, (int)xpCount, xpMult, Cfg.DOT_MAX);
    }

    public int GetAttr(string statId)
    {
        Tuple<int?, int?> statPair = this.GetStatPair(statId);
        return V5StatBlock.CalculateStat(statPair.Item1, statPair.Item2, Cfg.ATTR_MULT);
    }
    public int GetSkill(string statId)
    {
        Tuple<int?, int?> statPair = this.GetStatPair(statId);
        return V5StatBlock.CalculateStat(statPair.Item1, statPair.Item2, Cfg.SKILL_MULT);
    }
    public int GetDisc(string statId)
    {
        // TODO: Check if discipline is in-clan or not.
        Tuple<int?, int?> statPair = this.GetStatPair(statId);
        return V5StatBlock.CalculateStat(statPair.Item1, statPair.Item2, Cfg.DISC_MULT_IN_CLAN);
    }
    public int GetBackground(string statId)
    {
        Tuple<int?, int?> statPair = this.GetStatPair(statId);
        return V5StatBlock.CalculateStat(statPair.Item1, statPair.Item2, Cfg.BG_MULT);
    }

    public static Tuple<V5Stat[], V5Stat[]> GetStatGrouping(string statField)
    {
        var attrsCheck = Cfg.Attrs.Where((stat) => IsStatMatch(stat, statField)).ToArray();
        if (attrsCheck.Length > 0)
        {
            return new(Cfg.Attrs, attrsCheck);
        }
        var skillsCheck = Cfg.Skills.Where((stat) => IsStatMatch(stat, statField)).ToArray();
        if (skillsCheck.Length > 0)
        {
            return new(Cfg.Skills, skillsCheck);
        }
        var discsCheck = Cfg.Disciplines.Where((stat) => IsStatMatch(stat, statField)).ToArray();
        if (discsCheck.Length > 0)
        {
            return new(Cfg.Disciplines, discsCheck);
        }
        var bgCheck = Cfg.Backgrounds.Where((stat) => IsStatMatch(stat, statField)).ToArray();
        if (bgCheck.Length > 0)
        {
            return new(Cfg.Backgrounds, bgCheck);
        }
        throw new MissingMemberException($"No stat with id/name \"{statField}\" could be found!");
    }

    public int GetStat(string statField)
    {
        var statGrouping = GetStatGrouping(statField);
        V5Stat[] category = statGrouping.Item1, searchResults = statGrouping.Item2;
        if (category == Cfg.Attrs)
        {
            return GetAttr(searchResults[0].Id);
        }
        else if (category == Cfg.Skills)
        {
            return GetSkill(searchResults[0].Id);
        }
        else if (category == Cfg.Disciplines)
        {
            return GetDisc(searchResults[0].Id);
        }
        else if (category == Cfg.Backgrounds)
        {
            return GetBackground(searchResults[0].Id);
        }
        throw new Exception("GetStatCategory() returned something unexpected.");
    }

    public int GetStatOrZero(string statField)
    {
        try
        {
            int statValue = GetStat(statField);
            return statValue;
        }
        catch (ArgumentNullException anex)
        {
            GD.PrintErr($"GetStatDefaultZero(): '{statField}' not found; returning zero: {anex})");
            return 0;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"GetStatDefaultZero(): Returning zero due to exception: {ex})");
            return 0;
        }
    }

    public static int RankUpXpRequired(V5Stat stat, V5Entity buyer)
    {
        return RankUpXpRequired(stat.Id, buyer);
    }

    public static int RankUpXpRequired(string statId, V5Entity buyer)
    {
        var statGrouping = GetStatGrouping(statId);
        V5Stat[] category = statGrouping.Item1; // searchResults = statGrouping.Item2;
        int nextRank = buyer.GetStatOrZero(statId), mult;
        if (category == Cfg.Attrs) { mult = Cfg.ATTR_MULT; }
        else if (category == Cfg.Skills) { mult = Cfg.SKILL_MULT; }
        else if (category == Cfg.Disciplines)
        {
            // TODO: Update this to check in-clan, etc.
            mult = Cfg.DISC_MULT_IN_CLAN;
        }
        else if (category == Cfg.Backgrounds) { mult = Cfg.BG_MULT; }
        else { mult = 3; }
        return nextRank * mult;
    }

    public override string ToString()
    {
        string toStr = $"\n-- StatBlock ({SbId}) --\n";
        foreach (V5Stat stat in Cfg.Attrs)
        {
            toStr += $"{stat.Id}: {GetStatOrZero(stat.Id)},  ";
        }
        toStr += "\n";
        foreach (V5Stat stat in Cfg.Skills)
        {
            toStr += $"{stat.Id}: {GetStatOrZero(stat.Id)},  ";
        }
        toStr += "\n";
        foreach (V5Stat stat in Cfg.Disciplines)
        {
            toStr += $"{stat.Id}: {GetStatOrZero(stat.Id)},  ";
        }
        toStr += $"\n-- end ^StatBlock ({SbId}) --";
        return toStr;
    }
}

public partial class V5Power
{
    public enum POWER_TYPE
    {
        PASSIVE,
        FREE_TOGGLE,
        ACTIVATED,
        SINGLE_USE
        // TODO: Finish these
    }

    public enum POWER_PURCHASE_REQS
    {
        RANK,
        XP,
        AMALG_RANK,
        POWER_PREREQ,
        OTHER_DISQUALIFIER
    }

    public static int InstanceCount { get; private set; } = 0;
    public string StatId { get; init; }
    public int Rank { get; init; }
    public string Id { get; init; }
    public string Name { get; init; }
    public POWER_TYPE PowerType { get; init; }
    public int Cost { get; init; }

    public V5Power(
        string powerName, V5Stat stat, int rank, int cost,
        POWER_TYPE powerType = POWER_TYPE.ACTIVATED
    )
    {
        StatId = stat.Id;
        Id = $"v5Power-{stat.Name}-{++InstanceCount}"; Name = powerName;
        PowerType = powerType; Rank = rank; Cost = cost;
    }
}

// Cfg powers list
public partial class Cfg
{
    public static partial class Pwrs
    {
        // Animalism
        public static readonly V5Power AnimFamulus = new("Bond Famulus", DiscAnimalism, 1, 4);
        public static readonly V5Power AnimSenseBeast = new("Sense the Beast", DiscAnimalism, 1, 1);
        public static readonly V5Power AnimSpeak = new("Feral Whispers", DiscAnimalism, 2, 4);
        public static readonly V5Power AnimCarrierPigeon
            = new("Animal Messenger", DiscAnimalism, 2, 4);
        public static readonly V5Power AnimSucculence = new("Animal Succulence", DiscAnimalism, 3, 0);
        public static readonly V5Power AnimSonOfSam = new("Messenger's Command", DiscAnimalism, 3, 4);
        public static readonly V5Power AnimQuell = new("Quell the Beast", DiscAnimalism, 3, 4);
        public static readonly V5Power AnimHive = new("Unliving Hive", DiscAnimalism, 3, 0);
        // Auspex
        public static readonly V5Power AuspDaredevil = new("Heightened Senses", DiscAuspex, 1, 1);
        public static readonly V5Power AuspEsp = new("Sense the Unseen", DiscAuspex, 1, 1);
        // Celerity
        public static readonly V5Power CeleGrace = new("Cat's Grace", DiscCelerity, 1, 0);
        public static readonly V5Power CeleTwitch = new("Rapid Reflexes", DiscCelerity, 1, 0);
        public static readonly V5Power CeleFleet = new("Fleetness", DiscCelerity, 2, 4);
        public static readonly V5Power CeleBlink = new("Blink", DiscCelerity, 3, 4);
        public static readonly V5Power CeleMatrixDodge = new("Weaving", DiscCelerity, 4, 4);
        // Dominate
        public static readonly V5Power DomiForget = new("Cloud Memory", DiscDominate, 1, 0);
        public static readonly V5Power DomiCompel = new("Compel", DiscDominate, 1, 1);
        public static readonly V5Power DomiDevotion = new("Slavish Devotion", DiscDominate, 1, 0);
        public static readonly V5Power DomiMesmerize = new("Mesmerize", DiscDominate, 2, 4);
        public static readonly V5Power DomiGaslight = new("Compel", DiscDominate, 3, 4);
        // Fortitude
        public static readonly V5Power FortHpBonus = new("Resilience", DiscFortitude, 1, 0);
        public static readonly V5Power FortWill = new("Unswayable Mind", DiscFortitude, 1, 0);
        public static readonly V5Power FortTough = new("Toughness", DiscFortitude, 2, 4);
        public static readonly V5Power FortBane = new("Defy Bane", DiscFortitude, 3, 4);
        // Obfuscate
        public static readonly V5Power ObfuFade = new("Cloak of Shadows", DiscObfuscate, 1, 1);
        public static readonly V5Power ObfuSilence = new("Silence of Death", DiscObfuscate, 1, 1);
        public static readonly V5Power ObfuStealth = new("Unseen Passage", DiscObfuscate, 2, 4);
        public static readonly V5Power ObfuTrick = new("Chimerstry", DiscObfuscate, 2, 4);
        public static readonly V5Power ObfuMask
            = new("Mask of a Thousand Faces", DiscObfuscate, 3, 4);
        public static readonly V5Power ObfuLaughingMan
            = new("Ghost in the Machine", DiscObfuscate, 3, 0);
        public static readonly V5Power ObfuHallucination = new("Fata Morgana", DiscObfuscate, 3, 4);
        public static readonly V5Power ObfuVanish = new("Vanish", DiscObfuscate, 4, 0);
        // Oblivion
        public static readonly V5Power ObliShadowCloak = new("Shadow Raiment", DiscOblivion, 1, 0);
        // Potence
        public static readonly V5Power PoteFatality = new("Vanish", DiscPotence, 1, 0);
        public static readonly V5Power PoteSuperjump = new("Soarling Leap", DiscPotence, 1, 0);
        public static readonly V5Power PoteProwess = new("Prowess", DiscPotence, 2, 4);
        public static readonly V5Power PoteMegasuck = new("Brutal Feed", DiscPotence, 3, 0);
        public static readonly V5Power PoteRage = new("Spark of Rage", DiscPotence, 3, 4);
        // Presence
        public static readonly V5Power PresAwe = new("Awe", DiscPresence, 1, 1);
        public static readonly V5Power PresDaunt = new("Daunt", DiscPresence, 1, 1);
        public static readonly V5Power PresAddicted2U = new("Lingering Kiss", DiscPresence, 2, 0);
        public static readonly V5Power PresEntrance = new("Entracement", DiscPresence, 3, 4);
        public static readonly V5Power PresScaryface = new("Dread Gaze", DiscPresence, 3, 4);
        // Protean
        public static readonly V5Power ProtRedEye = new("Eyes of the Beast", DiscProtean, 1, 1);
        public static readonly V5Power ProtFeather = new("Weight of the Feather", DiscProtean, 1, 1);
        public static readonly V5Power ProtToothNClaw = new("Feral Weapons", DiscProtean, 2, 4);
        public static readonly V5Power ProtMoldSelf = new("Vicissitude", DiscProtean, 2, 4);
        public static readonly V5Power ProtDirtNap = new("Earthmeld", DiscProtean, 3, 4);
        public static readonly V5Power ProtMoldOthers = new("Fleshcrafting", DiscProtean, 3, 4);
        public static readonly V5Power ProtBooBleh = new("Shapechagne", DiscProtean, 3, 4);
        public static readonly V5Power ProtDruid = new("Metamorphosis", DiscProtean, 4, 4);
        public static readonly V5Power ProtFinalForm = new("Horride Form", DiscProtean, 4, 4);
        // Blood Sorcery
        public static readonly V5Power SorcAcidBlood = new("Corrosive Vitae", DiscSorcery, 1, 4); // +
        public static readonly V5Power SorcBloodyPresti
            = new("Shape the Sanguine Sacrament", DiscSorcery, 1, 0); // +
        // Thin-Blood Alchemy
        public static readonly V5Power AlchTelekinesis = new("Far Reach", DiscAlchemy, 1, 4);
        public static readonly V5Power AlchCounterfeitL1
            = new("Counterfeit Discipline (Level 1)", DiscAlchemy, 2, 0); // Same as power faked
        public static readonly V5Power AlchCounterfeitL2
            = new("Counterfeit Discipline (Level 2)", DiscAlchemy, 3, 0);
        public static readonly V5Power AlchNewYou
            = new("Profane Hieros Gamos", DiscAlchemy, 3, 0); // 1 Rouse (vamps), 1 Agg (humans)
    }

    // --- Cfg power trees
    public static readonly Dictionary<string, List<List<V5Power>>> DiscPowerTrees = new()
    {
        [Cfg.DiscAnimalism.Id] = new()
        {
            new(){Pwrs.AnimFamulus, Pwrs.AnimSenseBeast},
            new(){Pwrs.AnimSpeak, Pwrs.AnimCarrierPigeon},
            new(){Pwrs.AnimSucculence, Pwrs.AnimSonOfSam, Pwrs.AnimQuell, Pwrs.AnimHive}
        },
        [Cfg.DiscAuspex.Id] = new()
        {
            new(){Pwrs.AuspDaredevil, Pwrs.AuspEsp}
        },
        [Cfg.DiscCelerity.Id] = new()
        {
            new(){Pwrs.CeleGrace, Pwrs.CeleTwitch},
            new(){Pwrs.CeleFleet},
            new(){Pwrs.CeleBlink},
            new(){Pwrs.CeleMatrixDodge}
        },
        [Cfg.DiscDominate.Id] = new()
        {
            new(){Pwrs.DomiForget, Pwrs.DomiCompel, Pwrs.DomiDevotion},
            new(){Pwrs.DomiMesmerize},
            new(){Pwrs.DomiGaslight}
        },
        [Cfg.DiscFortitude.Id] = new()
        {
            new(){Pwrs.FortHpBonus},
            new(){Pwrs.FortTough},
            new(){Pwrs.FortBane}
        },
        [Cfg.DiscObfuscate.Id] = new()
        {
            new(){Pwrs.ObfuFade, Pwrs.ObfuSilence},
            new(){Pwrs.ObfuTrick, Pwrs.ObfuStealth},
            new(){Pwrs.ObfuHallucination, Pwrs.ObfuLaughingMan, Pwrs.ObfuMask},
            new(){Pwrs.ObfuVanish}
        },
        [Cfg.DiscOblivion.Id] = new()
        {
            new(){Pwrs.ObliShadowCloak}
        },
        [Cfg.DiscPotence.Id] = new()
        {
            new(){Pwrs.PoteFatality, Pwrs.PoteSuperjump},
            new(){Pwrs.PoteProwess},
            new(){Pwrs.PoteMegasuck, Pwrs.PoteRage}
        },
        [Cfg.DiscPresence.Id] = new()
        {
            new(){Pwrs.PresAwe, Pwrs.PresDaunt},
            new(){Pwrs.PresAddicted2U},
            new(){Pwrs.PresEntrance, Pwrs.PresScaryface}
        },
        [Cfg.DiscProtean.Id] = new()
        {
            new(){Pwrs.ProtFeather, Pwrs.ProtRedEye},
            new(){Pwrs.ProtToothNClaw, Pwrs.ProtMoldSelf},
            new(){Pwrs.ProtDirtNap, Pwrs.ProtMoldOthers, Pwrs.ProtBooBleh},
            new(){Pwrs.ProtFinalForm, Pwrs.ProtDruid}
        },
        [Cfg.DiscSorcery.Id] = new()
        {
            new(){Pwrs.SorcAcidBlood, Pwrs.SorcBloodyPresti}
        },
        [Cfg.DiscAlchemy.Id] = new()
        {
            new(){Pwrs.AlchTelekinesis},
            new(){Pwrs.AlchCounterfeitL1},
            new(){Pwrs.AlchCounterfeitL2, Pwrs.AlchNewYou}
        }
    };

    public static readonly Dictionary<string, Tuple<V5Stat, int>> AmalgamReqs = new()
    {
        [Pwrs.AnimCarrierPigeon.Id] = new(DiscAuspex, 1),
        [Pwrs.AnimSonOfSam.Id] = new(DiscDominate, 1),
        [Pwrs.AnimHive.Id] = new(DiscObfuscate, 2),
        [Pwrs.DomiDevotion.Id] = new(DiscFortitude, 1), // Errata; was Presence 1
        [Pwrs.ObfuTrick.Id] = new(DiscPresence, 1),
        [Pwrs.ObfuHallucination.Id] = new(DiscPresence, 2),
        [Pwrs.PoteRage.Id] = new(DiscPresence, 3),
        [Pwrs.ProtMoldSelf.Id] = new(DiscDominate, 2),
        [Pwrs.ProtMoldOthers.Id] = new(DiscDominate, 2)
        //[Pwrs.ProtFinalForm.Id] = new(DiscDominate.Id, 2)
    };

    // Dictionary, string -> List of Lists. This is done to accommodate weird prereq requirements
    // like those of Messenger's Command (Animal Messenger AND (Compel OR Mesmerize)).
    // Hopefull it doesn't get any more convoluted than that.
    public static readonly Dictionary<string, List<List<V5Power>>> PowerPrereqs = new()
    {
        // And, then or; i.e. top level are all required, any item w/in top level list counts
        [Pwrs.AnimSonOfSam.Id] = new()
        {
            new(){Pwrs.AnimCarrierPigeon},
            new(){Pwrs.DomiCompel, Pwrs.DomiMesmerize}
        },
        [Pwrs.CeleMatrixDodge.Id] = new() { new() { Pwrs.CeleTwitch } },
        [Pwrs.ObfuVanish.Id] = new() { new() { Pwrs.ObfuFade } },
        [Pwrs.ProtMoldOthers.Id] = new() { new() { Pwrs.ProtMoldSelf } },
        [Pwrs.ProtFinalForm.Id] = new() { new() { Pwrs.ProtMoldSelf } },
        [Pwrs.ProtDruid.Id] = new() { new() { Pwrs.ProtBooBleh } },
    };
}

public partial class V5Power
{
    public V5Stat AmalgStat
    {
        get => Cfg.AmalgamReqs.ContainsKey(Id) ? Cfg.AmalgamReqs[Id].Item1 : null;
    }
    public int AmalgStatReq
    {
        get => Cfg.AmalgamReqs.ContainsKey(Id) ? Cfg.AmalgamReqs[Id].Item2 : 0;
    }
    public List<List<V5Power>> PowerPrereqs
    {
        get => Cfg.PowerPrereqs.ContainsKey(Id) ? Cfg.PowerPrereqs[Id] : new();
    }

    public static Tuple<bool, List<POWER_PURCHASE_REQS>> CanBePurchased(V5Power power, V5Entity buyer, bool assumePreRankUp = true)
    {
        List<POWER_PURCHASE_REQS> disqualifiers = new();
        int buyerStatRank = buyer.GetStatOrZero(power.StatId);
        if (buyerStatRank < power.Rank - 1 || (!assumePreRankUp && buyerStatRank < power.Rank))
        {
            disqualifiers.Add(POWER_PURCHASE_REQS.RANK);
        }
        if (buyer.UnspentXp < V5StatBlock.RankUpXpRequired(power.StatId, buyer))
        {
            disqualifiers.Add(POWER_PURCHASE_REQS.XP);
        }
        if (power.AmalgStat is not null)
        {
            int buyerAmalgStatRank = buyer.GetStatOrZero(power.AmalgStat.Id);
            if (buyerAmalgStatRank < power.AmalgStatReq)
            {
                disqualifiers.Add(POWER_PURCHASE_REQS.AMALG_RANK);
            }
        }
        foreach (List<V5Power> prereqPowerOptSet in power.PowerPrereqs)
        {
            bool hasAnyPower = false;
            foreach (V5Power powerOpt in prereqPowerOptSet)
            {
                if (buyer.HasPower(powerOpt))
                {
                    hasAnyPower = true;
                }
            }
            if (!hasAnyPower)
            {
                disqualifiers.Add(POWER_PURCHASE_REQS.POWER_PREREQ);
            }
        }
        // Other disqualifiers, power specific perhaps?
        return new(disqualifiers.Count > 0, disqualifiers);
    }
}