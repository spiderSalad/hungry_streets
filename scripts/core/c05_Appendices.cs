using Godot;
using System;
using System.Collections.Generic;

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

public class MapLoc
{
    public string LocId { get; init; }
    public string LocName { get; set; }
    public string ShortDesc
    {
        get
        {
            string desc = LocName.ToLower().StartsWith("the ") ? $"the {LocName[4..]}" : LocName;
            return IsHaven ? $"your haven at {desc}" : desc;
        }
    }

    // Proportional position (between 0 and 1) relative to the background art the BtnMapLoc
    // Control nodes are placed against. UI map position.
    public Vector2 MapPosition { get; set; }
    // Theoretical position (between 0 and 1) relative to a representative top-down map,
    // used to calculate travel "distances". Do not need to be accurate.
    public Vector2 TruePosition { get; set; }

    public bool IsHaven { get; set; } = false;
    public bool OverridePosition { get; set; } = false;

    public MapLoc(string locName)
    {
        LocName = locName; LocId = LocName.ToLower().Replace(' ', '_');
        MapPosition = new(0.1f, 0.1f);
        TruePosition = new(0.1f, 0.1f);
    }

    public MapLoc(string locName, float x, float y)
    {
        LocName = locName; LocId = LocName.ToLower().Replace(' ', '_');
        if (x > 1 || x < 0 || y > 1 || y < 0)
        {
            throw new ArgumentOutOfRangeException(
                "x or y",
                $"MapLoc coordinates ({x}, {y}) represent percentages, and must be between 0 and 1."
            );
        }
        MapPosition = new(x, y);
        TruePosition = new(x, y);
    }

    public static int GetTravelTimeMinutes(float distance, Cfg.TRAVEL_OPTIONS travelMode)
    {
        var travelSpeed = travelMode switch
        {
            Cfg.TRAVEL_OPTIONS.RIDESHARE or
            Cfg.TRAVEL_OPTIONS.RIDE_DOMINATE or
            Cfg.TRAVEL_OPTIONS.RIDE_PRESENCE => 2.1f,
            Cfg.TRAVEL_OPTIONS.CELERITY => 1.7f,
            Cfg.TRAVEL_OPTIONS.SORCERY => 3.2f,
            _ => 1,
        };
        return (int)Math.Round(distance * Cfg.MINUTES_PER_TRAVEL_DIST_OVERWORLD / travelSpeed);
    }

    public override string ToString() { return $"{LocName}"; }
}

public partial class Cfg
{
    public readonly static System.Collections.Generic.Dictionary<string, MapLoc> LocationsDict =
        new()
        {
            ["old_cellar"] = new("Old Cellar", 0.2f, 0.4f)
            {
                IsHaven = true,
                LocName = "the Old Cellar",
                TruePosition = new(0.1f, 0.8f)
            },
            ["docks"] = new("Docks", 0.75f, 0.45f)
            {
                LocName = "the Docks",
                TruePosition = new(0.14f, 0.83f)
            },
            ["elysium"] = new("Elysium", 0.23f, 0.22f)
            {
                TruePosition = new(0.19f, 0.55f)
            },
            ["dockside_hood"] = new("Dockside Hood", 0.37f, 0.51f)
            {
                LocName = "the Dockside Neighborhood",
                TruePosition = new(0.11f, 0.82f)
            },
            ["downtown"] = new("Downtown", 0.45f, 0.18f)
            {
                TruePosition = new(0.7f, 0.32f)
            }
        };

    public readonly static List<MapLoc> StartingUnlockedLocs = new()
    {
        LocationsDict["old_cellar"],
        LocationsDict["docks"],
        // LocationsDict["dockside"],
        // NOTE: The above bad reference caused not just error but a silent failure, why?
        LocationsDict["dockside_hood"],
        LocationsDict["downtown"]
    };

    public enum TRAVEL_OPTIONS
    {
        CANCEL,
        WALK,
        RIDESHARE,
        RIDE_PRESENCE,
        RIDE_DOMINATE,
        CELERITY,
        SORCERY
    }

    // How many minutes would it take to walk from one end of the city to the other, roughly?
    public const float MINUTES_PER_TRAVEL_DIST_OVERWORLD = 360.0f;
    public const int RIDE_BASE_COST = 30;
}