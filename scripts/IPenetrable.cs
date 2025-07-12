using Godot;

namespace Pikol93.CJ14;

public interface IPenetrable
{
    float PenetrationValue { get; }

    float GetIntersectionLength(Vector2 lastCheckedPosition, Vector2 currentPosition);
}
