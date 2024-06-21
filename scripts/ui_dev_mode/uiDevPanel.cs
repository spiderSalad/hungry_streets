using System;
using Godot;

// Links to Utils functions?

public partial class uiDevPanel : Control
{
    private bool _initialized = false;
    private GameManager gm;
    private Godot.Collections.Array<Node> DpuiInputs;
    private Godot.Collections.Array<Node> DpuiDisplays;
    private AudioHandler audioplayer;

    [ExportGroup("DiceRollForm")]
    [Export] private OptionButton _actorSelection;
    [Export] private LineEdit _dicePoolInput1;
    [Export] private SpinBox _activeHungerOverride;
    [Export] private SpinBox _reactingHungerOverride;
    [Export] private SpinBox _testDiffInput;
    [Export] private OptionButton _reactorSelection;
    [Export] private LineEdit _dicePoolInput2;
    [Export] private RichTextReadout _rollReadout;
    [Export] private Godot.Button _btnWillRerollWin;
    [Export] private Godot.Button _btnWillRerollRestrain;
    [ExportGroup("")]

    private string _dicePool1 = "";

    [Signal]
    public delegate void FormTextInputEventHandler(Cfg.UI_KEY source, string text);
    [Signal]
    public delegate void FormSelectionEventHandler(string source, long selectionIndex);
    [Signal]
    public delegate void FormSubmitEventHandler(Cfg.UI_KEY source);
    [Signal]
    public delegate void CharacterUpdateEventHandler(string opt, Variant val);

    public bool Initialized { get => _initialized; }

    private V5Entity _activeRoller;
    public V5Roll LastActiveRoll { get; private set; }
    private V5Entity _reactingRoller;
    public V5Roll LastReactingRoll { get; private set; }

    public V5Contest LastTest { get; private set; }
    public V5Contest LastContest { get; private set; }

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
        foreach (Node node in DpuiDisplays) { }

        MoveMeToEnd();
        audioplayer = GetNode<AudioHandler>("/root/AudioHandler");

        var allEnts = V5Entity.GetAllEntities();
        for (int i = 0; i < allEnts.Count; i++)
        {
            GD.Print($"{i:00}: {allEnts[i]}");
        }
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
        if (LastReactingRoll is not null && LastReactingRoll is not default(V5Roll))
        {
            _rollReadout.UpdateText($"{LastContest}");
            _rollReadout.TooltipText =
                $"{LastActiveRoll.UnfStr()}\n vs \n{LastReactingRoll.UnfStr()}";
        }
        else if (LastActiveRoll is not null && LastActiveRoll is not default(V5Roll))
        {
            _rollReadout.UpdateText($"{LastTest}");
            _rollReadout.TooltipText = $"{LastActiveRoll.UnfStr()}";
        }
        else
        {
            throw new ArgumentNullException(
                "Missing both active and reacting V5Roll objects! This shouldn't happen."
            );
        }

        if (LastActiveRoll.CanReroll)
        {
            _btnWillRerollWin.Disabled = !LastActiveRoll.PossibleWillRerollGain;
            _btnWillRerollRestrain.Disabled = !LastActiveRoll.AvoidableMessyCrit;
        }
        else
        {
            _btnWillRerollWin.Disabled = true;
            _btnWillRerollRestrain.Disabled = true;
        }

