using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    private int PlayerSpeed = 10;

    public Vector3 PositionVector;
    public Vector3 Rotation;
    private ArenaGenerator Arena;
    public bool IsMoving;
    public bool IsTurning;

    // Start is called before the first frame update
    void Start()
    {
        Arena = GameObject.FindWithTag("Arena").GetComponent<ArenaGenerator>();
        GameObject.FindGameObjectsWithTag("Arena");
        PositionVector = transform.position;
        Rotation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsMoving)
        {
            if (Input.GetKey(KeyCode.W))
            {
                //MovePlayerTo(new Vector3(PositionVector.x += Time.deltaTime * PlayerSpeed, 0, PositionVector.z));
                PositionVector.x += 1;
                if (checkCollWIthObs(PositionVector))
                {
                    PositionVector.x -= 1;
                }
                //transform.position = Vector3.Slerp(transform.position, PositionVector, Time.deltaTime * 10);

                var healthUI = GetComponent<PlayerHealth>();
                healthUI.TakeDamage(10);
                OnPlayerMove();
            }

            if (Input.GetKey(KeyCode.S))
            {
                PositionVector.x -= 1;
                if (checkCollWIthObs(PositionVector))
                {
                    PositionVector.x += 1;
                }
                //MovePlayerTo(new Vector3(PositionVector.x -= Time.deltaTime * PlayerSpeed, 0, PositionVector.z));

                //transform.position = PositionVector;
                OnPlayerMove();
            }

            if (Input.GetKey(KeyCode.A))
            {
                PositionVector.z += 1;
                if (checkCollWIthObs(PositionVector))
                {
                    PositionVector.z -= 1;
                }
                //MovePlayerTo(new Vector3(PositionVector.x, 0, PositionVector.z += Time.deltaTime * PlayerSpeed));

                //transform.position = PositionVector;
                OnPlayerMove();
            }

            if (Input.GetKey(KeyCode.D))
            {
                PositionVector.z -= 1;
                if (checkCollWIthObs(PositionVector))
                {
                    PositionVector.z += 1;
                }
                //MovePlayerTo(new Vector3(PositionVector.x, 0, PositionVector.z -= Time.deltaTime * PlayerSpeed));

                //transform.position = PositionVector;
                OnPlayerMove();
            }            
        }


        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                SetTurn(hit);
            }

        }

        TurnToPoint();

        CheckPlayerMove();

    }

    void SetTurn(RaycastHit target)
    {
        IsTurning = true;

        Vector3 targetDirection = new Vector3(target.point.x, 0, target.point.z) - new Vector3(transform.position.x, 0, transform.position.z);
        Rotation = targetDirection;
    }

    void TurnToPoint()
    {
        //if(!IsTurning)
        //{
        //    return;
        //}
        var newDirection = Vector3.RotateTowards(transform.forward, Rotation, Time.deltaTime, 0.0f);
        //if (!transform.rotation.Equals(newDirection))
        //{
        transform.rotation = Quaternion.LookRotation(newDirection);
        //IsTurning = false;
        //}
    }

    void MovePlayerTo(Vector3 target)
    {
        List<Node> obstacles = Arena.GetNodes().FindAll(node => node.nodeType == NodeType.Wall || node.nodeType == NodeType.Enemy);
        foreach (var obstacle in obstacles)
        {

            if (CheckCollission(target, obstacle.position))
            {
                PositionVector = transform.position;
                return;
            }
        }
        transform.position = Vector3.Lerp(transform.position, PositionVector, Time.deltaTime * 50);
    }

    private bool checkCollWIthObs(Vector3 target)
    {
        List<Node> obstacles = Arena.GetNodes().FindAll(node => node.nodeType == NodeType.Wall || node.nodeType == NodeType.Enemy);
        foreach (var obstacle in obstacles)
        {

            if (CheckCollission(target, obstacle.position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckCollission(Vector3 source, Vector3 target)
    {
        if (source.x < target.x + 1 && source.x + 1 > target.x && source.z < target.z + 1 && source.z + 1 > target.z)
        {
            return true;
        }
        return false;
    }

    void CheckPlayerMove()
    {
        if (transform.position != PositionVector)
        {
            IsMoving = true;
            MovePlayerTo(PositionVector);
        }
        else
        {
            IsMoving = false;
        }
    }

    void OnPlayerMove()
    {

    }
}
