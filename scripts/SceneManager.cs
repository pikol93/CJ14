using Godot;

namespace Pikol93.CJ14;

public partial class SceneManager : Node
{
    public static SceneManager Instance { get; private set; }

    public override void _Ready() 
    {
        Instance = this;
    }

    public override void _Input(InputEvent ie)
    {
        if (ie is InputEventKey iek)
        {
            if (iek.Pressed)
            {
                switch (iek.PhysicalKeycode)
                {
                    // Go to main menu
                    case Key.F7:
                        GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
                        break;
                }
            }
        }
    }

    public static void SetScene(Node node)
    {
        Instance.SetSceneInternal(node);
    }

    private void SetSceneInternal(Node node)
    {
        var tree = GetTree();
        tree.UnloadCurrentScene();
        tree.Root.AddChild(node);
        tree.CurrentScene = node;
    }
}
