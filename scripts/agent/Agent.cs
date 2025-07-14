using Godot;

namespace Pikol93.CJ14;

public partial class Agent : CharacterBody2D, IDamagable, ITeammable
{
    [Export]
    public float MovementSpeed { get; set; } = 200.0f;
    [Export]
    public double BaseHealth { get; set; } = 1.0f;

    public IController Controller { get; set; } = new NullController();

    private InputFrame lastFrame = new()
    {
        movement = Vector2.Zero,
        rotation = 0.0f,
        primaryActionActive = false,
        secondaryActionActive = false,
    };

    protected Level Level { get; private set; }

    public Team Team => Team.Player;

    private double health;

    public override void _Ready()
    {
        health = BaseHealth;
        if (!GetGroups().Contains("Agent"))
        {
            GD.PrintErr($"Agent not in \"Agent\" group: {Name}");
        }

        Level = this.GetAncestor<Level>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsAlive())
        {
            return;
        }

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

    public bool IsAlive()
    {
        return health > 0.0f;
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

    public virtual AgentType GetAgentType()
    {
        return AgentType.Dummy;
    }

    protected virtual void PrimaryJustActivated() { }

    protected virtual void PrimaryActive() { }

    protected virtual void SecondaryJustActivated() { }

    protected virtual void SecondaryActive() { }

    protected virtual void OnDeath() { }

    public void TakeDamage(double damage, DamageType damageType)
    {
        health -= damage;
        if (health > 0.0f)
        {
            return;
        }

        GD.Print($"Agent {Name} died.");
        CollisionLayer = 0;
        OnDeath();
    }
}
