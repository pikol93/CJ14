using Godot;

namespace Pikol93.CJ14;

public partial class Enemy : CharacterBody2D, IDamagable
{
    public override void _Ready() {
        GD.Print("Enemy ready");
    }

    public void TakeDamage(double damage, DamageType bullet)
    {
        GD.Print($"Enemy damaged: {damage} {bullet}");
    }
}
