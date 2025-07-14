using System;
using Godot;

namespace Pikol93.CJ14;

public partial class CreditItem : Control
{
    public string OriginalName { get; set; }
    public string License { get; set; }
    public string Url { get; set; }
    [Export]
    private NodePath FileNamePath { get; set; }
    [Export]
    private NodePath LicenseTypePath { get; set; }
    [Export]
    private NodePath UrlPath { get; set; }

    public override void _Ready()
    {
        this.GetNodeOrThrow<Label>(FileNamePath).Text = OriginalName;
        this.GetNodeOrThrow<Label>(LicenseTypePath).Text = License;
        this.GetNodeOrThrow<Label>(UrlPath).Text = Url;
    }
}
