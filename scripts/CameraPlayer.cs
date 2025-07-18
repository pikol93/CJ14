using Godot;
using System;
using System.Linq;

namespace Pikol93.CJ14;

public partial class CameraPlayer : Camera2D
{
    private const float MaxDistanceScreen = 50f;
    private const float MaxDistance = 50f;

    private bool controller;

    private Vector2 currentPosition;
    private Vector2 targetPosition;
    public Node2D player;

    public override void _Ready()
    {
        var cameraNodes = GetTree().Root.GetDescendants<CameraPlayer>();
        if (cameraNodes.Count() > 1) {
            GD.PrintErr("Multiple game cameras detected. Dumping scene tree.");
            GetTree().Root.PrintTreePretty();
        }
        
        Zoom = new Vector2(2, 2);
    }

    public override void _Process(double delta)
    {
        if (player == null)
        {
            return;
        }
        
        targetPosition = controller ?
            GetTargetPositionController() : GetTargetPositionMouse();

        currentPosition = currentPosition.Lerp(targetPosition, 0.1f);
        GlobalPosition = player.GlobalPosition + currentPosition;
    }

    private static Vector2 GetTargetPositionController()
    {
        Vector2 input = new(Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left"),
            Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up"));

        if (input.LengthSquared() > 1f)
            input = input.Normalized();

        return input * MaxDistance;
    }

    private static Vector2 GetTargetPositionMouse()
    {
        Vector2 direction = GameManager.GetMouseRelativeToCenter() / MaxDistanceScreen;
        if (direction.LengthSquared() > 1f)
            direction = direction.Normalized();

        return direction * MaxDistance;
    }

}
