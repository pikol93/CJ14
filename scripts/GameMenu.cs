using Godot;

namespace Pikol93.CJ14;

public partial class GameMenu : CanvasLayer
{
    [Export]
    private NodePath ResetButtonPath { get; set; }
    private World World { get; set; }

    public override void _Ready()
    {
        World = this.GetAncestor<World>();
    }

    public void OnVisibilityChanged()
    {
        PauseManager.GameMenuOpen = Visible;
        PauseManager.ComputePause();

        this.GetNodeOrThrow<Button>(ResetButtonPath).Disabled = !World.CanReset();
    }

    public void OnContinuePressed()
    {
        Visible = false;
    }

    public void OnResetPressed()
    {
        World.ReloadLevel();
        Visible = false;
    }

    public void OnRestartPressed()
    {
        var world = GD.Load<PackedScene>("res://scenes/world.tscn").Instantiate<World>();
        world.LevelScene = World.LevelScene;
        SceneManager.Instance.CallDeferred("SetScene", world);
    }

    public void OnQuitPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
    }
}
