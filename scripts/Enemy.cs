using Godot;

namespace Pikol93.CJ14;

public partial class Enemy : CharacterBody2D, IDamagable, ITeammable
{
    public Team Team => Team.Enemy;

    public override void _Ready() {

    }

    public void TakeDamage(double damage, DamageType bullet)
    {
        GD.Print($"Enemy damaged: {damage} {bullet}");
    }
}
