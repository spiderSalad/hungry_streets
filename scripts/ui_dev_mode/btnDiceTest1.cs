using Godot;
using System;

public partial class btnDiceTest1 : Button
{
    [Signal]
    public delegate void FormSubmitEventHandler(string source);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddToGroup($"{Cfg.GROUP_NAMES.DPUI_INPUTS}");
        Pressed += TriggerDiceTest1;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void TriggerDiceTest1()
    {
        this.EmitSignal(SignalName.FormSubmit, (int)Cfg.UI_KEY.DICE_TEST_1);
    }
}