        if (LastActiveRoll.Actor is V5Entity ent1)
        {
            ent1.EmitSignal(SignalName.CharacterUpdate, "dicerolls", "actor");
        }
        if (LastReactingRoll is not default(V5Roll) && LastReactingRoll.Actor is V5Entity ent2)
        {
            ent2.EmitSignal(SignalName.CharacterUpdate, "dicerolls", "reactor");
        }
    }

    public void DpuiTextInput(Cfg.UI_KEY key, string text)
    {
        switch (key)
        {
            case Cfg.UI_KEY.DICE_TEST_1_TEST:
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
            case Cfg.UI_KEY.DICE_TEST_1_TEST:
                audioplayer.PlaySound(audioplayer.soundDiceMany);
                RunDiceTestType1x(false);
                break;
            case Cfg.UI_KEY.DICE_TEST_2_REROLL_WIN:
                audioplayer.PlaySound(audioplayer.soundDiceFew);
                RunDiceTestType2(true, false);
                break;
            case Cfg.UI_KEY.DICE_TEST_3_REROLL_RESTRAIN:
                audioplayer.PlaySound(audioplayer.soundDiceFew);
                RunDiceTestType2(false, true);
                break;
            case Cfg.UI_KEY.DICE_TEST_4_CONTEST:
                audioplayer.PlaySound(audioplayer.soundDiceMany);
                RunDiceTestType1x(true);
                break;
            default:
                GD.Print($"Unrecognized button press/form submit; source = {key}");
                break;
        }
    }

    public void RunDiceTestType1()
    {
        string dicePoolStr = _dicePoolInput1.Text;
        GD.Print($"\n--- Running dice test type 1, pool of {_dicePool1}/{dicePoolStr} ---\n");
        bool parsed = Int32.TryParse(dicePoolStr, out int totalDicePool);
        if (!parsed)
        {
            throw new FormatException($"Couldn't parse \"{dicePoolStr}\" to an integer.");
        }
        GD.Print($"Evaluated dice pool: {totalDicePool}");
        LastActiveRoll = new V5Roll(
            gm.playerchar, totalDicePool, true, hungerOverride: (int)_activeHungerOverride.Value
        );
        UpdateAllFormDisplays();
    }

    public void RunDiceTestType1x(bool isContest)
    {
        string dicePoolStr = _dicePoolInput1.Text;
        V5Entity actor = V5Entity.GetEntityFromList(_actorSelection.Selected, gm.playerchar);
        var totalDicePool = V5PoolParser.ParsePoolText(dicePoolStr, actor) ??
            throw new FormatException($"Couldn't parse \"{dicePoolStr}\" to an integer.");

        if (isContest)
        {
            string oppDicePoolStr = _dicePoolInput2.Text;
            V5Entity reactor = V5Entity.GetEntityFromList(_reactorSelection.Selected);
            GD.Print($" ----> Actor: {actor}, Reactor: {reactor}");
            var enemyDicePool = V5PoolParser.ParsePoolText(oppDicePoolStr, reactor) ??
                throw new FormatException("Couldn't parse \"{dicePoolStr}\" to an integer.");
            GD.Print($"\n - Contest, pool of {totalDicePool} vs opp pool {enemyDicePool}\n");
            LastContest = V5Contest.Contest(
                totalDicePool, actor, true, enemyDicePool, reactor, true,
                actorHungerOverride: (int) _activeHungerOverride.Value,
                reactorHungerOverride: (int) _reactingHungerOverride.Value,
                rng: gm.Rng
            );
            LastActiveRoll = LastContest.ActorRoll; LastReactingRoll = LastContest.ReactorRoll;
        }
        else
        {
            int difficulty = (int)_testDiffInput.Value;
            GD.Print($" ----> Actor: {actor}");
            GD.Print($"\n - Test, pool of {totalDicePool} vs flat diff {difficulty}\n");
            LastTest = V5Contest.Test(
                totalDicePool, actor, difficulty, true,
                actorHungerOverride: (int)_activeHungerOverride.Value, // TODO: check checkbox here
                rng: gm.Rng
            );
            LastActiveRoll = LastTest.ActorRoll; LastReactingRoll = null;
        }
        UpdateAllFormDisplays();
    }

    public void RunDiceTestType2(bool will2Win, bool will2Restrain)
    {
        GD.Print($"Willpower re-roll! will2Win = {will2Win}, will2Restrain = {will2Restrain}");
        LastActiveRoll.Reroll(will2Win, will2Restrain);
        LastTest = LastTest is not null ? LastTest.GetOutcome() : LastTest;
        LastContest = LastContest is not null ? LastContest.GetOutcome() : LastContest;
        UpdateAllFormDisplays();
    }
}
