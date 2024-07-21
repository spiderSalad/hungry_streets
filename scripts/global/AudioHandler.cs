using Godot;
using System;

public partial class Cfg
{
	public const string SOUND_UI_SCROLL_1 = PATH_SOUND + "727648__cjspellsfish__scroll-1.wav";
	public const string SOUND_UI_SELECT_1 = PATH_SOUND + "198449__cs279__menu-scroll-selection-sound.wav";
	public const string SOUND_UI_SELECT_2_CONFIRM = PATH_SOUND + "198448__cs279__menu-scroll-selection-sound.wav";
	public const string SOUND_UI_CANCEL_1 = PATH_SOUND + "672943__silverillusionist__cancel-retro-thin.wav";
	public const string SOUND_UI_DICEROLL_MANY = PATH_SOUND + "220744__dermotte__dice-06.wav";
	public const string SOUND_UI_DICEROLL_FEW = PATH_SOUND + "353975__nettimato__rolling-dice-1.wav";
	public const string SOUND_UI_TRAVEL_WALK = PATH_SOUND + "153055__xampi8__heels.wav";
	public const string SOUND_UI_TRAVEL_RIDE = PATH_SOUND + "447974__ggakatsukip__s0408_car-arrive.wav";
}

public partial class AudioHandler : AudioStreamPlayer
{
	public AudioStream SoundScroll1 { get; private set; }
	public AudioStream SoundSelect1 { get; private set; }
	public AudioStream SoundConfirm1 { get; private set; }
	public AudioStream SoundCancel1 { get; private set; }
	public AudioStream SoundDiceMany { get; private set; }
	public AudioStream SoundDiceFew { get; private set; }

	public AudioStream SoundTravelWalk { get; private set; }
	public AudioStream SoundTravelRide { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Load sounds used everywhere, as opposed to particular scenes.
		SoundScroll1 = GD.Load<AudioStream>(Cfg.SOUND_UI_SCROLL_1);
		SoundSelect1 = GD.Load<AudioStream>(Cfg.SOUND_UI_SELECT_1);
		SoundConfirm1 = GD.Load<AudioStream>(Cfg.SOUND_UI_SELECT_2_CONFIRM);
		SoundCancel1 = GD.Load<AudioStream>(Cfg.SOUND_UI_CANCEL_1);
		SoundDiceMany = GD.Load<AudioStream>(Cfg.SOUND_UI_DICEROLL_MANY);
		SoundDiceFew = GD.Load<AudioStream>(Cfg.SOUND_UI_DICEROLL_FEW);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LoadOverworldAudio()
	{
		SoundTravelWalk ??= GD.Load<AudioStream>(Cfg.SOUND_UI_TRAVEL_WALK);
		SoundTravelRide ??= GD.Load<AudioStream>(Cfg.SOUND_UI_TRAVEL_RIDE);
	}

	public void LoadCombatAudio()
	{
		// TODO: Implement combat, eventually.
	}

	public void PlaySound(Godot.AudioStream stream)
	{
		this.Stream = stream;
		this.Play();
	}
}
