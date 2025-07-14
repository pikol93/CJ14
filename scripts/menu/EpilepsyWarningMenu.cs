using Godot;

namespace Pikol93.CJ14;

public partial class EpilepsyWarningMenu : Control, IMenuItem
{
    [Export]
    private NodePath AnimationPlayerNodePath { get; set; }
    
    private bool Closing { get; set; }

    public override void _Ready()
    {
        OnOpen();
    }

    public void OnOpen()
    {
        Closing = false;
        this.GetNodeOrNull<AnimationPlayer>(AnimationPlayerNodePath).Play("display");
    }

    public void OnClose()
    {

    }

    public override void _GuiInput(InputEvent @event)
    {
        if (Closing)
        {
            return;
        }

        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Closing = true;
                this.GetNodeOrNull<AnimationPlayer>(AnimationPlayerNodePath).Play("hide");
            }
        }
    }

    public void OnAnimationEnd()
    {
        this.GetAncestor<Menu>().ToggleMenu("Main");
    }
}
