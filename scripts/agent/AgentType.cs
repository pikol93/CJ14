using Godot;

namespace Pikol93.CJ14;

public enum AgentType {
    DummyAgent,
    ShooterAgent,
}

public static class AgentTypeExtensions
{
    public static PackedScene GetPackedScene(this AgentType variant)
    {
        var result = variant switch
        {
            AgentType.DummyAgent => GD.Load("res://scenes/agent/dummy.tscn"),
            AgentType.ShooterAgent => GD.Load("res://scenes/agent/shooter.tscn"),
            _ => throw new System.NotImplementedException(),
        };

        return (PackedScene)result;
    }
}