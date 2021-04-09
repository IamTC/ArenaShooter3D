using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private List<Vector3> CurrentPath;
    private Vector3 NextNode;
    private Vector3 CurrentPosition;
    private bool IsMoving;
    float Timer;
    private float MoveSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        CurrentPath = new List<Vector3>();
        IsMoving = false;
    }

    private void CheckPosition()
    {
        Timer = 0;       
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsMoving)
        {
            return;
        }

        Timer += Time.deltaTime;
        if(transform.position != NextNode)
        {
            transform.position = Vector3.Lerp(transform.position, NextNode, Timer);
        } else
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
        Vector3 curr = transform.position;
        while (curr != GoalNode)
        {
            path.Add(curr);
            curr = nodeParents[curr];
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
        NextNode = CurrentPath[0];
    }
}
