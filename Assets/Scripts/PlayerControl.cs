using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    private int PlayerSpeed = 10;

    Vector3 PositionVector;
    private ArenaGenerator Arena;

    // Start is called before the first frame update
    void Start()
    {
        Arena = GameObject.FindWithTag("Arena").GetComponent<ArenaGenerator>();
        GameObject.FindGameObjectsWithTag("Arena");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            PositionVector = transform.position;
            MovePlayerTo(new Vector3(PositionVector.x += Time.deltaTime * PlayerSpeed, 0, PositionVector.z));
            //transform.position = PositionVector;

            var healthUI = GetComponent<PlayerHealth>();
            healthUI.TakeDamage(10);
            OnPlayerMove();
        }

        if (Input.GetKey(KeyCode.S))
        {
            PositionVector = transform.position;

            MovePlayerTo(new Vector3(PositionVector.x -= Time.deltaTime * PlayerSpeed, 0, PositionVector.z));

            //transform.position = PositionVector;
            OnPlayerMove();
        }

        if (Input.GetKey(KeyCode.A))
        {
            PositionVector = transform.position;

            MovePlayerTo(new Vector3(PositionVector.x, 0, PositionVector.z += Time.deltaTime * PlayerSpeed));

            //transform.position = PositionVector;
            OnPlayerMove();
        }

        if (Input.GetKey(KeyCode.D))
        {
            PositionVector = transform.position;

            MovePlayerTo(new Vector3(PositionVector.x, 0, PositionVector.z -= Time.deltaTime * PlayerSpeed));

            //transform.position = PositionVector;
            OnPlayerMove();
        }
    }

    void MovePlayerTo(Vector3 target)
    {
        List<Node> obstacles = Arena.GetNodes().FindAll(node => node.nodeType == NodeType.Wall || node.nodeType == NodeType.Enemy);        
        foreach (var obstacle in obstacles)
        {

            //if (obstacle.position.x == normalizedPosistion.x && obstacle.position.z == normalizedPosistion.z)
            if (CheckCollission(target, obstacle.position))
            {
                return;
            }
        }
        transform.position = target;
    }

    public bool CheckCollission(Vector3 source, Vector3 target)
    {       
        if (source.x < target.x + 1 && source.x + 1 > target.x && source.z < target.z + 1 && source.z + 1 > target.z)
        {
            return true;
        }
        return false;
    }

    void OnPlayerMove()
    {

    }
}
