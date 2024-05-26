using System;
using System.Linq;
using Godot;

// Links to Utils functions?

public partial class uiDevPanel : Control
{
    private bool _initialized = false;
    private GameManager gm;
    private Godot.Collections.Array<Node> DpuiInputs;
    private Godot.Collections.Array<Node> DpuiDisplays;
    private AudioHandler audioplayer;

    private RichTextReadout _activeRollReadout;
    private RichTextReadout _reactingRollReadout;

    [ExportGroup("DiceRollForm")]
    [Export]
    private SpinBox _activeHungerOverride;
    [Export]
    private SpinBox _reactingHungerOverride;
    [ExportGroup("")]

    private string _dicePool1 = "";

    [Signal]
    public delegate void FormTextInputEventHandler(Cfg.UI_KEY source, string text);
    [Signal]
    public delegate void FormSelectionEventHandler(string source, long selectionIndex);
    [Signal]
    public delegate void FormSubmitEventHandler(Cfg.UI_KEY source);

    public bool Initialized { get => _initialized; }

    private V5Entity _activeRoller;
    public V5Roll LastActiveRoll { get; private set; }
    private V5Entity _reactingRoller;
    public V5Roll LastReactingRoll { get; private set; }

    public uiDevPanel()
    {
        GD.Print($"{GetType()} constructor called.");
        if (!Cfg.DEV_MODE)
        {
            GD.Print(" -- But not in Dev Mode! Death calls!");
            this.QueueFree();
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print($"{GetType()}._Ready() fired!");

        if (!Cfg.DEV_MODE)
        {
            return;
        }
        gm = GetNode<GameManager>(Cfg.NODEPATH_ABS_GAMEMANAGER);
        gm.DevPanel = this;

        DpuiInputs = this.GetTree().GetNodesInGroup($"{Cfg.GROUP_NAMES.DPUI_INPUTS}");
        foreach (Node node in DpuiInputs)
        {
            Godot.Callable clbk;
            if (node is LineEdit)
            {
                clbk = Callable.From((Cfg.UI_KEY key, string text) => DpuiTextInput(key, text));
                node.Connect(SignalName.FormTextInput, clbk);
            }
            if (node is OptionButton || node is ItemList)
            {
                clbk = Callable.From((Cfg.UI_KEY key, long index) => DpuiSelection(key, index));
                node.Connect(SignalName.FormSelection, clbk);
            }
            if (node is Button && node is not OptionButton)
            {
                clbk = Godot.Callable.From((Cfg.UI_KEY key) => DpuiSubmit(key));
                node.Connect(SignalName.FormSubmit, clbk);
            }
        }

        DpuiDisplays = GetTree().GetNodesInGroup($"{Cfg.GROUP_NAMES.DPUI_DISPLAYS}");
        foreach (Node node in DpuiDisplays)
        {
            if (node is RichTextReadout rdt)
            {
                if (rdt.IsInGroup($"{Cfg.GROUP_NAMES.UI_ACTIVE_ROLL}"))
                {
                    _activeRollReadout = rdt;
                }
                else if (rdt.IsInGroup($"{Cfg.GROUP_NAMES.UI_REACTING_ROLL}"))
                {
                    _reactingRollReadout = rdt;
                }
                else
                {
                    GD.PrintErr($"{GetType()}: Unrecognized {typeof(RichTextReadout)} not in any expected groups.");
                }
            }
        }

        MoveMeToEnd();
        audioplayer = GetNode<AudioHandler>("/root/AudioHandler");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void MoveMeToEnd()
    {
        this.CallDeferred(MethodName.DeferredMoveMeToEnd);
    }

    protected void DeferredMoveMeToEnd()
    {
        Node myParent = this.GetParent();
        myParent.MoveChild(this, myParent.GetChildCount() - 1);
        _initialized = true;
    }

    public void UpdateAllFormDisplays()
    {
        // TODO: figure out how to get this in MethodName or something.
        // GetTree().CallGroup(
        //     $"{Cfg.GROUP_NAMES.DPUI_DISPLAYS}", "ShowDiceTestResults", LastActiveRoll.ToString()
        // );
        _activeRollReadout.UpdateText(LastActiveRoll.ToBBCodeString());
        _activeRollReadout.TooltipText = LastActiveRoll.ToString();
        if (LastReactingRoll is not default(V5Roll))
        {
            _reactingRollReadout.UpdateText(LastReactingRoll.ToBBCodeString());
            _reactingRollReadout.TooltipText = LastReactingRoll.ToString();
        }

    }

    public void DpuiTextInput(Cfg.UI_KEY key, string text)
    {
        switch (key)
        {
            case Cfg.UI_KEY.DICE_TEST_1:
                _dicePool1 = text;
                GD.Print($"\n -- dice pool #1 set to {_dicePool1}");
                break;
            default:
                GD.Print($"DpuiTextInput: Unrecognized source '{key}'");
                break;
        }
    }

    public void DpuiSelection(Cfg.UI_KEY key, long selectionIndex)
    {
        switch (key)
        {
            case Cfg.UI_KEY.SELECT_ACTIVE_ENTITY_ALL:
                _activeRoller = V5Entity.GetAllEntities()[(int)selectionIndex];
                break;
            case Cfg.UI_KEY.SELECT_REACTING_ENTITY_ALL:
                _reactingRoller = V5Entity.GetAllEntities()[(int)selectionIndex];
                break;
            default:
                GD.Print($"DpuiSelection: Unrecognized selection source '{key}'");
                break;
        }
    }

    public void DpuiSubmit(Cfg.UI_KEY key)
    {
        switch (key)
        {
            case Cfg.UI_KEY.DICE_TEST_1:
                audioplayer.PlaySound(audioplayer.soundDiceMany);
                RunDiceTestType1();
                break;
            default:
                GD.Print($"Unrecognized button press/form submit; source = {key}");
                break;
        }
    }

    public void RunDiceTestType1()
    {
        GD.Print($"\n--- Running dice test type 1, pool of {_dicePool1} ---\n");
        bool parsed = Int32.TryParse(_dicePool1, out int totalDicePool);
        if (!parsed)
        {
            throw new FormatException($"Couldn't parse \"{_dicePool1}\" to an integer.");
        }
        GD.Print($"Evaluated dice pool: {totalDicePool}");
        LastActiveRoll = new V5Roll(gm.playerchar, totalDicePool, true, defaultHunger: (int)_activeHungerOverride.Value);
        UpdateAllFormDisplays();
    }

    public void RunDiceTestType2()
    {

    }
}
