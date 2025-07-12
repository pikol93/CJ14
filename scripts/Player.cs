using Godot;

namespace Pikol93.CJ14;

public partial class Player : CharacterBody2D
{
    public override void _Ready()
    {
        var camera = new CameraPlayer()
        {
            Enabled = true,
            player = this
        };
        AddChild(camera);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        var targetPosition = GetGlobalMousePosition();
        var direction = (targetPosition - GlobalPosition).Normalized();

        var bullet = Bullet.Create();
        bullet.currentPosition = Position;
        bullet.damage = 20.0;
        bullet.velocity = direction * 2000.0f;
        bullet.minVelocity = 5.0f;

        // TODO: Fix direct GetParent() call.
        GetParent().CallDeferred("add_child", bullet);
    }
}
