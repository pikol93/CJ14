using Godot;

namespace Pikol93.CJ14;

public enum AgentType
{
    Dummy,
    Shooter,
    Bomber,
}

public static class AgentTypeExtensions
{
    public static PackedScene GetPackedScene(this AgentType variant)
    {
        var result = variant switch
        {
            AgentType.Dummy => GD.Load("res://scenes/agent/dummy.tscn"),
            AgentType.Shooter => GD.Load("res://scenes/agent/shooter.tscn"),
            AgentType.Bomber => GD.Load("res://scenes/agent/bomber.tscn"),
            _ => throw new System.NotImplementedException(),
        };

        return (PackedScene)result;
    }
}