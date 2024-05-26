using Godot;

public partial class itemListCharBackgrounds : ItemList
{
    [Signal]
    public delegate void FormSelectionEventHandler(string source, long selectionIndex);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddToGroup($"{Cfg.GROUP_NAMES.CCUI_FORM_INPUTS}");

        // this.ItemActivated += OnItemActivated;
        // this.ItemClicked += OnItemClicked;
        this.ItemSelected += OnItemSelected;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void OnItemSelected(long index)
    {
        this.EmitSignal(SignalName.FormSelection, (int)Cfg.UI_KEY.BACKGROUND_SELECT, index);
    }
}
