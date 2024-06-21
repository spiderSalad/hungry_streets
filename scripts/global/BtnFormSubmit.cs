using Godot;
using System;

public partial class BtnFormSubmit : Button
{
	[Signal] public delegate void FormSubmitEventHandler(Cfg.UI_KEY source);

	[Export] protected Godot.Collections.Array<Cfg.GROUP_NAMES> NodeGroups;
	[Export] protected Cfg.UI_KEY formSubmitUiKey;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Utils.AddNodeToGroups(this, NodeGroups);
		Pressed += OnPressed;
	}

	public void OnPressed()
	{
		this.EmitSignal(SignalName.FormSubmit, (int)formSubmitUiKey);
	}
}
