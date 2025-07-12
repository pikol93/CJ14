using Godot;

namespace Pikol93.CJ14;

public partial class ShooterAgent : Agent
{
    public override AgentType GetAgentType()
    {
        return AgentType.ShooterAgent;
    }

    protected override void PrimaryJustActivated() {
        ShootBullet();
    }

    private void ShootBullet()
    {
        var globalPosition = GlobalPosition;
        var angle = Rotation;
        var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        var bullet = Bullet.Create();
        bullet.currentPosition = globalPosition;
        bullet.damage = 20.0;
        bullet.velocity = direction * 4000.0f;
        bullet.minVelocity = 5.0f;

        // TODO: Fix direct GetParent() call.
        GetParent().CallDeferred("add_child", bullet);
    }
}
