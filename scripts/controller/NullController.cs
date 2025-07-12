using Godot;

namespace Pikol93.CJ14;

public class NullController : IController
{
    public InputFrame? FetchNextInputFrame()
    {
        GD.PrintErr("NullController used. This is probably an error.");
        return null;
    }
}