using Godot;
using System;

public partial class btnBack2CcuiTest : Button
{
    [Signal]
    public delegate void UiButtonPressEventHandler(string sourc, string opt);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddToGroup($"{Cfg.GROUP_NAMES.OWUI_CONTROLS}");
        this.Pressed += BackToCcui;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void BackToCcui()
    {
        GD.Print("BackToCcui called from button press");
        this.EmitSignal(SignalName.UiButtonPress, (int)Cfg.UI_KEY.SWITCH_SCENE, Cfg.SCENE_CCUI);
    }
}
