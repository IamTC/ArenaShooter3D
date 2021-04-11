using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NodeType
{
    Enemy,
    Wall,
    Tile
}

public class Node : IEquatable<Node>
{
    public Vector3 position;
    public NodeType nodeType;
    public float weight;

    public Node(Vector3 _position, NodeType _nodeType, float _weight = 1)
    {
        position = _position;
        nodeType = _nodeType;
        weight = _weight;
    }

    public override int GetHashCode()
    {
        string numStr = position.x + "" + position.z;
        if (numStr.All(char.IsNumber)){
            return Convert.ToInt32(numStr);
        } else
        {
            return 0;
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Node);
    }

    public bool Equals(Node other)
    {
        if ((other == null) || !this.GetType().Equals(other.GetType()))
        {
            return false;
        }
        else
        {
            Node node = (Node)other;
            if (node.position == null)
            {
                return false;
            }
            return (position.x == node.position.x) && (position.z == node.position.z);
        }
    }
}