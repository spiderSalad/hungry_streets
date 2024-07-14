using System;
using Godot;

public static partial class Utils
{
    public static void PopulateStatDict(
        V5Stat[] statArray, System.Collections.Generic.Dictionary<string, V5Stat> statDict
    )
    {
        foreach (V5Stat stat in statArray)
        {
            statDict.Add(stat.Id, stat);
        }
    }

    public static string GetFullSavePath(string fileName)
    {
        return $"{Cfg.PATH_SAVE_STATES}{fileName}{Cfg.PATH_SAVE_EXT}";
    }

    public static string SanitizeStr(string input) // TODO: Implement this.
    {
        return $"{input}";
    }

    public static int FindNthOccurrence(string input, string substr, int n = 1)
    {
        // GD.Print($"Find Nth: called looking for occurrence #{n} of '{substr}' in '{input}'");

        if (n < 1) {
            throw new ArgumentOutOfRangeException($"Utils.FindNthOccurrence(): n = {n}, but must be 1 or more.");
        }

        int foundIndex, occurrencesFound = 0, placeholder = 0;
        do
        {
            foundIndex = input.IndexOf(substr, placeholder);
            if (foundIndex < 0) {
                break;
            }
            occurrencesFound++;
            placeholder = foundIndex + 1;
        } while (occurrencesFound < n && foundIndex + 1 < input.Length);

        if (occurrencesFound == n) {
            GD.Print($"Find Nth: found {n}th occurrence at index {foundIndex}");
            return foundIndex;
        }
        if (occurrencesFound > n && n > 0) {
            throw new Exception(
                $"Utils.FindNthOccurrence(): found {occurrencesFound} occurrences, but only {n} were asked for! This shouldn't happen."
            );
        }
        // GD.Print($"Find Nth: did NOT find a(n) {n}th occurrence! Only found {occurrencesFound}");
        return -1;
    }

    public static string InjectSepEveryNthMark(string operand, string sep, int n, string mark)
    {
        int nthPlace, placeholder = 0;
        do
        {
            nthPlace = Utils.FindNthOccurrence(operand.Substring(placeholder), sep, n);
            if (nthPlace > -1)
            {
                operand = string.Concat(
                    operand.AsSpan(0, placeholder + nthPlace + sep.Length),
                    mark,
                    operand.AsSpan(placeholder + nthPlace + sep.Length)
                );
                placeholder += mark.Length;
            }
            placeholder += nthPlace + sep.Length;
        } while (nthPlace > -1 && placeholder < operand.Length);

        return operand;
    }

    public static string Array2String<T>(
        T[] testArray,
        bool prepend = true,
        Func<T, string> toStringAltFunc = null
    )
    {
        string arrayString = "";
        for (int i = 0; i < testArray.Length; i++)
        {
            arrayString += toStringAltFunc is null ? $"{testArray[i]}" : toStringAltFunc(testArray[i]);
            if (i < testArray.Length - 1)
            {
                arrayString += ", ";
            }
        }
        if (prepend)
        {
            arrayString = $"Array2String<{typeof(T)}>():\n{arrayString}";
        }
        return arrayString;
    }

