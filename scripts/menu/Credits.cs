using Godot;
using System;

namespace Pikol93.CJ14;

public partial class Credits : Control
{
    [Export]
    private PackedScene CreditItemPackedScene { get; set; }
    [Export]
    private NodePath CreditsListParentPath { get; set; }

    public override void _Ready()
    {
        var parent = this.GetNodeOrThrow<Control>(CreditsListParentPath);
        var json = FileAccess
            .Open("res://credits.json", FileAccess.ModeFlags.Read)
            .GetAsText();
        var arr = Json.ParseString(json).AsGodotArray();
        foreach (var item in arr)
        {
            var dict = item.AsGodotDictionary();
            var creditItem = CreditItemPackedScene.Instantiate<CreditItem>();
            creditItem.OriginalName = (string)dict["originalName"];
            creditItem.Url = (string)dict["url"];
            creditItem.License = (string)dict["license"];
            parent.AddChild(creditItem);
        }
    }

    public void OnBackButtonPressed() 
    {
        this.GetAncestor<Menu>().ToggleMenu("Main");
    }
}
