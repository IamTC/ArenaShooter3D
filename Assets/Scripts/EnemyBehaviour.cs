using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private List<Vector3> CurrentPath;
    private Vector3 NextNode;
    private bool IsMoving;
    float Timer;
    private float MoveSpeed = 0.5f;
    private AStar AStar;
    private Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        CurrentPath = new List<Vector3>();
        IsMoving = false;
        AStar = GameObject.FindWithTag("astar").GetComponent<AStar>();
        Player = GameObject.FindWithTag("PlayerTag").GetComponent<Transform>();
    }

    private void CheckPosition()
    {
        Timer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            BuildPath(AStar.StartAStar(transform.position), Player.position);
        }

        if (!IsMoving)
        {
            return;
        }

        Timer += Time.deltaTime;
        if (transform.position != NextNode)
        {
            //transform.position = Vector3.Lerp(transform.position, NextNode, Timer);
            transform.position = Vector3.MoveTowards(transform.position, NextNode, Timer);
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
    }

    public void BuildPath(IDictionary<Vector3, Vector3> nodeParents, Vector3 GoalNode)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curr = new Vector3((int)transform.position.x, 0, (int)transform.position.z);
        while (curr != GoalNode)
        {
            path.Add(curr);
            try
            {
                curr = nodeParents[curr];
            }
            catch(KeyNotFoundException ex)
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

    public void SetMovement(List<Vector3> path)
    {
        IsMoving = true;
        CurrentPath = path;
        if (CurrentPath.Count > 0)
        {
            NextNode = CurrentPath[0];
        }
    }
}
