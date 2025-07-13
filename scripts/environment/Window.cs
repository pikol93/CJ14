using Godot;
using System;

namespace Pikol93.CJ14;

public partial class Window : StaticBody2D, IHittable, IPenetrable
{
    [Export]
    private NodePath ParticlesNodePath { get; set; }
    [Export]
    private NodePath AnimationPlayerNodePath { get; set; }
    [Export]
    private NodePath AudioPlayerNodePath { get; set; }
    [Export]
    private float BasePenetrationValue { get; set; }
    
    public float PenetrationValue { get => intact ? BasePenetrationValue : 0.0f; }

    private bool intact = true;

    public float GetIntersectionLength(Vector2 lastCheckedPosition, Vector2 currentPosition)
    {
        return intact ? 8.0f : 0.0f;
    }

    public void Hit(Vector2 hitPosition, Vector2 direction, Vector2 normal, DamageType damageType, double impact)
    {
        if (!intact)
        {
            return;
        }
        intact = false;

        var particles = GetNode<GpuParticles2D>(ParticlesNodePath);
        var animationPlayer = GetNode<AnimationPlayer>(AnimationPlayerNodePath);
        var audioPlayer = GetNode<AudioStreamPlayer2D>(AudioPlayerNodePath);
        var forward = Vector2.Down.Rotated(Rotation);
        if (direction.Dot(forward) < 0.0f)
        {
            particles.Rotation = Mathf.Pi;
        }

        particles.Emitting = true;
        animationPlayer.Play("broken");

        audioPlayer.Play();
    }
}
