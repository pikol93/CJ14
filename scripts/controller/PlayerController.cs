using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ14;

public partial class PlayerController(Agent agent) : IController
{
    public List<InputFrame> Frames { get; } = [];

    public InputFrame? FetchNextInputFrame()
    {
        var result = new InputFrame
        {
            movement = new Vector2
            {
                X = Input.GetAxis("move_left", "move_right"),
                Y = Input.GetAxis("move_up", "move_down"),
            } * agent.MovementSpeed,
            rotation = GameManager.GetMouseRelativeToCenter().Angle(),
            primaryActionActive = Input.IsActionPressed("primary_action"),
            secondaryActionActive = Input.IsActionPressed("secondary_action"),
        };

        Frames.Add(result);

        return result;
    }
}
