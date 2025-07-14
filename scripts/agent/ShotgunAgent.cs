using System;
using Godot;

namespace Pikol93.CJ14;

public partial class ShotgunAgent : Agent
{
    [Export]
    private NodePath AnimationPlayerNodePath { get; set; }
    [Export]
    private NodePath MuzzleNodePath { get; set; }
    [Export]
    private SoundBank ShotgunShotSoundBank { get; set; }
    [Export]
    private int PelletCount { get; set; } = 7;
    [Export]
    private float MinRotation { get; set; } = -Mathf.DegToRad(5.0f);
    [Export]
    private float MaxRotation { get; set; } = Mathf.DegToRad(5.0f);
    [Export]
    private float MinSpeed { get; set; } = 1500.0f;
    [Export]
    private float MaxSpeed { get; set; } = 2000.0f;

    private AnimationPlayer animationPlayer;
    private Node2D muzzle;

    public override void _Ready()
    {
        base._Ready();
        animationPlayer = this.GetNodeOrThrow<AnimationPlayer>(AnimationPlayerNodePath);
        muzzle = this.GetNodeOrThrow<Node2D>(MuzzleNodePath);
    }

    public override AgentType GetAgentType()
    {
        return AgentType.Shotgun;
    }

    protected override void PrimaryJustActivated()
    {
        ShootBullet();
    }

    private void ShootBullet()
    {
        var random = new Random(0);
        var baseRotation = Rotation;
        var globalPosition = muzzle.GlobalPosition;
        Level.PlayRandomSoundAt(globalPosition, ShotgunShotSoundBank);

        for (int i = 0; i < PelletCount; i++)
        {
            var inaccuracyAngle = MinRotation + random.NextSingle() * (MaxRotation - MinRotation);
            var speed = MinSpeed + random.NextSingle() * (MaxSpeed - MinSpeed);

            var angle = baseRotation + inaccuracyAngle;
            var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            var bullet = Bullet.Create();
            bullet.currentPosition = globalPosition;
            bullet.damage = 7.0;
            bullet.velocity = direction * speed;
            bullet.minVelocity = 5.0f;
            bullet.Team = Team.Player;

            Level.CallDeferred("add_child", bullet);
            animationPlayer.Play("shoot");
        }

    }
}
