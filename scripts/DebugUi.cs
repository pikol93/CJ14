using System;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ14;

public partial class DebugUi : Control
{
    public const float UpdateCooldown = 1f;
    public static DebugUi Instance { get; private set; }

    private Dictionary<string, Label> Labels { get; } = [];
    private List<Tuple<Label, string, Func<object>>> UpdateFuncs { get; } = [];

    private double timeLeft;

    public DebugUi()
    {
        if (Instance != null)
        {
            GD.Print("Instance of DebugUI already exists.");
            QueueFree();
            return;
        }

        Instance = this;
        Visible = GameManager.DebugMode;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            Instance = null;
            Labels.Clear();
            UpdateFuncs.Clear();
        }
    }

    public override void _Ready()
    {
        AddToUpdate("FPS", () =>
        {
            return Engine.GetFramesPerSecond();
        });
        AddToUpdate("Memory", () =>
        {
            return (OS.GetStaticMemoryUsage() / 1_048_576.0).ToString("0.00");
        });
    }

    public override void _Process(double delta)
    {
        timeLeft -= delta;
        if (Visible && timeLeft < 0f)
        {
            timeLeft = UpdateCooldown;

            foreach (var tuple in UpdateFuncs)
            {
                object result = tuple.Item3.Invoke();
                if (result != null)
                {
                    tuple.Item1.Text = $"{tuple.Item2}: {result}";
                }
            }
        }
    }

    public static void ManualUpdateInfo(string key, object value)
    {
        if (Instance == null)
        {
            GD.PrintErr($"DebugUI instance is null.");
            return;
        }

        if (!Instance.Labels.ContainsKey(key))
        {
            var child = new Label();
            Instance.Labels.Add(key, child);
            Instance.GetNode("Labels").CallDeferred("add_child", child);
        }

        var label = Instance.Labels[key];
        var text = value == null ? String.Empty : value.ToString();
        label.Text = $"{key}: {text}";
    }

    public static void AddToUpdate(string key, Func<object> func)
    {
        if (Instance == null)
        {
            GD.PrintErr($"DebugUI instance is null.");
            return;
        }

        if (func == null)
        {
            GD.Print("Can't add empty Func to update.");
            return;
        }

        var child = new Label();
        Instance.GetNode("Labels").CallDeferred("add_child", child);

        Instance.UpdateFuncs.Add(Tuple.Create<Label, string, Func<object>>(child, key, func));
    }
}
