using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Pikol93.CJ14;

public static class GodotExtensions
{
    public static Node GetNodeOrThrow(this Node parent, string path)
    {
        return GetNodeOrThrow<Node>(parent, path);
    }

    public static T GetNodeOrThrow<T>(this Node parent, string path)
        where T : class
    {
        T target = parent.GetNode<T>(path);

        if (target is null)
            throw new ApplicationException($"Couldn't find node by path \"{path}\"");

        return target;
    }

    /// <summary>
    /// Goes up a tree in search of an ancestor of type T
    /// </summary>
    /// <typeparam name="T">The searched type</typeparam>
    /// <returns>The first found ancestor of given type</returns>
    public static T GetAncestor<T>(this Node node) where T : class
    {
        Node parent = node.GetParent();

        // In case the top of the tree has been reached and no node has been found
        if (parent == null)
            return null;

        if (parent is T result)
        {
            return result;
        }
        else
        {
            // Recursively ask next ancestors about a parent of type T
            return parent.GetAncestor<T>();
        }
    }

    /// <summary>
    /// Goes down a tree in search of a descendant of given type
    /// </summary>
    /// <typeparam name="T">The searched type</typeparam>
    /// <returns>The first found descendant of given type</returns>
    public static T GetDescendant<T>(this Node node) where T : class
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T result)
                return result;
            else
            {
                T recursiveResult = child.GetDescendant<T>();
                if (recursiveResult != null)
                    return recursiveResult;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all descendants of given node.
    /// </summary>
    /// <returns>Every descendant of given node.</returns>
    public static List<Node> GetDescendants(this Node node)
    {
        var nodes = new List<Node>() { node };

        foreach (Node child in node.GetChildren())
        {
            nodes.AddRange(child.GetDescendants());
        }

        return nodes;
    }

    /// <summary>
    /// Gets all descendants of given node and filters those who satisfy given type
    /// </summary>
    /// <typeparam name="T">Type to filter the descendants for</typeparam>
    /// <returns>A list containing the descendants of given type</returns>
    public static IEnumerable<T> GetDescendants<T>(this Node node) =>
        node.GetDescendants()
            .OfType<T>();

    public static IEnumerable<T> GetChildren<T>(this Node node) =>
        node.GetChildren()
            .OfType<T>()
            .ToList<T>();

    /// <summary>
    /// Searches the IEnumerable for a node that's closest to supplied Vector2
    /// </summary>
    /// <returns>Nearest Node2D in IEnumerable or null when empty</returns>
    public static Node2D GetNearestNodeToPosition(this IEnumerable<Node2D> nodes, Vector2 position)
    {
        float minDistance = float.PositiveInfinity;
        Node2D result = null;

        foreach (Node2D node in nodes)
        {
            float distance = position.DistanceSquaredTo(node.GlobalPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                result = node;
            }
        }

        return result;
    }

    public static int GetGlobalZIndex(this Node2D node)
    {
        if (node.ZAsRelative)
        {
            // Find if the ZIndex is inherited from parent
            var parent = node.GetParent() as Node2D;
            if (parent != null)
            {
                // Recursive call for all ancestors
                return node.ZIndex + parent.GetGlobalZIndex();
            }
        }

        // Returns Global ZIndex
        return node.ZIndex;
    }

    public static void RemoveFromTree(this Node node)
    {
        Node parent = node.GetParent();
        if (parent == null)
            return;

        parent.RemoveChild(node);
    }

    public static bool IsEqualApprox(this float a, float b)
        => Mathf.Abs(a - b) < Mathf.Epsilon;

    public static Level FindLevelInTree(this Node node)
    {
        var result = node
            .GetTree()
            .Root
            .GetDescendant<Level>();
        
        if (result == null)
        {
            GD.PrintErr("Could not find level in tree.");
        }

        return result;
    }
}