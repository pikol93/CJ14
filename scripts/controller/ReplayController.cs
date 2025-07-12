using System.Collections.Generic;

namespace Pikol93.CJ14;

public class ReplayController(List<InputFrame> frames) : IController
{
    private int counter;

    public InputFrame? FetchNextInputFrame()
    {
        var currentCounter = counter;
        counter += 1;
        if (currentCounter < frames.Count) {
            return frames[currentCounter];
        }

        return null;
    }
}