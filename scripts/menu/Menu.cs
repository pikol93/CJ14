using Godot;
using System.Collections.Generic;

namespace Pikol93.CJ14;

public partial class Menu : Control
{
    [Export]
    private NodePath[] MenuPaths;
    private List<Control> MenuNodes { get; } = [];

    public override void _Ready()
    {
        foreach (var path in MenuPaths)
        {
            var node = this.GetNodeOrThrow<Control>(path);
            node.Visible = false;

            MenuNodes.Add(node);
        }

        ToggleMenu("EpilepsyWarning");
    }

    public void ToggleMenu(string name)
    {
        foreach (var item in MenuNodes)
        {
            var before = item.Visible;
            item.Visible = item.Name == name;
            if (item is IMenuItem menuItem)
            {
                if (!before && item.Visible)
                {
                    menuItem.OnOpen();
                }
                else if (before && !item.Visible)
                {
                    menuItem.OnClose();
                }
            }
        }
    }
}
