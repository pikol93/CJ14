using System;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ14;

class AgentHistory
{
    public AgentType agentType;
    public List<InputFrame> frames;
}

public partial class World : Node
{
    public PackedScene LevelScene { get; set; }
    private List<AgentHistory> AgentHistories { get; } = [];
    private Node2D Level { get; set; }
    private Agent PlayerAgent { get; set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        LoadLevel();
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
                    case Key.R:
                        ReloadLevel();
                        break;
                    case Key.P:
                        GetTree().Paused = !GetTree().Paused;
                        break;
                }
            }
        }
    }

    private void ReloadLevel()
    {
        GetTree().Paused = true;
        CommitAndUnloadLevel();
        LoadLevel();
    }

    private void CommitAndUnloadLevel()
    {
        if (Level == null)
        {
            GD.PrintErr("Cannot unload level since no level is loaded.");
            return;
        }

        if (PlayerAgent is not null)
        {
            PlayerAgent.GetAgentType();
            if (PlayerAgent.Controller is PlayerController playerController)
            {
                AgentHistories.Add(new AgentHistory
                {
                    agentType = PlayerAgent.GetAgentType(),
                    frames = playerController.Frames,
                });
            }
        }

        RemoveChild(Level);
        Level.QueueFree();
        Level = null;
    }

    private void LoadLevel()
    {
        Level = LevelScene.Instantiate<Node2D>();
        foreach (var history in AgentHistories)
        {
            var agent = history.agentType.GetPackedScene().Instantiate<Agent>();
            agent.Controller = new ReplayController(history.frames);
            Level.AddChild(agent);
        }

        PlayerAgent = AgentType.ShooterAgent.GetPackedScene().Instantiate<Agent>();
        PlayerAgent.Controller = new PlayerController(PlayerAgent);
        Level.AddChild(PlayerAgent);

        CallDeferred("add_child", Level);
    }
}
