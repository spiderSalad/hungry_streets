using Godot;

[GlobalClass]
public partial class V5EntBundle : Resource
{
    [Export] public string Name { get; set; }
    [Export] public string PronounSetId { get; set; } = Cfg.Person.Id;
    [Export] public string BackgroundName { get; set; } = "None";
    [Export] public Godot.Collections.Dictionary<string, int> StatVals { get; set; }
    [Export] public Godot.Collections.Dictionary<string, int> XpVals { get; set; }
    [Export] public int HpDmgSpf { get; set; }
    [Export] public int HpDmgAgg { get; set; }
    [Export] public int WillDmgSpf { get; set; }
    [Export] public int WillDmgAgg { get; set; }
    [Export] public int Hunger { get; set; }
    // Powers, Buffs

    [Export] public bool Sheltered { get; set; }
    [Export] public bool Traveling { get; set; }
    [Export] public bool InPublic { get; set; }

    [Export] public int Cash { get; set; }
    // TODO: Whole-ass Inventory

    [Export] public int NumDiceTests { get; set; }
}
