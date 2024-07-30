using System;
using Godot;

public partial class uiDevPanel : Control
{
    private bool _initialized = false;
    private GameManager gm;
    private Godot.Collections.Array<Node> DpuiInputs;
    private Godot.Collections.Array<Node> DpuiDisplays;
    private AudioHandler audioplayer;

    [ExportGroup("ToggleMain")]
    [Export] private CheckButton _toggleDevToolsBtn;
    [Export] private Control _devToolsContainer;
    [Export] private Control _devPanelOuter;

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

    [ExportGroup("Misc. Tests")]
    [Export] private Button _btnCashTest1;
    [Export] private LineEdit _inputMinutesToAdd;
    [Export] private Button _btnAddMinutes;

    [ExportGroup("SaveLoadForm")]
    [Export] private Label _saveStateLabel;
    [Export] private LineEdit _inputSaveFileName;
    [Export] private Button _btnSaveTo;
    [Export] private LineEdit _inputLoadFileName;
    [Export] private Button _btnLoadFrom;

    [ExportGroup("")] // Can I only use this to break out once?

    private string _dicePool1 = "";

    private V5Entity _activeRoller;
    public V5Roll LastActiveRoll { get; private set; }
    private V5Entity _reactingRoller;
    public V5Roll LastReactingRoll { get; private set; }

    public V5Contest LastTest { get; private set; }
    public V5Contest LastContest { get; private set; }

    [Signal] public delegate void FormTextInputEventHandler(Cfg.UI_KEY source, string text);
    [Signal] public delegate void FormSelectionEventHandler(string source, long selectionIndex);
    [Signal] public delegate void FormSubmitEventHandler(Cfg.UI_KEY source);
    [Signal] public delegate void PcUpdateEventHandler(Cfg.UI_KEY source, PlayerChar pc);
    [Signal] public delegate void GameStateLoadedEventHandler();
    [Signal] public delegate void GameStateSavedEventHandler();

    private bool mostRecentSaveSignalWasSave = false;

    public bool Initialized { get => _initialized; }

    public enum MISC_TEST_KEY
    {
        ADD_CASH,
        RUN_CLOCK
    }

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
        gm.Connect(SignalName.GameStateLoaded, Godot.Callable.From(() => OnGameLoaded()));
        gm.Connect(SignalName.GameStateSaved, Godot.Callable.From(() => OnGameSaved()));

        ToggleMe(_toggleDevToolsBtn.ButtonPressed);
        _toggleDevToolsBtn.Toggled += ToggleMe;

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
        _btnCashTest1.Pressed += () => DpuiMiscTests(MISC_TEST_KEY.ADD_CASH);
        _btnAddMinutes.Pressed += () => DpuiMiscTests(MISC_TEST_KEY.RUN_CLOCK);

        MoveMeToEnd();
        audioplayer = GetNode<AudioHandler>("/root/AudioHandler");

        var allEnts = V5Entity.GetAllEntities();
        for (int i = 0; i < allEnts.Count; i++)
        {
            //GD.Print($"{i:00}: {allEnts[i]}");
        }

        UpdateAllFormDisplays();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void MoveMeToEnd()
    {
        this.CallDeferred(MethodName.DeferredMoveMeToEnd);
    }

    public void ToggleMe(bool toggledOn)
    {
        _devToolsContainer.Visible = toggledOn;
        _devPanelOuter.SizeFlagsVertical = SizeFlags.ShrinkBegin; // Don't think this works here.
        _devPanelOuter.Size = new(
            _devPanelOuter.Size.X, toggledOn ? 615 : _toggleDevToolsBtn.Size.Y + 10
        );
    }

    protected void DeferredMoveMeToEnd()
    {
        Node myParent = this.GetParent();
        myParent.MoveChild(this, myParent.GetChildCount() - 1);
        _initialized = true;
    }

    public void UpdateAllFormDisplays()
    {
        UpdateFormDiceRolls();
        UpdateFormSaveLoad();
    }

