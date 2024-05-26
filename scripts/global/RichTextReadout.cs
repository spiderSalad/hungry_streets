using Godot;
using System;

public partial class RichTextReadout : RichTextLabel
{
    [Export] protected Godot.Collections.Array<Cfg.GROUP_NAMES> NodeGroups;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Utils.AddNodeToGroups(this, NodeGroups);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateText(string newText)
	{
		this.Text = newText;
	}
}
