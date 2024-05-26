using Godot;
using System;

public partial class lblCharSummary : RichTextLabel
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddToGroup($"{Cfg.GROUP_NAMES.CCUI_FORM_DISPLAYS}");
        this.AddToGroup($"{Cfg.GROUP_NAMES.OWUI_DISPLAYS}");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void CharCreationUpdate(PlayerChar currentChar)
    {
        this.Text = GetCharReadout(currentChar);
    }

    public string GetCharReadout(PlayerChar currentChar)
    {
        string charReadout = "", lb = "\n\n";
        PronounSet prns = currentChar.Pns;
        string pssv = prns.HerHisTheir;
        charReadout += $" --- Character Summary --- {lb}";
        charReadout += $"{pssv} name: {currentChar.Name}{lb}";
        charReadout += $"{pssv} occupation: {currentChar.Background.Name}{lb}";
        charReadout += $"etc, etc, todo...";
        return charReadout;
    }
}
