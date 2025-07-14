using System;
using Godot;

namespace Pikol93.CJ14;

public partial class BloodSplatter : Node2D
{
    private static Random Random { get; } = new Random();

    [Export]
    private Texture2D[] Textures { get; set; } = [];
    private Sprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        var index = Random.Next(Textures.Length);
        var texture = Textures[index];
        Rotation = Random.NextSingle() * Mathf.Tau;
        sprite.Texture = texture;
    }
}
