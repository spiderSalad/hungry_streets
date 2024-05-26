using Godot;

public partial class lblRollResults1 : Label
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddToGroup($"{Cfg.GROUP_NAMES.DPUI_DISPLAYS}");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void ShowDiceTestResults(string rollToString)
    {
        Text = rollToString;
    }
}
