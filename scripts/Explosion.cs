using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ14;

public partial class Explosion : Node2D
{
    private List<Vector2> ContactPoints { get; } = [];

    [Export]
    private Team Team { get; set; } = CJ14.Team.Player;
    [Export]
    private NodePath AnimationPlayerPath { get; set; }
    [Export]
    private CircleShape2D ExplosionShape { get; set; }
    [Export]
    private double MaxExplosionDamage { get; set; } = 100.0;
    [Export]
    private bool DisplayRange { get; set; } = true;

    public override void _Ready()
    {
        var animationPlayer = GetNode<AnimationPlayer>(AnimationPlayerPath);
        animationPlayer.Play("explode");

        Hurt();
    }

    public override void _PhysicsProcess(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (!DisplayRange)
        {
            return;
        }

        if (!GameManager.DebugMode)
        {
            return;
        }

        DrawCircle(Vector2.Zero, ExplosionShape.Radius, Colors.Red, false, 2);
        foreach (var point in ContactPoints)
        {

            DrawCircle(point - Position, 2.0f, Colors.DarkRed, true);
        }
    }

    private void Hurt()
    {
        var shapeParams = new PhysicsShapeQueryParameters2D
        {
            CollideWithAreas = true,
            CollideWithBodies = true,
            Transform = Transform,
            CollisionMask = 7,
            Shape = ExplosionShape,
        };
        var results = GetWorld2D().DirectSpaceState.IntersectShape(shapeParams);
        foreach (var result in results)
        {
            var collider = (CollisionObject2D)result["collider"];
            var rid = (Rid)result["rid"];
            var obscured = IsObscured(collider, rid);
            if (obscured)
            {
                GD.Print($"Obscured: {collider.Name}");
                continue;
            }

            if (collider is ITeammable teammable)
            {
                if (teammable.Team == Team)
                {
                    continue;
                }
            }

            var (contactPoint, normal) = GetExplosionContactPointAndNormal(collider);
            var strength = GetExplosionStrength(contactPoint);
            var damage = strength * MaxExplosionDamage;

            if (collider is IDamagable damagable)
            {
                damagable.TakeDamage(damage, DamageType.Explosion);
            }

            if (collider is IHittable hittable1)
            {
                var direction = (collider.GlobalPosition - GlobalPosition).Normalized();
                hittable1.Hit(contactPoint, direction, normal, DamageType.Explosion, damage);
            }

            ContactPoints.Add(contactPoint);
        }
    }

    private bool IsObscured(CollisionObject2D collider, Rid rid)
    {
        var rayParams = new PhysicsRayQueryParameters2D
        {
            CollideWithAreas = true,
            CollideWithBodies = true,
            From = Position,
            To = collider.GlobalPosition,
            CollisionMask = 3,
            Exclude = [rid],
        };
        var result = GetWorld2D().DirectSpaceState.IntersectRay(rayParams);
        var obscured = result.Count > 0 && (CollisionObject2D)result["collider"] != collider;
        return obscured;
    }

    private (Vector2, Vector2) GetExplosionContactPointAndNormal(CollisionObject2D collider)
    {
        Godot.Collections.Array<Rid> exclude = [];
        while (true)
        {
            var rayParams = new PhysicsRayQueryParameters2D
            {
                CollideWithAreas = true,
                CollideWithBodies = true,
                From = Position,
                To = collider.GlobalPosition,
                CollisionMask = 7,
                Exclude = exclude,
                HitFromInside = true,
            };
            var result = GetWorld2D().DirectSpaceState.IntersectRay(rayParams);
            if (result.Count == 0) {
                GD.PrintErr("Could not find the target collider. This should never happen.");
                break;
            }

            var rayCollider = (CollisionObject2D)result["collider"];
            if (rayCollider == collider)
            {
                var position = (Vector2)result["position"];
                var normal = (Vector2)result["normal"];
                return (position, normal);
            }

            exclude.Add((Rid)result["rid"]);
        }

        return (collider.Position, Vector2.Zero);
    }

    private float GetExplosionStrength(Vector2 position)
    {
        var radius = ExplosionShape.Radius;
        var radiusSquared = radius * radius;
        var distanceSquared = Position.DistanceSquaredTo(position);
        var strengthInverse = distanceSquared / radiusSquared;
        return 1.0f - strengthInverse;
    }
}
