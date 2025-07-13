using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Pikol93.NavigationMesh;

namespace Pikol93.CJ14;

public partial class Level : Node2D
{
    private static readonly Color[] PolygonColors =
    [
            Colors.Yellow,
            Colors.Green,
            Colors.Aqua,
            Colors.Blue,
            Colors.Purple,
            Colors.Pink,
            Colors.Orange,
            Colors.Gold,
            Colors.DarkCyan,
            Colors.DarkOrange,
    ];

#pragma warning disable 0649
    [Export]
    public Vector2[] Bounds { get; private set; } =
    [
        new(-10000, -10000),
        new(10000, -10000),
        new(10000, 10000),
        new(-10000, 10000),
    ];
    [Export]
    public double AgentSize { get; private set; } = 12.0f;
#pragma warning restore 0649

    private NavMesh navMesh;

    private void PrepareNavigation()
    {
        GetTree().Connect("node_removed", new Callable(this, "OnChildExitingTree"));
        RebakeNavigation();
    }

    private void OnChildExitingTree(Node node)
    {
        if (IsQueuedForDeletion())
        {
            return;
        }

        if (node is StaticBody2D)
        {
            CallDeferred("RebakeNavigation");
        }
    }

    public void RebakeNavigation()
    {
        var shapes = GetShapes();
        navMesh = NavigationMeshHelper.CreateNavMesh(Bounds, shapes, AgentSize);
        QueueRedraw();
    }
    public override void _Draw()
    {
        if (navMesh != null)
        {
            if (GameManager.DrawNavMesh)
            {
                var random = new Random(0);
                foreach (Polygon polygon in navMesh.Polygons)
                {
                    var points = polygon
                        .Vertices
                        .Select(x => navMesh.Vertices[x])
                        .ToArray()
                        .ToGodotVector2Array();

                    Color[] color = [PolygonColors[random.Next() % PolygonColors.Length]];
                    color[0].A = 0.7f;

                    DrawPolygon(points, color);
                }
            }
        }
    }

    private Vector2[][] GetShapes()
    {
        // Find all children that contain collision shapes
        var collisionShapes = new List<CollisionShape2D>();
        var polygonShapes = new List<CollisionPolygon2D>();
        foreach (var child in this.GetDescendants<StaticBody2D>())
        {
            foreach (var bodyChild in child.GetChildren())
            {
                // Don't process not visible shapes
                if (bodyChild is Node2D node2d && !node2d.Visible)
                    continue;

                if (bodyChild is CollisionShape2D shape)
                    collisionShapes.Add(shape);
                else if (bodyChild is CollisionPolygon2D polygon)
                    polygonShapes.Add(polygon);
            }
        }

        // Get vertices from each found CollisionShape2D and CollisionPolygon2D
        Vector2[][] result = new Vector2[collisionShapes.Count + polygonShapes.Count][];
        for (int i = 0; i < collisionShapes.Count; i++)
        {
            var collisionShape = collisionShapes[i];
            var a = GetVerticesFromCollisionShape(collisionShape);
            result[i] = a.Select(item => collisionShape.GlobalTransform * item).ToArray();
        }
        for (int i = 0; i < polygonShapes.Count; i++)
        {
            var polygonShape = polygonShapes[i];
            var a = GetVerticesFromPolygonShape(polygonShape);
            result[collisionShapes.Count + i] = a.Select(item => polygonShape.GlobalTransform * item).ToArray();
        }

        return result;
    }

    private static Vector2[] GetVerticesFromCollisionShape(CollisionShape2D collisionShape)
    {
        if (collisionShape.Shape is RectangleShape2D rect)
        {
            // Return the vertices in a clockwise order
            return
            [
                    new(-rect.Size.X / 2, -rect.Size.Y / 2),
                    new(rect.Size.X / 2, -rect.Size.Y / 2),
                    new(rect.Size.X / 2, rect.Size.Y / 2),
                    new(-rect.Size.X / 2, rect.Size.Y / 2),
            ];
        }
        else if (collisionShape.Shape is ConvexPolygonShape2D convex)
        {
            // convex.Points already contains the list of vertices
            // but they need to be offset to correctly reflect changes in the editor
            return convex.Points;
        }

        throw new NotImplementedException("Tried to get vertices from a not implemented shape type.");
    }

    private static Vector2[] GetVerticesFromPolygonShape(CollisionPolygon2D collisionPolygon)
    {
        // Keep in mind that the vertices need to be put in a clockwise order!
        return collisionPolygon.Polygon;
    }
}