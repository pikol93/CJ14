using Godot;

namespace Pikol93.CJ14;

public partial class LevelSelectMenu : Control
{
    private static readonly PackedScene WorldScene = GD.Load<PackedScene>("res://scenes/world.tscn");

    public void OnLevel1ButtonPressed()
    {
        LoadLevel("res://scenes/level/level1.tscn");
    }

    public void OnLevel2ButtonPressed()
    {
        LoadLevel("res://scenes/level/level2.tscn");
    }

    public void OnBackButtonPressed()
    {
        this.GetAncestor<Menu>().ToggleMenu("Main");
    }

    private void LoadLevel(string path)
    {
        var world = WorldScene.Instantiate<World>();
        world.LevelScene = GD.Load<PackedScene>(path);
        SceneManager.Instance.CallDeferred("SetScene", world);
    }
}
