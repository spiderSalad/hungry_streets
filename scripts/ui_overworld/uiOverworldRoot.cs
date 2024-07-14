using Godot;

public partial class uiOverworldRoot : Control
{
	private GameManager gm;
	private Godot.Collections.Array<Node> OwuiControls;
	private Godot.Collections.Array<Node> OwuiDisplays;
	private AudioHandler audioplayer;

	[Signal] public delegate void UiButtonPressEventHandler(Cfg.UI_KEY key, string opt);
	[Signal] public delegate void PcUpdateEventHandler(Cfg.UI_KEY source, PlayerChar pc);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>(Cfg.NODEPATH_ABS_GAMEMANAGER);
		Godot.Callable clbk;

		OwuiControls = GetTree().GetNodesInGroup($"{Cfg.GROUP_NAMES.OWUI_CONTROLS}");
		foreach (Node node in OwuiControls)
		{
			if (node is Button btn)
			{
				clbk = Callable.From((Cfg.UI_KEY com, string opt) => this.OnButtonPress(com, opt));
				btn.Connect(SignalName.UiButtonPress, clbk);
			}
		}

		OwuiDisplays = GetTree().GetNodesInGroup($"{Cfg.GROUP_NAMES.OWUI_DISPLAYS}");
		if (gm.ThePc is not null)
		{
			GetTree().CallGroup(
				$"{Cfg.GROUP_NAMES.OWUI_DISPLAYS}", "CharCreationUpdate", gm.ThePc
			);
		}
		else
		{
			GD.PrintErr($"gm.playerchar ({typeof(PlayerChar)}) is unset/null.");
		}

		clbk = Godot.Callable.From((Cfg.UI_KEY src, PlayerChar pc) => this.OnPcUpdate(src, pc));
		gm.Connect(SignalName.PcUpdate, clbk);

		audioplayer = GetNode<AudioHandler>("/root/AudioHandler");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPcUpdate(Cfg.UI_KEY source, PlayerChar pc)
	{
		GD.Print($"Owui.OnPcUpdate(): Received source = {source}, Pc = {pc}");
		GetTree().CallGroup(
			$"{Cfg.GROUP_NAMES.OWUI_DISPLAYS}", "CharCreationUpdate", gm.ThePc
		);
	}

	public void OnButtonPress(Cfg.UI_KEY key, string opt) // change this moethod name
	{
		switch (key)
		{
			case Cfg.UI_KEY.SWITCH_SCENE:
				gm.GoToNamedScene(opt);
				break;
			default:
				GD.Print($"Unrecognized button press; source = {key}, opt = {opt}");
				break;
		}
		audioplayer.PlaySound(audioplayer.soundConfirm1);
	}
}
