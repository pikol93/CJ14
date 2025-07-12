using Godot;

namespace Pikol93.CJ14;

public partial class SoundBank : Resource
{
    [Export]
    public Godot.Collections.Array<AudioStream> Samples { get; set; } = [];
}