using Godot;
using System;
using System.Diagnostics.Contracts;

namespace Pikol93.CJ14;

public partial class OptionsMenu : Control, IMenuItem
{
    [Export]
    private NodePath GorePath { get; set; }
    [Export]
    private NodePath SoundVolumeValuePath { get; set; }
    [Export]
    private NodePath MusicVolumeValuePath { get; set; }

    private CheckButton GoreOption { get; set; }
    private Label SoundVolumeLabel { get; set; }
    private Label MusicVolumeLabel { get; set; }

    public override void _Ready()
    {
        GoreOption = this.GetNodeOrThrow<CheckButton>(GorePath);
        SoundVolumeLabel = this.GetNodeOrThrow<Label>(SoundVolumeValuePath);
        MusicVolumeLabel = this.GetNodeOrThrow<Label>(MusicVolumeValuePath);
    }

    public void OnOpen()
    {
        GoreOption.ButtonPressed = GameSettings.EnableGore;
    }

    public void OnClose()
    {
        GameSettings.Save();
    }

    public void Reload()
    {
        GoreOption.ButtonPressed = GameSettings.EnableGore;
        SoundVolumeLabel.Text = "" + GameSettings.SoundVolume;
        MusicVolumeLabel.Text = "" + GameSettings.MusicVolume;
    }

    public void OnGoreToggled(bool value)
    {
        GameSettings.EnableGore = value;
        GameSettings.Save();
        Reload();
    }

    public void OnSoundMinusPressed()
    {
        GameSettings.SoundVolume = Math.Clamp(GameSettings.SoundVolume - 1, 0, 9);
        GameSettings.Save();
        Reload();
    }

    public void OnSoundPlusPressed()
    {
        GameSettings.SoundVolume = Math.Clamp(GameSettings.SoundVolume + 1, 0, 9);
        GameSettings.Save();
        Reload();
    }

    public void OnMusicMinusPressed()
    {
        GameSettings.MusicVolume = Math.Clamp(GameSettings.MusicVolume - 1, 0, 9);
        GameSettings.Save();
        Reload();
    }

    public void OnMusicPlusPressed()
    {
        GameSettings.MusicVolume = Math.Clamp(GameSettings.MusicVolume + 1, 0, 9);
        GameSettings.Save();
        Reload();
    }
}
