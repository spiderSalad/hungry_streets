using Godot;
using System;

public partial class lblBackgroundInfo : RichTextLabel
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddToGroup($"{Cfg.GROUP_NAMES.CCUI_FORM_DISPLAYS}");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta){}

    public void CharCreationUpdate(PlayerChar currentChar)
    {
        GD.Print($"{GetClass()}: Current Background is '{currentChar.Background.Name}'");
        string charName;
        try { charName = currentChar.Name.Trim(); } catch { charName = "-Ent-Unnamed"; }
        this.Text = "";
        if (charName is not string || charName == "" || charName.Substr(0, 4) == "-Ent")
        {
            this.Text += $"\"Oh, {currentChar.Pns.HerHimThem}? ";
        }
        else
        {
            this.Text += $"\"Oh, {charName}? ";
        }
        this.Text += $"{currentChar.Pns.ShesHesTheyre.Capitalize()} that ";
        // this.Text = charName == "Unnamed" ? $"{charName}" : $"{charName}, the";
        if (currentChar.Background != Cfg.BgEmpty)
        {
            this.Text += $"{currentChar.Background.Name}, right?\"";
            this.Text += $"\n\n{currentChar.Background.Desc}";
        }
        else
        {
            Text += "one lick, right?";
        }
    }
}
