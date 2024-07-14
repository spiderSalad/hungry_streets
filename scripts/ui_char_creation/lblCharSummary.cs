using Godot;
using System;

public partial class lblCharSummary : RichTextReadout
{
    public void CharCreationUpdate(PlayerChar currentChar)
    {
        UpdateText(GetCharReadout(currentChar));
    }

    public string GetCharReadout(PlayerChar currentChar)
    {
        string charReadout = "", lb = "\n\n";
        PronounSet prns = currentChar.Pns;
        string pssv = prns.HerHisTheir.Capitalize();
        charReadout += $" --- Character Summary --- {lb}";
        charReadout += $"Name: {currentChar.Name}{lb}";
        charReadout += $"Background: {currentChar.Background.Name}{lb}";
        charReadout += $"{currentChar?.Block}\n";
        charReadout += $"{currentChar.Hp}\n{currentChar.Will}\n";
        charReadout += $"etc, etc, todo...";
        if (Cfg.DEV_MODE)
        {
            charReadout += $"\n\n{prns.ShesHesTheyve} been used in "
                + $"{currentChar.numDiceTests} dice roll"
                + (currentChar.numDiceTests != 1 ? "s": "");
        }
        charReadout += "\n";
        return charReadout;
    }
}
