using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private List<Node> CurrentPath;
    private Node NextNode;
    private bool IsMoving;
    private float MoveSpeed = 0.5f;
    private AStar AStar;
    private PlayerControl Player;
    float TimeLimitPerPathFind = 1f;
    private Transform PlayerTransform;
    // Start is called before the first frame update    
    void Start()
    {
        CurrentPath = new List<Node>();
        IsMoving = false;
        AStar = GameObject.FindWithTag("astar").GetComponent<AStar>();
        Player = GameObject.FindWithTag("PlayerTag").GetComponent<PlayerControl>();
        PlayerTransform = Player.GetComponent<Transform>();
    }

    private void CheckPosition()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = PlayerTransform.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        if (TimeLimitPerPathFind <= 0 && !Player.IsMoving)
        {
            BuildPath(AStar.StartAStar(new Node(new Vector3(transform.position.x, 0, transform.position.z), NodeType.Tile), new Node(new Vector3(Player.PositionVector.x, 0, Player.PositionVector.z), NodeType.Tile)),
               new Node(new Vector3(Player.PositionVector.x, 0, Player.PositionVector.z), NodeType.Tile));
            TimeLimitPerPathFind = 1f;
        }
        else
        {
            TimeLimitPerPathFind-= Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Space))
        {

        }

        if (!IsMoving)
        {
            return;
        }

        if (!new Node(transform.position, NodeType.Tile).Equals(NextNode))
        {
            //transform.position = Vector3.Lerp(transform.position, NextNode, Timer);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(NextNode.position.x, 0.5f, NextNode.position.z), Time.deltaTime * 0.9f);
        }
        else
        {
            if (CurrentPath.Count > 0)
            {
                NextNode = CurrentPath[0];
                CurrentPath.RemoveAt(0);
                CheckPosition();
            }
            else
            {
                IsMoving = false;
            }
        }
        IsInRange();
    }

    public void BuildPath(IDictionary<Node, Node> nodeParents, Node GoalNode)
    {
        List<Node> path = new List<Node>();
        Node curr = new Node(new Vector3(transform.position.x, 0, transform.position.z), NodeType.Tile);
        while (!curr.Equals(GoalNode))
        {
            path.Add(curr);
            try
            {
                curr = nodeParents[curr];
            }
            catch (KeyNotFoundException ex)
            {
                Debug.Log("Path not available to player");
                return;
            }
        }

        if (path.Count > 0)
        {
            path.RemoveAt(0);
            SetMovement(path);
        }
    }

    private bool IsInRange()
    {
        float distance = Mathf.Sqrt(Mathf.Pow(Player.PositionVector.x - transform.position.x, 2f) + Mathf.Pow(Player.PositionVector.z - transform.position.z, 2f));
        Debug.Log("FIREEEEEEEEE");
        return distance < 5;
    }

    public void SetMovement(List<Node> path)
    {
        CurrentPath = path;
        if (CurrentPath.Count > 0)
        {
            IsMoving = true;
            NextNode = CurrentPath[0];
        }
        else
        {
            IsMoving = false;
        }
    }
}
