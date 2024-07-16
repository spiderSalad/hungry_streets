using Godot;
using System;

public partial class TogglablePanel : Control
{
	[Export] private CheckButton _toggleSwitch;
	[Export] private Container _panel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_toggleSwitch.Toggled += TogglePanel;
		_panel.Visible = _toggleSwitch.ButtonPressed;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {}

	public void TogglePanel(bool toggledOn)
	{
		// _panel.Visible = !_panel.Visible;
		GD.Print($"\nTogglePanel: {toggledOn}");
		_panel.Visible = toggledOn;
	}
}
