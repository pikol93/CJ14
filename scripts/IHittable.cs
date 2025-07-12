using Godot;

namespace Pikol93.CJ14;

public interface IHittable
{

    void Hit(Vector2 hitPosition, Vector2 direction, Vector2 normal, DamageType damageType, double impact);
}
