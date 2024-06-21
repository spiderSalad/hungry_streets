using Godot;
using System;
using System.Collections.Generic;

public partial class optBtnChooseRoller : OptionButton
{
	[Export] bool activeRoller;

	[Signal] public delegate void FormSelectionEventHandler(Cfg.UI_KEY source, long index);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		string defaultEntListing = activeRoller ? "(default->PC->Dummy)" : "(default->Dummy)";
		List<V5Entity> allEnts = V5Entity.GetAllEntities();
		for (int i = 0; i < allEnts.Count; i++)
		{
			V5Entity ent = allEnts[i];
			this.AddItem(ent is null ? defaultEntListing : ent.ToString(), i);
		}
		ItemSelected += OnItemSelected;
	}

	public void OnItemSelected(long index)
	{
		Cfg.UI_KEY key = activeRoller ?
			Cfg.UI_KEY.SELECT_ACTIVE_ENTITY_ALL : Cfg.UI_KEY.SELECT_REACTING_ENTITY_ALL;
		this.EmitSignal(SignalName.FormSelection, (int)key, index);
	}
}
