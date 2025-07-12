using Godot;
using System.Diagnostics;

namespace Pikol93.CJ14;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    public static bool DebugMode { get; private set; }
    public static bool Paused { get; private set; }

    public GameManager()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            GD.PrintErr("GameManager instance already exists.");
            QueueFree();
            return;
        }

        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent ie)
    {
        if (ie is InputEventKey iek)
        {
            if (iek.Pressed)
            {
                switch (iek.PhysicalKeycode)
                {
                    // Toggle debug mode
                    case Key.F1:
                        DebugMode = !DebugMode;
                        var debugUiInstance = DebugUi.Instance;
                        if (debugUiInstance != null)
                        {
                            debugUiInstance.Visible = DebugMode;
                            debugUiInstance.QueueRedraw();
                        }
                        break;

                    // Toggle FPS lock
                    case Key.F2:
                        var next = DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Disabled ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
                        DisplayServer.WindowSetVsyncMode(next);
                        break;

                    // Reload scene
                    case Key.F7:
                        var sw = new Stopwatch();
                        sw.Start();

                        Error error = GetTree().ReloadCurrentScene();
                        if (error != Error.Ok)
                        {
                            GD.PrintErr($"Couldn't reload the scene. {error}");
                        }

                        sw.Stop();
                        GD.Print($"Reloading scene time: {sw.ElapsedMilliseconds}");
                        break;
                }
            }
        }
    }

    public static Vector2 GetMouseRelativeToCenter()
    {
        if (Instance == null)
            return Vector2.Zero;

        return Instance.GetViewport().GetMousePosition() - Instance.GetViewport().GetVisibleRect().Size / 2;
    }
}
