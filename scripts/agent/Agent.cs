using Godot;

namespace Pikol93.CJ14;

public partial class Agent : CharacterBody2D
{
    [Export]
    public float MovementSpeed { get; set; } = 300.0f;

    public IController Controller { get; set; } = new NullController();

    private InputFrame lastFrame = new()
    {
        movement = Vector2.Zero,
        rotation = 0.0f,
        primaryActionActive = false,
        secondaryActionActive = false,
    };

    public override void _Ready()
    {
        if (Controller is PlayerController) 
        {
            var camera = new CameraPlayer()
            {
                Enabled = true,
                player = this
            };
            AddChild(camera);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var inputFrame = Controller.FetchNextInputFrame();
        if (inputFrame is InputFrame value)
        {
            Velocity = value.movement;
            MoveAndSlide();

            Rotation = value.rotation;

            ProcessPrimary(lastFrame.primaryActionActive, value.primaryActionActive);
            ProcessSecondary(lastFrame.secondaryActionActive, value.secondaryActionActive);

            lastFrame = value;
        }
        else
        {
            ProcessPrimary(lastFrame.primaryActionActive, false);
            ProcessSecondary(lastFrame.secondaryActionActive, false);

            lastFrame.primaryActionActive = false;
            lastFrame.secondaryActionActive = false;
        }
    }

    private void ProcessPrimary(bool before, bool now)
    {
        if (now)
        {
            if (!before)
            {
                PrimaryJustActivated();
            }

            PrimaryActive();
        }
    }

    private void ProcessSecondary(bool before, bool now)
    {
        if (now)
        {
            if (!before)
            {
                SecondaryJustActivated();
            }

            SecondaryActive();
        }
    }

    public virtual AgentType GetAgentType() {
        return AgentType.DummyAgent;
    }

    protected virtual void PrimaryJustActivated() { }

    protected virtual void PrimaryActive() { }

    protected virtual void SecondaryJustActivated() { }

    protected virtual void SecondaryActive() { }
}
