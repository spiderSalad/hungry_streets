using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class GameState : Godot.Resource
{
    public string PathLastLoadedFrom { get; private set; }
    public string PathLastSavedTo { get; private set; }
    [Export] public string Id { get; private set; }
    // [Export] public PlayerChar Pc { get; set; }
    [Export] public V5EntBundle PcDataBundle { get; set; }
    [Export] public int TimePlayed { get; private set; }
    [Export] public int TimesLoaded { get; set; }
    [Export] public int TimesWrittenTo { get; set; }

    [Export] public int TestField1 { get; set; }// = 201;
    [Export] public string TestField2 { get; set; }// = "ole you fuck";

    public string TestField3Unex { get; set; }


    // TODO: Obviously these strings have to be replaced with some other type.

    public string CurrentLoc { get; set; }
    private string _playerHaven = "any";
    public string PlayerHaven
    {
        get => _playerHaven;
        set
        {
            if (value is not null && _playerHaven != value)
            {
                PreviousPlayerHavens.Add(_playerHaven);
            }
            _playerHaven = value;

        }
    }
    public List<string> PreviousPlayerHavens { get; private set; }

    // Zero-parameter constructor required for compatibility reasons.
    public GameState() : this($"save-{DateTime.Now.ToString(Cfg.DT_FORMAT_SAVE)}", null, 0)
    {
        GD.Print("Creating blank GameState...");
    }

    public GameState(string id, V5EntBundle pcBundle, int timePlayed)
    {
        GD.Print($"Creating GameState '{id}' feat. PC '{pcBundle}', with time '{timePlayed}'.");
        Id = id; PcDataBundle = pcBundle; TimePlayed = timePlayed;
    }

    public void OnLoad(string pathUsed) { TimesLoaded++; PathLastLoadedFrom = pathUsed; }
    public void OnWrite(string pathUsed) { TimesWrittenTo++; PathLastSavedTo = pathUsed; }

    public override string ToString()
    {
        string toStr = $"-- GameState <{Id}> --\n";
        toStr += $"Times Loaded: {TimesLoaded}, Times Saved To: {TimesWrittenTo}\n";
        toStr += $"Character: '{PcDataBundle?.Name ?? "-None-"}'\nTime played: {TimePlayed}\n";
        toStr += $"TestField1: {TestField1}\nTestField2: {TestField2}\n";
        toStr += $"TestField3: {TestField3Unex}\n";
        toStr += "-- End GameState --\n";
        return toStr;
    }
}