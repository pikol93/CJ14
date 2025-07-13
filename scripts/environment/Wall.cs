using Godot;
using System;
using System.Collections.Generic;

namespace Pikol93.CJ14;

[Tool]
public partial class Wall : StaticBody2D, IHittable, IPenetrable
{
    private const float WIDTH = 8f;
    private const float HALF_WIDTH = WIDTH / 2f;
    private static readonly PackedScene WallHitParticlesScene = GD.Load<PackedScene>("res://scenes/wall_hit_particles.tscn");

    private Sprite2D sprite;
    private Sprite2D joint1;
    private Sprite2D joint2;
    private CollisionShape2D collider;
    private LightOccluder2D occluder;
    private Level level;

    private bool isRotated;
    private float length;
    
    [Export] public Resource HitSoundBank { get; set; }
    [Export] public float PenetrationValue { get; set; }

    [Export]
    public bool IsRotated
    {
        get { return isRotated; }
        set
        {
            isRotated = value;
            Rotation = value ? Mathf.Pi / 2f : 0f;
        }
    }

    [Export]
    public float Length
    {
        get { return length; }
        set
        {
            // Set minimum width
            if (value < WIDTH)
                value = WIDTH;

            value -= value % HALF_WIDTH;

            length = value;

            if (sprite is null)
                return;

            UpdateWall();
        }
    }

    public float HalfLength { get => length / 2; }

    public override void _Ready()
    {
        sprite = this.GetNodeOrThrow<Sprite2D>("Sprite");
        joint1 = this.GetNodeOrThrow<Sprite2D>("Joint1");
        joint2 = this.GetNodeOrThrow<Sprite2D>("Joint2");
        collider = this.GetNodeOrThrow<CollisionShape2D>("Collider");
        occluder = this.GetNodeOrThrow<LightOccluder2D>("Occluder");
        if (!Engine.IsEditorHint()) {
            level = this.GetAncestor<Level>();
        }

        UpdateWall();
    }

    private void UpdateWall()
    {
        sprite.RegionEnabled = true;
        sprite.RegionRect = new Rect2(0f, 0f, Length, WIDTH);

        joint1.Position = new Vector2(-HalfLength + HALF_WIDTH, 0f);
        joint2.Position = new Vector2(HalfLength - HALF_WIDTH, 0f);

        collider.Shape = new RectangleShape2D()
        {
            Size = new Vector2(Length, WIDTH),
        };

        occluder.Occluder = new OccluderPolygon2D()
        {
            Polygon =
            [
                    new(-HalfLength, -HALF_WIDTH),
                    new(HalfLength, -HALF_WIDTH),
                    new(HalfLength, HALF_WIDTH),
                    new(-HalfLength, HALF_WIDTH)
            ]
        };
    }

    public void Hit(Vector2 hitPosition, Vector2 direction, Vector2 normal, DamageType damageType, double impact)
    {
        if (damageType != DamageType.Bullet)
            return;

        level.PlayRandomSoundAt(hitPosition, (SoundBank)HitSoundBank);

        Vector2 bouncedVector = direction - 2 * direction.Dot(normal) * normal;
        var particles = WallHitParticlesScene.Instantiate<GpuParticles2D>();
        particles.Emitting = true;
        particles.Amount = Mathf.CeilToInt(impact / 10.0);

        var material = (ParticleProcessMaterial)particles.ProcessMaterial;
        material.Direction = new Vector3(bouncedVector.X, bouncedVector.Y, 0f);
        level.SpawnParticles(hitPosition, particles);
    }

    public Vector2 GetClosestPoint(Vector2 value)
    {
        float minX;
        float maxX;
        float minY;
        float maxY;
        if (IsRotated)
        {
            minX = Position.X - HalfLength;
            maxX = Position.X + HalfLength;
            minY = Position.Y - HALF_WIDTH;
            maxY = Position.Y + HALF_WIDTH;
        }
        else
        {
            minX = Position.X - HALF_WIDTH;
            maxX = Position.X + HALF_WIDTH;
            minY = Position.Y - HalfLength;
            maxY = Position.Y + HalfLength;
        }

        return new Vector2(Mathf.Clamp(value.X, minX, maxX), Mathf.Clamp(value.Y, minY, maxY));
    }

    public float GetIntersectionLength(Vector2 from, Vector2 to)
    {
        // Get AABB intersection
        float lengthOfVector = (to - from).Length();
        Vector2 colliderExtents = ((RectangleShape2D)collider.Shape).Size / 2;

        // Get plane coordinates
        float planeBottom = Position.Y;
        float planeTop = Position.Y;
        if (isRotated)
        {
            planeBottom += colliderExtents.X;
            planeTop -= colliderExtents.X;
        }
        else
        {
            planeBottom += colliderExtents.Y;
            planeTop -= colliderExtents.Y;
        }

        float planeLeft = Position.X;
        float planeRight = Position.X;
        if (isRotated)
        {
            planeLeft -= colliderExtents.Y;
            planeRight += colliderExtents.Y;
        }
        else
        {
            planeLeft -= colliderExtents.X;
            planeRight += colliderExtents.X;
        }

        var fractions = new List<float>()
            {
                (planeLeft - from.X) / (to.X - from.X), (planeRight - from.X) / (to.X - from.X),
                (planeBottom - from.Y) / (to.Y - from.Y), (planeTop - from.Y) / (to.Y - from.Y)
            };
        fractions.Sort();

        // Common interval for 2 intervals is equal to <fraction[1], fraction[2]> in this case
        return lengthOfVector * Mathf.Clamp(fractions[2] - fractions[1], 0f, 1f);
    }
}
