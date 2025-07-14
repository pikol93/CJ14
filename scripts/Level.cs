using Godot;
using System;
using System.Collections.Generic;

namespace Pikol93.CJ14;

public partial class Level : Node2D
{
    private static readonly Random random = new();

    private Queue<AudioStreamPlayer2D> AudioPlayers { get; } = [];

    [Export]
    public PackedScene BloodSplatterScene { get; set; } = GD.Load<PackedScene>("res://scenes/blood_splatter.tscn");
    [Export]
    public int ShootersAvailable { get; set; } = 0;
    [Export]
    public int BombersAvailable { get; set; } = 0;
    [Export]
    public int ShotgunsAvailable { get; set; } = 0;

    public override void _Ready()
    {
        for (var i = 0; i < 32; i++) {
            var audioPlayer = new AudioStreamPlayer2D
            {
                Bus = "Sound"
            };
            AddChild(audioPlayer);
            AudioPlayers.Enqueue(audioPlayer);
        }

        PrepareNavigation();
    }

    public void PlayRandomSoundAt(Vector2 position, SoundBank soundBank)
    {
        if (soundBank is null)
        {
            throw new ApplicationException("Sound bank is not set.");
        }

        var samples = soundBank.Samples;
        var samplesCount = samples.Count;
        if (samplesCount == 0)
        {
            throw new ApplicationException("Sound bank is empty.");
        }

        var pitch = soundBank.MinPitch + ((soundBank.MaxPitch - soundBank.MinPitch) * random.NextSingle());

        var index = random.Next(samples.Count);
        var sample = samples[index];

        var player = AudioPlayers.Dequeue();
        player.PitchScale = pitch;
        player.Stream = sample;
        player.Position = position;
        player.Play();
        AudioPlayers.Enqueue(player);
    }

    public void SpawnParticles(Vector2 position, Node2D particlesNode)
    {
        particlesNode.Position = position;
        CallDeferred("add_child", particlesNode);
    }

    public void SpawnBloodSplatters(Vector2 position, int amount)
    {
        if (!GameSettings.EnableGore)
        {
            return;
        }

        const float Distance = 50.0f;
        var random = new Random();
        for (var i = 0; i < amount; i++)
        {
            var offset = new Vector2((random.NextSingle() - 0.5f) * 2 * Distance, (random.NextSingle() - 0.5f) * 2 * Distance);
            var node = BloodSplatterScene.Instantiate<BloodSplatter>();
            node.Position = position + offset;
            AddChild(node);
        }
    }
}
