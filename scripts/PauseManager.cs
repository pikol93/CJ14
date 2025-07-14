using Godot;

namespace Pikol93.CJ14;

public partial class PauseManager : Node
{
    private static PauseManager Instance { get; set; }

    public static bool AgentChoiceActive { get; set; }
    public static bool GameMenuOpen { get; set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public static void ComputePause()
    {
        var shouldPause = AgentChoiceActive || GameMenuOpen;
        Instance.GetTree().Paused = shouldPause;
    }
}