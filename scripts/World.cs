using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;
using Microsoft.VisualBasic;

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
    private CameraPlayer Camera { get; set; }
    private Level Level { get; set; }
    private Agent PlayerAgent { get; set; }
    private CanvasLayer AgentChoiceLayer { get; set; }
    private CanvasLayer YouAreDeadLayer { get; set; }
    private CanvasLayer GameMenuLayer { get; set; }

    public int ShootersAvailable { get; set; }
    public int BombersAvailable { get; set; }
    public int ShotgunsAvailable { get; set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        AgentChoiceLayer = GetNode<CanvasLayer>("AgentChoice");
        YouAreDeadLayer = GetNode<CanvasLayer>("YouAreDead");
        GameMenuLayer = GetNode<CanvasLayer>("GameMenu");

        Camera = new CameraPlayer()
        {
            Enabled = true,
            ProcessCallback = Camera2D.Camera2DProcessCallback.Physics,
        };
        AddChild(Camera);

        var level = LevelScene.Instantiate<Level>();
        ShootersAvailable = level.ShootersAvailable;
        BombersAvailable = level.BombersAvailable;
        ShotgunsAvailable = level.ShotgunsAvailable;
        level.Free();

        AgentChoiceLayer.Visible = true;
        GameMenuLayer.Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        var hideYouAreDead = PlayerAgent != null && PlayerAgent.IsAlive();
        YouAreDeadLayer.Visible = !hideYouAreDead;
    }

    public override void _Input(InputEvent ie)
    {
        if (ie is InputEventKey iek)
        {
            if (iek.Pressed)
            {
                switch (iek.PhysicalKeycode)
                {
                    // Reload
                    case Key.R:
                        ReloadLevel();
                        break;
                    case Key.Escape:
                        GameMenuLayer.Visible = true;
                        break;
                }
            }
        }
    }

    public void ReloadLevel()
    {
        if (!CanReset())
        {
            return;
        }
        CommitAndUnloadLevel();
        AgentChoiceLayer.Visible = true;
    }

    public bool CanReset()
    {
        return ShootersAvailable > 0 || BombersAvailable > 0 || ShotgunsAvailable > 0;
    }

    private void CommitAndUnloadLevel()
    {
        if (Level == null)
        {
            GD.PrintErr("Cannot unload level since no level is loaded.");
            return;
        }

        Camera.player = null;

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

    public void LoadLevel(AgentType playerAgentType)
    {
        AgentChoiceLayer.Visible = false;
        Level = LevelScene.Instantiate<Level>();
        if (Level.ProcessMode != ProcessModeEnum.Pausable)
        {
            GD.PrintErr("Level {} is not pausable. This may cause issues.");
        }
        var startingPosition = ExtractStartingPosition(Level);

        foreach (var history in AgentHistories)
        {
            var agent = history.agentType.GetPackedScene().Instantiate<Agent>();
            agent.Position = startingPosition;
            agent.Controller = new ReplayController(history.frames);
            Level.AddChild(agent);
        }

        PlayerAgent = playerAgentType.GetPackedScene().Instantiate<Agent>();
        PlayerAgent.Position = startingPosition;
        PlayerAgent.Controller = new PlayerController(PlayerAgent);
        Level.AddChild(PlayerAgent);

        Camera.player = PlayerAgent;

        CallDeferred("add_child", Level);
    }

    private static Vector2 ExtractStartingPosition(Node level)
    {
        var startingPositionNode = level.FindChild("PlayerSpawn") as Node2D;
        return startingPositionNode?.Position ?? Vector2.Zero;
    }
}
