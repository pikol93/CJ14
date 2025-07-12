using Godot;

namespace Pikol93.CJ14;

public struct InputFrame {
    public Vector2 movement;
    public float rotation;
    public bool primaryActionActive;
    public bool secondaryActionActive;
}

public interface IController {
    InputFrame? FetchNextInputFrame();
}