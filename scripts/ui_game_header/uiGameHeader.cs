using Godot;
using System;

public partial class uiGameHeader : Control
{
	[Export] private Label _mainGameHeaderText;
	[Export] private Label _lblGameClock;

	private GameManager gm;

	[Signal] public delegate void ClockAdvancedEventHandler(int night, int minutesRemaining);
	[Signal] public delegate void ClockDaybreakEventHandler(int night);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>(Cfg.NODEPATH_ABS_GAMEMANAGER);
		UpdateClockDisplay();
		// MoveMeToEnd();
		gm.TheClock.Connect(
			SignalName.ClockAdvanced,
			Godot.Callable.From((int night, int minutes) => ShowClockChange(night, minutes))
		);
		gm.TheClock.Connect(
			SignalName.ClockDaybreak,
			Godot.Callable.From((int night) => ShowDaybreak(night))
		);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }

	public void ShowClockChange(int night, int minutes)
	{
		GD.Print($"uiGameHeader: ShowClockChange() called, night {night} and {minutes} minutes.");
		UpdateClockDisplay($"{gm.TheClock}");
	}

	public void ShowDaybreak(int night)
	{
		UpdateClockDisplay("THE SUN IS DEATH");
	}

	public void UpdateClockDisplay(string clockString = null)
	{
		if (clockString != null)
		{
			_lblGameClock.Text = clockString;
		}
		else
		{
			_lblGameClock.Text = gm.TheClock == null ? "Night?" : $"{gm.TheClock}";
		}
	}

	public void MoveMeToEnd()
	{
		this.CallDeferred(MethodName.DeferredMoveMeToEnd);
	}

	protected void DeferredMoveMeToEnd()
	{
		Node myParent = this.GetParent();
		myParent.MoveChild(this, myParent.GetChildCount() - 1);
	}
}