    public void UpdateFormDiceRolls()
    {
        // TODO: figure out how to get this in MethodName or something.
        // GetTree().CallGroup(
        //     $"{Cfg.GROUP_NAMES.DPUI_DISPLAYS}", "ShowDiceTestResults", LastActiveRoll.ToString()
        // );
        if (LastReactingRoll != default)
        {
            _rollReadout.UpdateText($"{LastContest}");
            _rollReadout.TooltipText =
                $"{LastActiveRoll.UnfStr()}\n vs \n{LastReactingRoll.UnfStr()}";
        }
        else if (LastActiveRoll != default)
        {
            _rollReadout.UpdateText($"{LastTest}");
            _rollReadout.TooltipText = $"{LastActiveRoll.UnfStr()}";
        }
        else
        {
            GD.Print("Neither \"Last Roll\" object present; dice form update ends here."); return;
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

        if (LastActiveRoll.Actor is PlayerChar pc1)
        {
            // pc1.EmitSignal(SignalName.PcUpdate, (int)Cfg.UI_KEY.ROLL_ENT_UPDATE, pc1);
            gm.SignalPcUpdate(Cfg.UI_KEY.ROLL_ENT_UPDATE, pc1);

        }
        else if (LastActiveRoll.Actor is V5Entity ent1)
        {
            gm.SignalEntityUpdate(Cfg.UI_KEY.ROLL_ENT_UPDATE, ent1);
        }

        if (LastReactingRoll != default)
        {
            if (LastReactingRoll.Actor is PlayerChar pc2)
            {
                // pc2.EmitSignal(SignalName.PcUpdate, (int)Cfg.UI_KEY.ROLL_ENT_UPDATE, pc2);
                gm.SignalPcUpdate(Cfg.UI_KEY.ROLL_ENT_UPDATE, pc2);
            }
            else if (LastReactingRoll.Actor is V5Entity ent2)
            {
                gm.SignalEntityUpdate(Cfg.UI_KEY.ROLL_ENT_UPDATE, ent2);
            }
        }
    }

    public void OnGameLoaded()
    {
        GD.Print($"Dpui: Received signal indicating loaded state '{gm.PresentState.Id}'");
        mostRecentSaveSignalWasSave = false;
        UpdateFormSaveLoad();
    }

    public void OnGameSaved()
    {
        GD.Print($"Dpui: Received signal indicating saved state '{gm.PresentState.Id}'");
        mostRecentSaveSignalWasSave = true;
        UpdateFormSaveLoad();
    }

    public void UpdateFormSaveLoad()
    {
        string newText = "Current Save ";
        newText += mostRecentSaveSignalWasSave ? "(Most recent save)" : "(Most recent load)";
        newText += $":\n'{gm.PresentState.Id}',\nfrom '";
        newText += mostRecentSaveSignalWasSave ?
                $"{gm.PresentState.PathLastSavedTo}" : $"{gm.PresentState.PathLastLoadedFrom}";
        newText += "')";
        _saveStateLabel.Text = newText;
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
        Error err;

        switch (key)
        {
            case Cfg.UI_KEY.DICE_TEST_1_TEST:
                audioplayer.PlaySound(audioplayer.SoundDiceMany);
                RunDiceTestType1x(false);
                break;
            case Cfg.UI_KEY.DICE_TEST_2_REROLL_WIN:
                audioplayer.PlaySound(audioplayer.SoundDiceFew);
                RunDiceTestType2(true, false);
                break;
            case Cfg.UI_KEY.DICE_TEST_3_REROLL_RESTRAIN:
                audioplayer.PlaySound(audioplayer.SoundDiceFew);
                RunDiceTestType2(false, true);
                break;
            case Cfg.UI_KEY.DICE_TEST_4_CONTEST:
                audioplayer.PlaySound(audioplayer.SoundDiceMany);
                RunDiceTestType1x(true);
                break;
            case Cfg.UI_KEY.LOAD_GAME_FROM_PATH:
                err = gm.LoadGame(_inputLoadFileName.Text);
                if (err == Error.Ok)
                {
                    audioplayer.PlaySound(audioplayer.SoundConfirm1);
                }
                break;
            case Cfg.UI_KEY.SAVE_GAME_TO_PATH:
                err = gm.SaveGame(_inputSaveFileName.Text);
                if (err == Error.Ok)
                {
                    audioplayer.PlaySound(audioplayer.SoundConfirm1);
                }
                break;
            default:
                GD.Print($"Unrecognized button press/form submit; source = {key}");
                break;
        }
    }

    public void DpuiMiscTests(MISC_TEST_KEY whichTest)
    {
        switch (whichTest)
        {
            case MISC_TEST_KEY.ADD_CASH:
                GD.Print("Mo money, no problems");
                gm.ThePc.Cash += 100;
                //gm.SignalPcUpdate(Cfg.UI_KEY.OTHER_GAMEPLAY_ENT_UPDATE, gm.ThePc);
                audioplayer.PlaySound(audioplayer.SoundConfirm1);
                break;
            case MISC_TEST_KEY.RUN_CLOCK:
                string inputText = _inputMinutesToAdd.Text;
                bool parsed = Int32.TryParse(inputText, out int minutesToAdd);
                if (parsed)
                {
                    GD.Print($"Cock Test: Advancing {minutesToAdd} minutes...");
                    gm.TheClock.Advance(minutesToAdd);
                }
                else
                {
                    GD.PrintErr($"Clock Test: Could not parse minutes from \"{inputText}\"");
                }
                audioplayer.PlaySound(audioplayer.SoundSelect1);
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
            gm.ThePc, totalDicePool, true, hungerOverride: (int)_activeHungerOverride.Value
        );
        UpdateFormDiceRolls();
    }

    public void RunDiceTestType1x(bool isContest)
    {
        string dicePoolStr = _dicePoolInput1.Text;
        V5Entity actor = V5Entity.GetEntityFromList(_actorSelection.Selected, gm.ThePc);
        var totalDicePool = V5PoolParser.ParsePoolText(dicePoolStr, actor) ??
            throw new FormatException($"Couldn't parse \"{dicePoolStr}\" to an integer.");

        if (isContest)
        {
            string oppDicePoolStr = _dicePoolInput2.Text;
            V5Entity reactor = V5Entity.GetEntityFromList(_reactorSelection.Selected);
            GD.Print($" ----> Actor: {actor}, Reactor: {reactor}");
            var enemyDicePool = V5PoolParser.ParsePoolText(oppDicePoolStr, reactor) ??
                throw new FormatException("Couldn't parse \"{dicePoolStr}\" to an integer.");

            string msg = $"\n - Contest, pool of {totalDicePool} vs opp pool {enemyDicePool}\n";
            if (totalDicePool.AnyTokenHasError() || enemyDicePool.AnyTokenHasError())
            {
                GD.PrintErr(msg);
            }
            else
            {
                GD.Print(msg);
            }

            LastContest = V5Contest.Contest(
                totalDicePool, actor, true, enemyDicePool, reactor, true,
                actorHungerOverride: (int)_activeHungerOverride.Value,
                reactorHungerOverride: (int)_reactingHungerOverride.Value,
                rng: gm.Rng
            );
            LastActiveRoll = LastContest.ActorRoll; LastReactingRoll = LastContest.ReactorRoll;
        }
        else
        {
            int difficulty = (int)_testDiffInput.Value;
            GD.Print($" ----> Actor: {actor}");
            string msg = $"\n - Test, pool of {totalDicePool} vs flat diff {difficulty}\n";
            if (totalDicePool.AnyTokenHasError())
            {
                GD.PrintErr(msg);
            }
            else
            {
                GD.Print(msg);
            }
            LastTest = V5Contest.Test(
                totalDicePool, actor, difficulty, true,
                actorHungerOverride: (int)_activeHungerOverride.Value, // TODO: check checkbox here
                rng: gm.Rng
            );
            LastActiveRoll = LastTest.ActorRoll; LastReactingRoll = null;
        }
        UpdateFormDiceRolls();
    }

    public void RunDiceTestType2(bool will2Win, bool will2Restrain)
    {
        GD.Print($"Willpower re-roll! will2Win = {will2Win}, will2Restrain = {will2Restrain}");
        LastActiveRoll.Reroll(will2Win, will2Restrain);
        LastTest = LastTest is not null ? LastTest.GetOutcome() : LastTest;
        LastContest = LastContest is not null ? LastContest.GetOutcome() : LastContest;
        UpdateFormDiceRolls();
    }
}
