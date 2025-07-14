using Godot;

namespace Pikol93.CJ14;

public partial class Door : StaticBody2D, IDamagable, IHittable
{
    private static readonly PackedScene WallHitParticlesScene = GD.Load<PackedScene>("res://scenes/wall_hit_particles.tscn");

    [Export] public Resource HitSoundBank { get; set; }
    [Export] public float PenetrationValue { get; set; }
    [Export] public double BaseHealth { get; set; }
    
    private Level level;
    private double health;

    public override void _Ready()
    {
        level = this.GetAncestor<Level>();
        health = BaseHealth;
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

    public float GetIntersectionLength(Vector2 lastCheckedPosition, Vector2 currentPosition)
    {
        return 4.0f;
    }

    public void TakeDamage(double damage, DamageType damageType)
    {
        GD.Print($"Taking damage: {damage}");
        health -= damage;
        if (health <= 0.0)
        {
            QueueFree();
        }
    }
}
