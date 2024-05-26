using Godot;
using System;

public partial class Cfg
{
    public const string SOUND_UI_SCROLL_1 = PATH_SOUND + "727648__cjspellsfish__scroll-1.wav";
    public const string SOUND_UI_SELECT_1 = PATH_SOUND + "198449__cs279__menu-scroll-selection-sound.wav";
    public const string SOUND_UI_SELECT_2_CONFIRM = PATH_SOUND + "198448__cs279__menu-scroll-selection-sound.wav";
	public const string SOUND_UI_DICEROLL_MANY = PATH_SOUND + "220744__dermotte__dice-06.wav";
	public const string SOUND_UI_DICEROLL_FEW = PATH_SOUND + "353975__nettimato__rolling-dice-1.wav";
}

public partial class AudioHandler : AudioStreamPlayer
{
	public AudioStream
		soundScroll1,
		soundSelect1,
		soundConfirm1,
		soundDiceMany,
		soundDiceFew;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        soundScroll1 = GD.Load<AudioStream>(Cfg.SOUND_UI_SCROLL_1);
        soundSelect1 = GD.Load<AudioStream>(Cfg.SOUND_UI_SELECT_1);
        soundConfirm1 = GD.Load<AudioStream>(Cfg.SOUND_UI_SELECT_2_CONFIRM);
		soundDiceMany = GD.Load<AudioStream>(Cfg.SOUND_UI_DICEROLL_MANY);
		soundDiceFew = GD.Load<AudioStream>(Cfg.SOUND_UI_DICEROLL_FEW);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlaySound(Godot.AudioStream stream)
	{
		this.Stream = stream;
		this.Play();
	}
}
