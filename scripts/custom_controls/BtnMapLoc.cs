using Godot;

public partial class BtnMapLoc : TextureButton
{
	[Signal] public delegate void MapLocHoverEventHandler(string locLink);
	[Signal] public delegate void MapLocFocusEventHandler(string locLink);
	[Signal] public delegate void MapLocPressEventHandler(string locLink);

	private const string _btnMapLocTexturePath = $"{Cfg.PATH_GUI}wod_circle_300x300_";

	public string MapPinId { get; init; }

	[Export] public string LocId { get; private set; }

	public BtnMapLoc() : base()
	{
		MapPinId = $"map-pin-{Utils.GenerateRandomAlphanumeric(6)}";
	}

	public BtnMapLoc(string locId) : base()
	{
		MapPinId = $"map-pin-{Utils.GenerateRandomAlphanumeric(6)}";
		LocId = locId;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(" --> BtnMapLoc: _Ready()");
		IgnoreTextureSize = true;
		StretchMode = StretchModeEnum.KeepAspect;
		CustomMinimumSize = new(50, 50);

		TextureNormal = GetTexture("default");
		TexturePressed = GetTexture("pressed");
		TextureHover = GetTexture("hover");
		TextureDisabled = GetTexture("disabled");
		TextureFocused = GetTexture("focused");

		Pressed += OnPressed;
	}

	public void OnPressed()
	{
		EmitSignal(SignalName.MapLocPress, LocId);
	}

	protected static Texture2D GetTexture(string whichTexture)
	{
		return GD.Load<Texture2D>($"{_btnMapLocTexturePath}{whichTexture}.png");
	}
}
