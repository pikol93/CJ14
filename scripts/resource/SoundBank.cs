using Godot;

namespace Pikol93.CJ14;

public partial class SoundBank : Resource
{
    [Export]
    public float MinPitch = 0.9f;
    [Export]
    public float MaxPitch = 1.1f;
    [Export]
    public Godot.Collections.Array<AudioStream> Samples { get; set; } = [];
}