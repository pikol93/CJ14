using Godot;
using System;

namespace Pikol93.CJ14;

public partial class CloudShaderController : CanvasLayer
{
    private static CloudShaderController Instance { get; set; }

    [Export]
    private float TimeMultiplier { get; set; } = 0.1f;
    [Export]
    private float DepthMultiplier { get; set; } = 0.1f;
    [Export]
    private float ColorMultiplier { get; set; } = 0.1f;

    private ShaderMaterial Material { get; set; }
    private AnimationPlayer AnimationPlayer { get; set; }

    private float time = 0.0f;
    private float noiseDepth = 0.0f;

    public override void _Ready()
    {
        Instance = this;
        Material = (ShaderMaterial)this.GetNodeOrThrow<ColorRect>("ColorRect").Material;
        AnimationPlayer = this.GetNodeOrThrow<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Input(InputEvent ie)
    {
        if (ie is InputEventKey iek)
        {
            if (iek.Pressed)
            {
                switch (iek.PhysicalKeycode)
                {
                    // Excite the background
                    case Key.F5:
                        ExciteInternal();
                        break;
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        time += (float)delta * TimeMultiplier;
        noiseDepth += (float)delta * DepthMultiplier;

        Material.SetShaderParameter("time", time);
        Material.SetShaderParameter("noise_depth", noiseDepth);
        Material.SetShaderParameter("color_multiplier", ColorMultiplier);
    }

    public static void Excite()
    {
        Instance.ExciteInternal();
    }

    public void ExciteInternal()
    {
        AnimationPlayer.Play("excite");
    }
}
