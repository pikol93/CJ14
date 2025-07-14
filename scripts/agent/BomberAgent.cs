using Godot;
using System;

namespace Pikol93.CJ14;

public partial class BomberAgent : Agent
{
    private static readonly PackedScene ExplosionScene = GD.Load<PackedScene>("res://scenes/bomber_explosion.tscn");
    private bool AlreadyExploded { get; set; } = false;

    public override AgentType GetAgentType()
    {
        return AgentType.Bomber;
    }

    protected override void PrimaryJustActivated()
    {
        Explode();
        TakeDamage(99.0, DamageType.Explosion);
    }

    protected override void OnDeath()
    {
        Explode();
    }

    private void Explode()
    {
        if (AlreadyExploded)
        {
            return;
        }

        AlreadyExploded = true;
        var explosion = ExplosionScene.Instantiate<Node2D>();
        // Move forward the explosion a bit so that it's position does not overlap with the bomber
        var forward = GlobalTransform.BasisXform(Vector2.Right);
        explosion.Position = Position + forward;
        Level.CallDeferred("add_child", explosion);
    }
}
