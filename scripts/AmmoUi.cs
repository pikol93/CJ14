using Godot;

namespace Pikol93.CJ14;

public partial class AmmoUi : CanvasLayer
{
    public static AmmoUi Instance { get; set; }

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public static void UpdateAmmo(int reserve, int total)
    {
        if (Instance != null)
        {
            Instance.GetNode<Label>("Label").Text = $"Ammo: {reserve} / {total}";
        }
    }
}
