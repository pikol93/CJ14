using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Godot;

namespace Pikol93.CJ14;

public partial class Enemy : CharacterBody2D, IDamagable, ITeammable
{
    [Export]
    public double BaseHealth { get; set; } = 1.0;
    [Export]
    public double BaseRareUpdateCooldown { get; set; } = 0.7;
    [Export]
    public float MovementSpeed { get; set; } = 200.0f;
    [Export]
    public float TurnSpeed { get; set; } = Mathf.DegToRad(60.0f);
    [Export]
    public float AcceptableRotationDifference { get; set; } = Mathf.DegToRad(3.0f);
    [Export]
    public double BaseAttackCooldown { get; set; } = 0.2;
    [Export]
    private NodePath AnimationPlayerNodePath { get; set; }
    [Export]
    private NodePath MuzzleNodePath { get; set; }
    [Export]
    private SoundBank PistolShotSoundBank { get; set; }

    protected Level Level { get; private set; }
    protected double Health { get; set; } = 1.0;
    protected double RareUpdateCooldown { get; private set; }

    public Team Team => Team.Enemy;

    public bool IsAlive => Health > 0.0;
    public Agent TargetAgent;
    public List<Vector2> Path;
    private float targetRotation;
    private double attackCooldown;
    private AnimationPlayer animationPlayer;
    private Node2D muzzle;

    public override void _Ready()
    {
        animationPlayer = this.GetNodeOrThrow<AnimationPlayer>(AnimationPlayerNodePath);
        muzzle = this.GetNodeOrThrow<Node2D>(MuzzleNodePath);
        Health = BaseHealth;
        RareUpdateCooldown = Mathf.Abs(Position.X + Position.Y) % BaseRareUpdateCooldown;
        Level = this.GetAncestor<Level>();
        if (Level == null)
        {
            throw new ApplicationException("Level is null.");
        }
        targetRotation = Rotation;
    }

    public override void _PhysicsProcess(double delta)
    {
        QueueRedraw();
        if (!IsAlive)
        {
            return;
        }

        RareUpdateCooldown -= delta;
        if (RareUpdateCooldown <= 0.0)
        {
            RareUpdateCooldown += BaseRareUpdateCooldown;
            RareProcess();
        }

        attackCooldown = Mathf.Max(attackCooldown - delta, 0.0);

        Process(delta);
    }

    public override void _Draw()
    {
        if (!GameManager.DebugMode)
        {
            return;
        }

        DrawPolyline(Path.Select(item => item * GlobalTransform).ToArray(), Colors.Red, 2, false);
    }

    public void TakeDamage(double damage, DamageType bullet)
    {
        GD.Print($"Enemy damaged: {damage} {bullet}");
        Health -= damage;
        Level.SpawnBloodSplatters(Position, (int)(damage / 5));
        if (!IsAlive)
        {
            CloudShaderController.Excite();
            OnDeath();
            CollisionLayer = 0;
        }
    }

    protected virtual void RareProcess()
    {
        var lastTarget = TargetAgent;
        var agents = FindVisibleAgentsInFront();
        if (agents.Count > 0)
        {
            TargetAgent = agents[0];
        }
        else
        {
            TargetAgent = null;
            if (lastTarget != null)
            {
                Path = Level.FindPath(GlobalPosition, lastTarget.GlobalPosition);
            }
        }

        if (TargetAgent != null)
        {
            if (!TargetAgent.IsAlive())
            {
                TargetAgent = null;
            }
        }
    }

    protected virtual void Process(double delta)
    {
        if (TargetAgent == null)
        {
            if (Path != null && Path.Count != 0)
            {
                var nextPoint = Path[0];
                var difference = nextPoint - GlobalPosition;
                var distance = difference.Length();
                if (distance > 0.5f)
                {
                    var possibleMovementSpeedThisFrame = 1.0f / Engine.PhysicsTicksPerSecond * MovementSpeed;
                    var multiplier = Mathf.Min(distance / possibleMovementSpeedThisFrame, 1.0f);
                    var movementSpeedThisFrame = MovementSpeed * (float)multiplier;
                    var direction = difference / distance;

                    Velocity = direction * movementSpeedThisFrame;
                    GD.Print($"Velocity: {Velocity}, {difference}, {distance} {possibleMovementSpeedThisFrame} {multiplier} {movementSpeedThisFrame} {direction}");
                    MoveAndSlide();
                    targetRotation = Mathf.Atan2(direction.Y, direction.X);
                }
                else
                {
                    Path.RemoveAt(0);
                }
                if (GlobalPosition.DistanceTo(nextPoint) < 2f)
                {
                }
            }
        }
        else
        {
            var targetPosition = TargetAgent.GlobalPosition;
            var direction = targetPosition - GlobalPosition;
            targetRotation = Mathf.Atan2(direction.Y, direction.X);

            if (Mathf.AngleDifference(Rotation, targetRotation) < AcceptableRotationDifference)
            {
                if (attackCooldown <= 0.0)
                {
                    attackCooldown = BaseAttackCooldown;
                    ShootBullet();
                }
            }
        }


        Rotation = Mathf.RotateToward(Rotation, targetRotation, (float)delta * TurnSpeed);
    }

    protected virtual void OnDeath()
    {

    }

    private void ShootBullet()
    {
        var globalPosition = muzzle.GlobalPosition;
        Level.PlayRandomSoundAt(globalPosition, PistolShotSoundBank);

        var angle = Rotation;
        var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        var bullet = Bullet.Create();
        bullet.currentPosition = globalPosition;
        bullet.damage = 7.0;
        bullet.velocity = direction * 4000.0f;
        bullet.minVelocity = 5.0f;
        bullet.Team = Team.Enemy;

        Level.CallDeferred("add_child", bullet);
        animationPlayer.Play("shoot");
    }

    protected IEnumerable<Agent> FindAgents()
    {
        return GetTree().FindNodesInGroup<Agent>("Agent").Where(item => item.IsAlive());
    }

    protected List<Agent> FindVisibleAgents()
    {
        List<Agent> result = [];
        var directSpaceState = GetWorld2D().DirectSpaceState;
        foreach (var agent in FindAgents())
        {
            var rayParams = new PhysicsRayQueryParameters2D
            {
                From = GlobalPosition,
                To = agent.GlobalPosition,
                CollideWithAreas = true,
                CollideWithBodies = true,
                CollisionMask = 1,
            };

            var raycastResult = directSpaceState.IntersectRay(rayParams);
            if (raycastResult.Count == 0)
            {
                result.Add(agent);
            }
        }

        return result;
    }

    protected List<Agent> FindVisibleAgentsInFront()
    {
        List<Agent> result = [];
        foreach (var agent in FindVisibleAgents())
        {
            var forward = GlobalTransform.BasisXform(Vector2.Right);
            var inFront = forward.Dot(agent.GlobalPosition - GlobalPosition) > 0;
            if (inFront)
            {
                GD.Print($"Agent {agent.Name} visible by enemy {Name}");
                result.Add(agent);
            }
        }

        return result;
    }
}
