using Godot;
using System;
using System.IO;

public partial class GameManager : Node
{
    public static readonly Godot.Collections.Dictionary<string, string> SCENE_FILEPATHS = 
        new Godot.Collections.Dictionary<string, string>{
            [Cfg.SCENE_ROOT] = "res://scenes/test_game_root_2.tscn",
            [Cfg.SCENE_CCUI] = "res://scenes/ui_char_create.tscn",
            [Cfg.SCENE_OWUI] = "res://scenes/ui_overworld.tscn"
        };

    private PlayerChar _playerchar;
    private uiDevPanel _devPanel;

    public Node CurrentScene { get; private set; }
    public uiDevPanel DevPanel {
        get => _devPanel;
        set {
            if (!Cfg.DEV_MODE) {
                throw new InvalidOperationException(
                    "Not allowed to set dev panel outside of dev mode!"
                );
            }
            _devPanel = value;
        }
    }
    public PlayerChar playerchar {
        get => _playerchar;
        set {
            GD.Print($"{GetType()}:: PlayerChar set! ({_playerchar} -> {value}");
            _playerchar = value;
        }
    }

    public GameManager()
    {
        GD.Print("I'm bout them commas, fuck your comments.");
        Utils.PopulateStatDict(Cfg.Attrs, Cfg.AttrsDict);
        Utils.PopulateStatDict(Cfg.Skills, Cfg.SkillsDict);

        GD.Print("---a01----");

        Cfg.AllStats.AddRange(Cfg.Attrs);
        Cfg.AllStats.AddRange(Cfg.Skills);
        Cfg.AllStats.AddRange(Cfg.Disciplines);
        Cfg.AllStats.AddRange(Cfg.Backgrounds);

        GD.Print("---a02----");
    }

    public override void _Ready()
    {
        Viewport root = this.GetTree().Root; // Why Viewport?
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        // CurrentScene is last child of root at this point because this (GameManager) is an
        // auto-loaded node and they come first (presumably that means this _Ready() fires
        // before other nodes are added to root? Unsure.)
        GD.Print("Game manager ready.");
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
}
