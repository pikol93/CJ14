using Godot;

namespace Pikol93.CJ14;

public partial class ShooterAgent : Agent
{
    [Export]
    private NodePath AnimationPlayerNodePath { get; set; }
    [Export]
    private NodePath MuzzleNodePath { get; set; }
    [Export]
    private SoundBank PistolShotSoundBank { get; set; }
    [Export]
    private int BaseAmmo { get; set; } = 7;

    private AnimationPlayer animationPlayer;
    private Node2D muzzle;
    private int ammo;

    public override void _Ready()
    {
        base._Ready();
        ammo = BaseAmmo;
        animationPlayer = this.GetNodeOrThrow<AnimationPlayer>(AnimationPlayerNodePath);
        muzzle = this.GetNodeOrThrow<Node2D>(MuzzleNodePath);
    }

    public override AgentType GetAgentType()
    {
        return AgentType.Shooter;
    }

    protected override (int reserve, int total) GetAmmo()
    {
        return (ammo, BaseAmmo);
    }

    protected override void PrimaryJustActivated() {
        ShootBullet();
    }

    private void ShootBullet()
    {
        if (ammo <= 0)
        {
            return;
        }
        ammo--;
        
        var globalPosition = muzzle.GlobalPosition;
        Level.PlayRandomSoundAt(globalPosition, PistolShotSoundBank);

        var angle = Rotation;
        var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        var bullet = Bullet.Create();
        bullet.currentPosition = globalPosition;
        bullet.damage = 7.0;
        bullet.velocity = direction * 4000.0f;
        bullet.minVelocity = 5.0f;
        bullet.Team = Team.Player;

        Level.CallDeferred("add_child", bullet);
        animationPlayer.Play("shoot");
    }
}
