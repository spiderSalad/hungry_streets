using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class GameManager : Node
{
    [Signal] public delegate void GameStateLoadedEventHandler();
    [Signal] public delegate void GameStateSavedEventHandler();
    [Signal] public delegate void PcUpdateEventHandler(Cfg.UI_KEY source, PlayerChar pc);
    [Signal] public delegate void OtherEntUpdateEventHandler(Cfg.UI_KEY source, V5Entity newEnt);

    public static readonly Godot.Collections.Dictionary<string, string> SCENE_FILEPATHS = new()
    {
        [Cfg.SCENE_ROOT] = "res://scenes/test_game_root_2.tscn",
        [Cfg.SCENE_CCUI] = "res://scenes/ui_char_create.tscn",
        [Cfg.SCENE_OWUI] = "res://scenes/ui_overworld.tscn"
    };

    private PlayerChar _playerChar;
    private readonly List<PlayerChar> _allCreatedChars = new();
    private uiDevPanel _devPanel;
    private string _testSavePath = "res://temp/testsave04.tres";

    public readonly Godot.Collections.Dictionary<string, PlayerChar> DEMO_CHARS = new();
    public Godot.RandomNumberGenerator Rng { get; private set; }
    public Node CurrentScene { get; private set; }
    public GameState PreviousState { get; private set; }
    public GameState PresentState { get; private set; }
    public uiDevPanel DevPanel
    {
        get => _devPanel;
        set
        {
            if (!Cfg.DEV_MODE)
            {
                throw new InvalidOperationException(
                    "Not allowed to set dev panel outside of dev mode!"
                );
            }
            _devPanel = value;
        }
    }
    public PlayerChar ThePc
    {
        get => _playerChar;
        set
        {
            if (_playerChar == value)
            {
                GD.PrintErr($"GM:: PlayerChars {_playerChar} and {value} are identical!");
                return;
            }
            if (_playerChar == default)
            {
                GD.Print($"GM:: PlayerChar set for the first time to {value}.");
            }
            else
            {
                GD.Print($"GM:: (new) PlayerChar set. ({_playerChar} -> {value}");
            }
            _playerChar = value;
            if (PresentState != default)
            {
                GD.Print($"GM: Added new Pc '{_playerChar}' to save state.");
                PresentState.PcDataBundle = _playerChar.Bundle();
            }
            _allCreatedChars.Add(_playerChar);
        }
    }

    public GameManager()
    {
        InitGameParams();
    }

    protected void InitGameParams()
    {
        Utils.PopulateStatDict(Cfg.Attrs, Cfg.AttrsDict);
        Utils.PopulateStatDict(Cfg.Skills, Cfg.SkillsDict);

        foreach (PronounSet pns in Cfg.PronounSetsAll) { Cfg.PronounSetsDict.Add(pns.Id, pns); }

        foreach (CharBackground charbg in Cfg.PcBackgrounds)
        {
            Cfg.PcBgsDict.Add(charbg.Name, charbg);
            DEMO_CHARS.Add(charbg.Name, new PlayerChar()
            {
                Name = $"Fk{charbg.Name[..7]}",
                Background = charbg
            });
        }

        Rng = Utils.SetRng(new RandomNumberGenerator(), true);
        ValidateGameParams();
    }

    protected void ValidateGameParams()
    {
        // Check that powers are in their proper place within discipline trees.
        foreach (var keyValuePair in Cfg.DiscPowerTrees)
        {
            string discId = keyValuePair.Key;
            List<List<V5Power>> discTree = keyValuePair.Value;
            for (int i = 0; i < discTree.Count; i++)
            {
                List<V5Power> powersAtRank = discTree[i];
                foreach (V5Power power in powersAtRank)
                {
                    if (power.Rank != i + 1)
                    {
                        throw new ArgumentOutOfRangeException(
                            "ValidateGameParams(): " +
                            $"Power '{power.Name}' has wrong rank ({power.Rank}, should be {i + 1})!"
                        );
                    }
                    if (power.StatId != discId)
                    {
                        throw new ArgumentOutOfRangeException(
                            "ValidateGameParams(): " +
                            $"Power '{power.Name}' has discipline id '{power.StatId}', " +
                            $"but it's been placed in '{discId}'!"
                        );
                    }
                }
            }
        }
    }

    public override void _Ready()
    {
        Viewport root = this.GetTree().Root; // Why Viewport?
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        // CurrentScene is last child of root at this point because this (GameManager) is an
        // auto-loaded node and they come first (presumably that means this _Ready() fires
        // before other nodes are added to root? Unsure.)
        GD.Print("Game manager ready.");
        //TempCheckForGameStates();
        LoadGame(Cfg.FILENAME_RECENT_TEMP, true); // Auto-attempt an auto-load.
    }

    public Godot.Error LoadGame(string path, bool autoLoad = false, bool createFallbackState = true)
    {
        string fullPath = Utils.GetFullSavePath(path);
        GD.Print(
            autoLoad ?
            $"\nGM: Auto-loading...\n" : $"\n\nGM: Loading game at '{fullPath}'...\n\n"
        );
        Error result;
        PreviousState ??= autoLoad ? PreviousState : PresentState;
        try
        {
            if (ResourceLoader.Exists(fullPath))
            {
                // TODO: Reflect loading/saving in UI
                PresentState = GD.Load<GameState>(fullPath);
                HandleLoadNewState(fullPath);
                GD.Print($"\nGM: Successfully loaded from '{fullPath}'.\nState:\n{PresentState}\n");
                if (!autoLoad)
                {
                    SaveGame(Cfg.FILENAME_RECENT_TEMP, true);
                }
                return Error.Ok;
            }
            else
            {
                GD.PrintErr($"GM: Failed to load; no Resource exists at '{fullPath}'!");
                result = Error.FileNotFound;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"GM: Failed to load from path '{fullPath}' due to:\n{ex}");
            result = Error.Failed;
        }
        if (result != Error.Ok && createFallbackState && PresentState == default)
        {
            PresentState = new GameState();
        }
        return result;
    }

    public void HandleLoadNewState(string pathUsed)
    {
        GD.Print($"\nHandle load state: {PresentState.Id},\n\tfrom path '{pathUsed}'\n");
        PresentState.OnLoad(pathUsed);
        ThePc = PlayerChar.BuildFromBundle(new PlayerChar(), PresentState.PcDataBundle);
        ThePc.CurrentLocation = Cfg.LocationsDict[PresentState.CurrentLoc];

        foreach (MapLoc loc in Cfg.StartingUnlockedLocs)
        {
            GD.Print($"starting loc: {loc}");
            if (!PresentState.UnlockedLocations.Contains(loc.LocId))
            {
                PresentState.UnlockedLocations.Add(loc.LocId);
            }
        }

        EmitSignal(SignalName.GameStateLoaded);
        SignalPcUpdate(Cfg.UI_KEY.LOADED_GAME_ENT_UPDATE, ThePc);
    }

    public Godot.Error SaveGame(string path, bool overWriteTemp = false)
    {
        string fullPath = Utils.GetFullSavePath(path);
        GD.Print($"\nGM: Saving current game to '{fullPath}'...\n");
        CreateSaveFromCurrentState();
        ResourceSaver.Save(PresentState, Utils.GetFullSavePath(Cfg.FILENAME_RECENT_TEMP));
        Godot.Error err = ResourceSaver.Save(PresentState, fullPath);
        if (err == Error.Ok)
        {
            if (!overWriteTemp)
            {
                PresentState.OnWrite(fullPath);
                EmitSignal(SignalName.GameStateSaved);
            }
        }
        else
        {
            GD.PrintErr($"GM: Save failed, with error code '{err}'.");
        }
        return err;
    }

    public void CreateSaveFromCurrentState()
    {
        PresentState ??= new();
        PresentState.PcDataBundle = ThePc.Bundle();
        PresentState.CurrentLoc = ThePc.CurrentLocation.LocId;
    }

    public void GoToScene(string path)
    {
        // We don't want the scene switch to actually fire until idle time, meaning when the
        // current scene has finished executing - which might not be the case when the call
        // happens. Terminating the calling scene early can cause problems.
        this.CallDeferred(MethodName.DeferredGoToScene, path);
    }

    public void GoToNamedScene(string sceneName)
    {
        GoToScene(GameManager.SCENE_FILEPATHS[sceneName]);
    }

    public void DeferredGoToScene(string path)
    {
        // get current scene
        // wait for it to finish
        CurrentScene.Free();
        // destroy it
        // load new scene
        var nextScene = GD.Load<PackedScene>(path);
        // instantiate it
        CurrentScene = nextScene.Instantiate();
        // add it to tree
        this.GetTree().Root.AddChild(CurrentScene);
        this.GetTree().CurrentScene = CurrentScene;

        // What do these below do differenlty?
        // this.GetTree().ChangeSceneToFile(path);
        // this.GetTree().ChangeSceneToPacked(nextScene);

        if (Cfg.DEV_MODE && DevPanel.Initialized)
        {
            DevPanel.MoveMeToEnd();
        }
    }

    public void SignalPcUpdate(Cfg.UI_KEY source, PlayerChar pc)
    {
        if (pc == ThePc)
        {
            EmitSignal(SignalName.PcUpdate, (int)source, ThePc);
        }
        else
        {
            GD.PrintErr($"GM: Won't broadcast update to '{pc}', as they're not ThePC('{ThePc}')");
        }
    }

    public void SignalEntityUpdate(Cfg.UI_KEY source, V5Entity ent)
    {
        EmitSignal(SignalName.OtherEntUpdate, (int)source, ent);
    }
}
