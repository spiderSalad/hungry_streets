using Godot;
using System;
using System.Collections.Generic;

public partial class optBtnChooseRoller : OptionButton
{
	[Signal]
    public delegate void FormSelectionEventHandler(Cfg.UI_KEY source, long selectionIndex);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		List<V5Entity> allEnts = V5Entity.GetAllEntities();
		for (int i = 0; i < allEnts.Count; i++)
		{
			this.AddItem(allEnts[i].ToString(), i);
		}
		ItemSelected += OnItemSelected;
	}

	public void OnItemSelected(long index)
	{
		this.EmitSignal(SignalName.FormSelection, (int)Cfg.UI_KEY.SELECT_ACTIVE_ENTITY_ALL, index);
	}
}
