using Godot;
using System;

namespace Pikol93.CJ14;

public partial class AgentChoiceMenu : CanvasLayer
{
    [Export]
    private NodePath ShooterAvailableLabelPath { get; set; }
    [Export]
    private NodePath BomberAvailableLabelPath { get; set; }
    [Export]
    private NodePath ShotgunAvailableLabelPath { get; set; }
    [Export]
    private NodePath ShooterButtonPath { get; set; }
    [Export]
    private NodePath BomberButtonPath { get; set; }
    [Export]
    private NodePath ShotgunButtonPath { get; set; }
    private World World { get; set; }

    public override void _Ready()
    {
        World = this.GetAncestor<World>();
    }

    public void OnVisibilityChanged()
    {
        PauseManager.AgentChoiceActive = Visible;
        PauseManager.ComputePause();

        this.GetNodeOrThrow<Label>(ShooterAvailableLabelPath).Text = "" + World.ShootersAvailable + " available";
        this.GetNodeOrThrow<Label>(BomberAvailableLabelPath).Text = "" + World.BombersAvailable + " available";
        this.GetNodeOrThrow<Label>(ShotgunAvailableLabelPath).Text = "" + World.ShotgunsAvailable + " available";

        this.GetNodeOrThrow<Button>(ShooterButtonPath).Disabled = World.ShootersAvailable <= 0;
        this.GetNodeOrThrow<Button>(BomberButtonPath).Disabled = World.BombersAvailable <= 0;
        this.GetNodeOrThrow<Button>(ShotgunButtonPath).Disabled = World.ShotgunsAvailable <= 0;
    }

    public void OnShooterPressed()
    {
        World.ShootersAvailable--;
        World.LoadLevel(AgentType.Shooter);
    }

    public void OnBomberPressed()
    {
        World.BombersAvailable--;
        World.LoadLevel(AgentType.Bomber);
    }

    public void OnShotgunPressed()
    {
        World.ShotgunsAvailable--;
        World.LoadLevel(AgentType.Shotgun);
    }
}
