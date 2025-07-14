using System;
using Godot;

namespace Pikol93.CJ14;

public partial class Enemy : CharacterBody2D, IDamagable, ITeammable
{
    [Export]
    public double BaseHealth { get; set; } = 1.0;
    [Export]
    public double BaseRareUpdateCooldown { get; set; } = 0.7;

    protected Level Level { get; private set; }
    protected double Health { get; set; } = 1.0;
    protected double RareUpdateCooldown { get; private set; }

    public Team Team => Team.Enemy;

    public bool IsAlive => Health > 0.0;

    public override void _Ready() {
        Health = BaseHealth;
        RareUpdateCooldown = Mathf.Abs(Position.X + Position.Y) % BaseRareUpdateCooldown;
        Level = this.GetAncestor<Level>();
        if (Level == null)
        {
            throw new ApplicationException("Level is null.");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
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

        Process();
    }

    public void TakeDamage(double damage, DamageType bullet)
    {
        GD.Print($"Enemy damaged: {damage} {bullet}");
        Health -= damage;
        if (!IsAlive)
        {
            CloudShaderController.Excite();
            OnDeath();
            CollisionLayer = 0;
        }
    }

    protected virtual void RareProcess()
    {
        GD.Print("Rare process!");
    }

    protected virtual void Process()
    {

    }

    protected virtual void OnDeath()
    {

    }
}
