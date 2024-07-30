using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class uiOverworldRoot : Control
{
	[ExportGroup("Map")]
	[Export] private Control _mapLocParentNode;
	[Export] private Label _mapSubheader;
	[Export] private PopupMenu _modalTravelMenu;

	[ExportGroup("")]

	private GameManager gm;
	private Godot.Collections.Array<Node> OwuiControls;
	private Godot.Collections.Array<Node> OwuiDisplays;
	private AudioHandler audioplayer;

	[Signal] public delegate void UiButtonPressEventHandler(Cfg.UI_KEY key, string opt);
	[Signal] public delegate void PcUpdateEventHandler(Cfg.UI_KEY source, PlayerChar pc);
	[Signal] public delegate void MapLocHoverEventHandler(string locLink);
	[Signal] public delegate void MapLocFocusEventHandler(string locLink);
	[Signal] public delegate void MapLocPressEventHandler(string locLink);

	public Godot.Collections.Dictionary MapPinBtnsDict { get; init; } = new();
	public MapLoc Destination { get; private set; }
	Godot.Callable mapLocPressClbk;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<GameManager>(Cfg.NODEPATH_ABS_GAMEMANAGER);
		Godot.Callable clbk;

		clbk = Godot.Callable.From((Cfg.UI_KEY src, PlayerChar pc) => this.OnPcUpdate(src, pc));
		gm.Connect(SignalName.PcUpdate, clbk);

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

		_mapLocParentNode.Resized += UpdateMapPinsOnResize;
		mapLocPressClbk = Godot.Callable.From((string loc) => OnMapLocPressed(loc));
		_modalTravelMenu.IdPressed += OnTravelOptionPicked;

		audioplayer = GetNode<AudioHandler>("/root/AudioHandler");
		audioplayer.LoadOverworldAudio();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }

	public void UpdateMapPinsOnResize()
	{
		GD.Print($"New size for _mapLocParentNode: {_mapLocParentNode.Size}");
		CallDeferred(MethodName.LoadMapLocations);
		CallDeferred(MethodName.ResetMapPinSignalConnections);
	}

	public void LoadMapLocations()
	{
		ResetMapPinButtons();
		foreach (string locId in gm.PresentState.UnlockedLocations.Select(v => (string)v))
		{
			MapLoc loc = Cfg.LocationsDict[locId]; BtnMapLoc bml;
			bool missing = !MapPinBtnsDict.ContainsKey(loc.LocId);
			if (missing || loc.OverridePosition)
			{
				if (missing)
				{
					bml = new BtnMapLoc(loc.LocId);
					MapPinBtnsDict.Add(loc.LocId, bml);
					_mapLocParentNode.AddChild(bml);
				}
				else
				{
					bml = (BtnMapLoc)MapPinBtnsDict[loc.LocId];
				}

				GD.Print(
					"Adding new map loc to " +
					$"{loc.MapPosition.X * _mapLocParentNode.Size.X}, {loc.MapPosition.Y * _mapLocParentNode.Size.Y}" +
					$"\n that's ({loc.MapPosition.X} * {_mapLocParentNode.Size.X}), ({loc.MapPosition.Y} x {_mapLocParentNode.Size.Y})"
				);
				bml.Position = new(
					loc.MapPosition.X * _mapLocParentNode.Size.X,
					loc.MapPosition.Y * _mapLocParentNode.Size.Y
				);
			}
		}
	}

	public void ResetMapPinSignalConnections()
	{
		ResetMapPinButtons();
		foreach (var entry in MapPinBtnsDict)
		{
			BtnMapLoc bml = (BtnMapLoc)entry.Value;
			if (!bml.IsConnected(SignalName.MapLocPress, mapLocPressClbk))
			{
				bml.Connect(SignalName.MapLocPress, mapLocPressClbk);
			}
		}
	}

	public void ResetMapPinButtons()
	{
		MapPinBtnsDict.Clear();
		foreach (Node node in _mapLocParentNode.GetChildren())
		{
			if (node is BtnMapLoc bml)
			{
				if (bml.LocId == default) { throw new ArgumentNullException("Missing LocId!"); }
				MapPinBtnsDict.Add(bml.LocId, bml);
			}
		}
	}

	public void OnMapLocPressed(string locLink)
	{
		MapLoc loc = Cfg.LocationsDict[locLink];
		Destination = loc;
		if (Destination.LocId != gm.ThePc.CurrentLocation.LocId)
		{
			_mapSubheader.Text = "Where do you want to go? " +
				$"{loc.ShortDesc[..1].Capitalize()}{loc.ShortDesc[1..]}?";
			BtnMapLoc bml = (BtnMapLoc)MapPinBtnsDict[loc.LocId];
			Vector2I newModalPos = (Vector2I)bml.Position + (Vector2I)new Vector2(bml.Size.X * 1.1f, 0);
			_modalTravelMenu.Position = newModalPos;
			_modalTravelMenu.Title = $"Travel to {(loc.IsHaven ? "your haven" : loc.ShortDesc)}?";
			// Checks if you have the cash for a rideshare, TODO: expand later.
			int rideIndex = _modalTravelMenu.GetItemIndex((int)Cfg.TRAVEL_OPTIONS.RIDESHARE);
			_modalTravelMenu.SetItemDisabled(rideIndex, gm.ThePc.Cash < Cfg.RIDE_BASE_COST);
			//
			_modalTravelMenu.Visible = true;
			audioplayer.PlaySound(audioplayer.SoundSelect1);
		}
		else
		{
			_mapSubheader.Text = $"Where do you want to go? You're already at {loc.ShortDesc}.";
			audioplayer.PlaySound(audioplayer.SoundCancel1);
		}
	}

	public void OnTravelOptionPicked(long id)
	{
		Cfg.TRAVEL_OPTIONS travelOpt = (Cfg.TRAVEL_OPTIONS)id;
		if (travelOpt != Cfg.TRAVEL_OPTIONS.CANCEL) { TravelToMapLoc(Destination, travelOpt); }
		else { audioplayer.PlaySound(audioplayer.SoundCancel1); }
	}

	public void TravelToMapLoc(MapLoc destination, Cfg.TRAVEL_OPTIONS travelMode)
	{
		// TODO: move some of this functionality over to events, when implemented
		gm.PcTravelEvent(destination, travelMode);
		switch (travelMode)
		{
			case Cfg.TRAVEL_OPTIONS.WALK:
				audioplayer.PlaySound(audioplayer.SoundTravelWalk); break;
			case Cfg.TRAVEL_OPTIONS.RIDESHARE:
			case Cfg.TRAVEL_OPTIONS.RIDE_DOMINATE:
			case Cfg.TRAVEL_OPTIONS.RIDE_PRESENCE:
				audioplayer.PlaySound(audioplayer.SoundTravelRide);
				break;
			case Cfg.TRAVEL_OPTIONS.CANCEL:
				audioplayer.PlaySound(audioplayer.SoundCancel1); break;
		}
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
		audioplayer.PlaySound(audioplayer.SoundConfirm1);
	}
}
