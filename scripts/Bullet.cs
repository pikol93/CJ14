using Godot;
using System.Collections.Generic;

namespace Pikol93.CJ14;

public partial class Bullet : Line2D
{
    private const uint CollisionMask = 7;
    private const int MaxCollisionsPerFrame = 100;
    private const float BulletLifetime = 2f;
    private const float TrailLifetime = 0.1f;

    private static readonly PackedScene Scene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet.tscn");

    public double damage;
    public Vector2 velocity;
    // Bullet with no set minVelocity will immediately be deleted
    public double minVelocity = Mathf.Inf;

    private Vector2 lastCheckedPosition;
    public Vector2 currentPosition;

    private double lifeTimer;
    private readonly List<double> trailTimers = [];
    private Godot.Collections.Array<Rid> ignoredObjects = [];

    public static Bullet Create()
    {
        return (Bullet)Scene.Instantiate();
    }

    public override void _EnterTree()
    {
        lastCheckedPosition = currentPosition;
        AppendPoint(currentPosition);
    }

    public override void _Process(double delta)
    {
        // Cease bullet collision detection after timeout
        lifeTimer += delta;
        if (lifeTimer > BulletLifetime)
            SetPhysicsProcess(false);

        // Reversed for loop to delete the trail
        for (int i = trailTimers.Count - 1; i >= 0; i--)
        {
            trailTimers[i] -= delta;
            if (trailTimers[i] < 0f)
            {
                trailTimers.RemoveAt(i);
                RemovePoint(i);
            }
        }

        // Free after the trail has faded
        if (!IsPhysicsProcessing() && Points.Length < 1)
            QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        // TODO: Shorten this method
        // Update the position
        bool stop = false;

        lastCheckedPosition = currentPosition;
        // TODO: Fix this
        currentPosition += velocity * (float)delta;

        QueueRedraw();

        int collisionCounter = 0;
        while (!stop && collisionCounter < MaxCollisionsPerFrame)
        {
            collisionCounter++;

            var rayParams = new PhysicsRayQueryParameters2D
            {
                CollisionMask = CollisionMask,
                Exclude = ignoredObjects,
                CollideWithBodies = true,
                CollideWithAreas = true,
                From = lastCheckedPosition,
                To = currentPosition,
            };
            // Check for collisions
            Godot.Collections.Dictionary result = GetWorld2D()
                .DirectSpaceState
                .IntersectRay(rayParams);

            // No results
            if (result.Count <= 0)
                break;

            var collider = (CollisionObject2D)result["collider"];
            var position = (Vector2)result["position"];

            ignoredObjects.Add(collider.GetRid());

            if (collider is IDamagable damagable)
            {
                damagable.TakeDamage(damage, DamageType.Bullet);
            }

            if (collider is IPenetrable penetrable)
            {
                // Reduce the velocity by returned number
                float intersectionLength = penetrable.GetIntersectionLength(lastCheckedPosition, currentPosition);
                float velocityLength = velocity.Length();
                float newVelocityLength = velocityLength - (intersectionLength * penetrable.PenetrationValue);
                velocity = velocity / velocityLength * newVelocityLength;

                // Stop the bullet once below velocity threshold
                if (newVelocityLength < minVelocity)
                {
                    currentPosition = position;
                    stop = true;
                }
            }
            else
            {
                // Hit something that's not penetrable
                currentPosition = position;
                stop = true;
            }
        }

        AppendPoint(currentPosition);

        if (stop)
        {
            SetPhysicsProcess(false);
        }
    }

    public override void _Draw()
    {
        if (!GameManager.DebugMode)
            return;

        DrawLine(lastCheckedPosition, currentPosition, new Color(1f, 0f, 0f, 1f), Width / 2f);
    }

    private void AppendPoint(Vector2 at)
    {
        AddPoint(at);
        trailTimers.Add(TrailLifetime);
    }

    public void Destroy() => QueueFree();
}
