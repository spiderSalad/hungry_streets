using Godot;
using System;

public partial class uiCharCreateRoot : Control
{
    public PlayerChar characterToBe;

    private GameManager gm;
    private Godot.Collections.Array<Node> CcuiFormInputs;
    private AudioHandler audioplayer;

    [Signal]
    public delegate void FormTextInputEventHandler(Cfg.UI_KEY source, string text);
    [Signal]
    public delegate void FormSelectionEventHandler(Cfg.UI_KEY source, long selectionIndex);
    [Signal]
    public delegate void FormSubmitEventHandler(Cfg.UI_KEY source);

    //public delegate void CharCreationUpdate(PlayerChar currentChar);

    public uiCharCreateRoot()
    {
        GD.Print("Character Creation UI | Constructor called.");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print($"{GetType()}._Ready() start.");

        DumpTestNodes();
        gm = GetNode<GameManager>(Cfg.NODEPATH_ABS_GAMEMANAGER);
        if (gm.playerchar is default(PlayerChar))
        {
            characterToBe = new PlayerChar();
            GD.Print($"{GetType()}._Ready(): Creating new character ({characterToBe}).");
        }
        else
        {
            characterToBe = gm.playerchar;
            GD.Print(
                $"{GetType()}._Ready(): Picking up existing character "
                + $"({gm.playerchar} should equal ({characterToBe}))."
            );
        }
        // characterToBe = gm.playerchar is default(PlayerChar) ? new PlayerChar() : gm.playerchar;

        //CcuiFormInputs = this.GetTree().GetNodesInGroup(Cfg.GROUP_CCUI_FORM_INPUTS);
        CcuiFormInputs = this.GetTree().GetNodesInGroup($"{Cfg.GROUP_NAMES.CCUI_FORM_INPUTS}");
        foreach (Node node in CcuiFormInputs)
        {
            Godot.Callable clbk;
            if (node is LineEdit)
            {
                clbk = Callable.From((Cfg.UI_KEY key, string text) => CcuiTextInput(key, text));
                node.Connect(SignalName.FormTextInput, clbk);
            }
            if (node is OptionButton || node is ItemList)
            {
                clbk = Callable.From((Cfg.UI_KEY key, long index) => CcuiSelection(key, index));
                node.Connect(SignalName.FormSelection, clbk);
            }
            if (node is Button && node is not OptionButton)
            {
                clbk = Godot.Callable.From((Cfg.UI_KEY key) => CcuiSubmit(key));
                node.Connect(SignalName.FormSubmit, clbk);
            }
        }

        UpdateAllFormDisplays();
        Godot.Collections.Array<Node> tabcons = this.FindChildren("tabcon*");
        foreach (Node node in tabcons)
        {
            if (node is TabContainer tabcon)
            {
                tabcon.TabChanged += CcuiTabSwitch;
            }
        }

        audioplayer = GetNode<AudioHandler>("/root/AudioHandler");
        GD.Print($"{GetType()}._Ready() end.");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void UpdateAllFormDisplays()
    {
        // TODO: figure out how to get CharCreationUpdate in MethodName or something.
        GetTree().CallGroup(
            $"{Cfg.GROUP_NAMES.CCUI_FORM_DISPLAYS}", "CharCreationUpdate", this.characterToBe
        );
    }

    public void CcuiTextInput(Cfg.UI_KEY key, string text)
    {
        switch (key)
        {
            case Cfg.UI_KEY.NAME_ENTRY:
                characterToBe.Name = Utils.SanitizeStr(text);
                UpdateAllFormDisplays();
                break;
            default:
                GD.Print($"CcuiTextInput: Unrecognized source '{key}'");
                break;
        }
        audioplayer.PlaySound(audioplayer.soundSelect1);
    }

    public void CcuiSelection(Cfg.UI_KEY key, long selectionIndex)
    {
        switch (key)
        {
            case Cfg.UI_KEY.PRONOUN_SELECT:
                characterToBe.Pns = Cfg.PcGenders[selectionIndex - 1];
                UpdateAllFormDisplays(); break;
            case Cfg.UI_KEY.BACKGROUND_SELECT:
                characterToBe.Background = Cfg.PcBackgrounds[selectionIndex];
                UpdateAllFormDisplays(); break;
            default:
                GD.Print($"CcuiSelection: Unrecognized selection source '{key}'");
                break;
        }
        audioplayer.PlaySound(audioplayer.soundSelect1);
    }

    public void CcuiSubmit(Cfg.UI_KEY key)
    {
        switch (key)
        {
            default:
                GD.Print("CcuiSubmit called (default behavior).");
                Tuple<bool, string[]> validationResults = ValidateCharacter(characterToBe);
                if (validationResults.Item1)
                {
                    if (gm.playerchar is default(PlayerChar))
                    {
                        gm.playerchar = characterToBe;
                    }
                    else
                    {
                        GD.Print(
                            "Existing player character updated. " +
                            $"({gm.playerchar} should be the same as {characterToBe})"
                        );
                    }
                    gm.GoToNamedScene(Cfg.SCENE_OWUI);
                }
                else
                {

                }
                // TODO: Probably want logic to disable form activity here,
                // and reenable it elsewhere.
                break;
        }
        audioplayer.PlaySound(audioplayer.soundConfirm1);
    }

    public void CcuiTabSwitch(long tab)
    {
        GD.Print($"CcuiTabSwitch: {tab}");
        switch (tab) { default: break; }
        audioplayer.PlaySound(audioplayer.soundSelect1);
    }

    private Tuple<bool, string[]> ValidateCharacter(PlayerChar currentChar)
    {
        return new Tuple<bool, string[]>(true, new string[] { "possibleIssue1", "possibleIssue2" });
    }

    public void DumpTestNodes()
    {
        Godot.Collections.Array<Node> testNodes = this.FindChildren("*TEST*", recursive: true, owned: false);
        foreach (var testNode in testNodes)
        {
            if (testNode is Node)
            {
                Node byenode = (Node)testNode;
                byenode.GetParent().RemoveChild(byenode);
                byenode.Free();
            }
        }
    }
}