    public static string Array2String2d<T>(
        T[,] array2d,
        bool prepend = true,
        Func<T, string> toStringAltFunc = null
    )
    {
        string arrayString2d = "";
        int rows = array2d.GetLength(0), cols = array2d.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                arrayString2d += toStringAltFunc is null ? $"{array2d[i, j]}" : toStringAltFunc(array2d[i, j]);
                if (j < cols - 1)
                {
                    arrayString2d += ", ";
                }
            }
            if (i < rows - 1)
            {
                arrayString2d += "\n";
            }
        }
        if (prepend)
        {
            arrayString2d = $"Array2String2D<{typeof(T)}>():\n{arrayString2d}";
        }
        return arrayString2d;
    }

    public static T[] Transpose1dGridArray<T>(T[] origArr, int numRows)
    {
        if (origArr.Length < numRows)
        {
            throw new ArgumentException($"Operand array length ({origArr.Length}) must be >= numRows ({numRows})!");
        }
        //GD.Print($"\n1d test | original array: {Utils.Array2String<T>(origArr)}\n");
        T[] resultArr = new T[origArr.Length];
        int numCols = origArr.Length / numRows;
        for (int i = 0; i < origArr.Length; i++)
        {
            int tindexRowAdjust = (i % numRows) * numCols;
            int tindexColAdjust = i / numRows; // NOTE: Integer division
            resultArr[tindexRowAdjust + tindexColAdjust] = origArr[i];
            //GD.Print(
            //    $"original[{i}] (value={origArr[i]}) --> " +
            //    $"new[{tindexRowAdjust} + {tindexColAdjust}] --> " +
            //    $"new[{tindexRowAdjust + tindexColAdjust}]"
            //);
        }
        //GD.Print($"\n1d test | new array: {Utils.Array2String<T>(resultArr)}\n");
        return resultArr;
    }

    public static void AddNodeToGroups(Node n, Godot.Collections.Array<Cfg.GROUP_NAMES> groups)
    {
        try
        {
            foreach (var NodeGroup in groups)
            {
                n.AddToGroup($"{NodeGroup}");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr(
                $"Utils.AddNodeToGroups<{n.GetType()}>: Failed to add node to groups :: {ex}"
            );
            throw;
        }
    }

    public static HBoxContainer CreateDotStatNode(V5Stat stat, int statValue)
    {
        HBoxContainer hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 10);
        hbox.SetMeta(Cfg.KEY_ID, stat.Id);
        // hbox.SetMeta(Cfg.META_STATNAME, stat.Name);
        hbox.SetMeta(Cfg.KEY_STATVALUE, statValue);
        // hbox.SetMeta(Cfg.META_STATDESC, stat.Desc);
        hbox.TooltipText = $"{stat.Name} of {statValue}{(stat.Desc != "" ? "\n" : "")}{stat.Desc}";

        Label attrLabel = new Label();
        attrLabel.Text = stat.Name;
        attrLabel.AddThemeFontSizeOverride("font_size", 16);

        TextureRect dots = new TextureRect();
        dots.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
        dots.Texture = GD.Load<Texture2D>($"res://images/gui/dots_red_{Cfg.NUM_WORDS[statValue]}.png");

        hbox.AddChild(attrLabel);
        hbox.AddChild(dots);
        return hbox;
    }
}

public abstract partial class FormInputLine : LineEdit
{
    [Export] protected Godot.Collections.Array<Cfg.GROUP_NAMES> NodeGroups;
    [Export] protected Cfg.UI_KEY UiCommandKey;

    [Signal]
    public delegate void FormTextInputEventHandler(Cfg.UI_KEY source, string text);

    public override void _Ready()
    {
        Utils.AddNodeToGroups(this, NodeGroups);
        this.TextSubmitted += OnTextEntered;
        this.FocusExited += OnFocusLost;
    }

    public void OnTextEntered(string newText)
    {
        this.EmitSignal(SignalName.FormTextInput, (int)UiCommandKey, Utils.SanitizeStr(newText));
    }

    public void OnFocusLost()
    {
        this.EmitSignal(SignalName.FormTextInput, (int)UiCommandKey, Utils.SanitizeStr(this.Text));
    }
}

public abstract partial class StatGridDisplay : GridContainer
{
    private bool _contentsInitialized = false;

    [Export] protected Godot.Collections.Array<Cfg.GROUP_NAMES> NodeGroups;

    protected abstract V5Stat[] statArray { get; }
    protected abstract System.Collections.Generic.Dictionary<string, V5Stat> statDict { get; }
    protected abstract int GetStatValue(V5Entity currentChar, V5Stat stat);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddThemeConstantOverride("h_separation", 10);
        this.AddThemeConstantOverride("v_separation", 5);
        Utils.AddNodeToGroups(this, NodeGroups);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void CharCreationUpdate(V5Entity currentChar)
    {
        int statValue;
        if (_contentsInitialized)
        {
            Godot.Collections.Array<Node> childNodes = this.GetChildren();
            foreach (Node childNode in childNodes)
            {
                Control dotStatNode = (Control)childNode;
                // V5Stat stat = Cfg.AttrsDict[(string)dotStatNode.GetMeta(Cfg.KEY_ID)];
                V5Stat stat = statDict[(string)dotStatNode.GetMeta(Cfg.KEY_ID)];
                // statValue = currentChar.Block.GetAttr(stat.Id);
                statValue = GetStatValue(currentChar, stat);
                dotStatNode.SetMeta(Cfg.KEY_STATVALUE, statValue);
                dotStatNode.TooltipText =
                    $"{stat.Name} of {statValue}{(stat.Desc != "" ? "\n" : "")}{stat.Desc}";
                foreach (Node subNode in dotStatNode.GetChildren())
                {
                    if (subNode is TextureRect dotImg)
                    {
                        dotImg.Texture = GD.Load<Texture2D>(
                            $"res://images/gui/dots_red_{Cfg.NUM_WORDS[statValue]}.png"
                        );
                    }
                }
            }
        }
        else
        {
            foreach (V5Stat stat in statArray)
            {
                statValue = GetStatValue(currentChar, stat);
                this.AddChild(Utils.CreateDotStatNode(stat, statValue));
            }
            _contentsInitialized = true;
        }
    }
}