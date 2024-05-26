using Godot;
using System;

public partial class optBtnCharPronouns : OptionButton
{
    [Signal]
    public delegate void FormSelectionEventHandler(string source, long selectionIndex);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddToGroup($"{Cfg.GROUP_NAMES.CCUI_FORM_INPUTS}");
        this.ItemSelected += OnItemSelected;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnItemSelected(long index)
    {
        this.EmitSignal(SignalName.FormSelection, (int)Cfg.UI_KEY.PRONOUN_SELECT, index);
    }
}
