using Godot;

namespace Pikol93.CJ14;

public partial class Weapon : Resource
{
#pragma warning disable 0649
    [Export]
    public WeaponType Type { get; private set; }
    [Export]
    public string Name { get; private set; }
    [Export]
    public bool IsAuto { get; private set; }
    [Export]
    public float Cooldown { get; private set; }
    [Export]
    public float Damage { get; private set; }
    [Export]
    public float MuzzleVelocity { get; private set; }
    [Export]
    public float MinBulletVelocity { get; private set; }
#pragma warning restore 0649

    public override string ToString()
    {
        return $"WeaponResource: \"{Name}\"";
    }
}
