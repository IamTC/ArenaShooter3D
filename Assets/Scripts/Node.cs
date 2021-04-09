using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Enemy,
    Wall,
    Tile
}

public class Node 
{
    public Vector3 position;
    public NodeType nodeType;
    public float weight;

    public Node(Vector3 _position, NodeType _nodeType, float _weight = 1 )
    {
        position = _position;
        nodeType = _nodeType;
        weight = _weight;
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }

}
