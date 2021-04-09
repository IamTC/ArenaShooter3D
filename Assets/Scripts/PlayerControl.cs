using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    private int PlayerSpeed = 10;

    Vector3 PositionVector;

    AStar a;
    // Start is called before the first frame update
    void Start()
    {
        a = GameObject.FindWithTag("astar").GetComponent<AStar>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {

            PositionVector = transform.position;
            PositionVector.x += Time.deltaTime * PlayerSpeed;
            transform.position = PositionVector;

            var healthUI = GetComponent<PlayerHealth>();
            healthUI.TakeDamage(10);
        }

        if (Input.GetKey(KeyCode.S))
        {

            //PositionVector = transform.position;
            //PositionVector.x -= Time.deltaTime * PlayerSpeed;
            //transform.position = PositionVector;

            a.StartAStar();
        }

        if (Input.GetKey(KeyCode.A))
        {

            PositionVector = transform.position;
            PositionVector.z += Time.deltaTime * PlayerSpeed;
            transform.position = PositionVector;
        }

        if (Input.GetKey(KeyCode.D))
        {

            PositionVector = transform.position;
            PositionVector.z -= Time.deltaTime * PlayerSpeed;
            transform.position = PositionVector;
        }
    }
}
