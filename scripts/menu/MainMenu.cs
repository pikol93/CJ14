using Godot;
using System;

namespace Pikol93.CJ14;

public partial class MainMenu : Control
{
    public void OnPlayButtonPressed()
    {
        GD.Print("Play pressed");
        this.GetAncestor<Menu>().ToggleMenu("LevelSelect");
    }
    
    public void OnOptionsButtonPressed()
    {
        GD.Print("Play pressed");
        this.GetAncestor<Menu>().ToggleMenu("Options");
    }

    public void OnCreditsButtonPressed()
    {
        GD.Print("Play pressed");
        this.GetAncestor<Menu>().ToggleMenu("Credits");
    }

    public void OnQuitButtonPressed()
    {
        GD.Print("Quitting. Goodbye. :)");
        GetTree().Quit();
    }
}
