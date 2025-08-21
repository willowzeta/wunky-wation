namespace Content.Shared._Funkystation.Stockpile;

// Used to store a list of items to be passed to the screen to display
[RegisterComponent]
public partial class StockpileInventoryComponent : Component
{
    [DataField]
    public List<int> Inventory = new List<int>();
}
