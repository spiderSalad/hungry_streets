using Godot;
using System;
using System.Reflection;

public partial class btnCcuiFinishExit : Button
{
    [Signal]
    public delegate void FormSubmitEventHandler(string source);
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddToGroup($"{Cfg.GROUP_NAMES.CCUI_FORM_INPUTS}");
        Pressed += ExitCharCreation;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void ExitCharCreation()
    {
        this.EmitSignal(SignalName.FormSubmit, (int)Cfg.UI_KEY.SWITCH_SCENE);
    }
}
