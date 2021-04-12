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
    public Bullet Bullet;
    public GameObject[] Enemies;


    public int missChance = 50;
    public int criticalDamageChance = 20;

    private PlayerHealth HealthUI;

    // Start is called before the first frame update
    void Start()
    {
        Arena = GameObject.FindWithTag("Arena").GetComponent<ArenaGenerator>();
        GameObject.FindGameObjectsWithTag("Arena");
        PositionVector = transform.position;
        Rotation = transform.position;
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        HealthUI = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsMoving)
        {
            if (Input.GetKey(KeyCode.W))
            {
                PositionVector.x += 1;
                if (checkCollWIthObs(PositionVector))
                {
                    PositionVector.x -= 1;
                }
                OnPlayerMove();
            }

            if (Input.GetKey(KeyCode.S))
            {
                PositionVector.x -= 1;
                if (checkCollWIthObs(PositionVector))
                {
                    PositionVector.x += 1;
                }
                OnPlayerMove();
            }

            if (Input.GetKey(KeyCode.A))
            {
                PositionVector.z += 1;
                if (checkCollWIthObs(PositionVector))
                {
                    PositionVector.z -= 1;
                }
                OnPlayerMove();
            }

            if (Input.GetKey(KeyCode.D))
            {
                PositionVector.z -= 1;
                if (checkCollWIthObs(PositionVector))
                {
                    PositionVector.z += 1;
                }
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var bulletPosition = transform.position + transform.forward * 1.5f;
            bulletPosition.y = 1.5f;

            var BulletGameObj = Instantiate(Bullet, bulletPosition, Quaternion.identity);
            var BulletRigidBody = BulletGameObj.GetComponent<Rigidbody>();
            BulletRigidBody.velocity = transform.forward * 20f;
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
        var newDirection = Vector3.RotateTowards(transform.forward, Rotation, Time.deltaTime * 1.5f, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
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
        foreach (var obstacle in Enemies)
        {
            if (obstacle != null)
            {
                var enemy = obstacle.GetComponent<EnemyBehaviour>();
                if (CheckCollission(target, enemy.transform.position))
                {
                    return true;
                }

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag != "Bullet")
        {
            return;
        }
        Destroy(collision.transform.gameObject);
        var criticalDamageRand = Random.Range(0, 100);
        var missRand = Random.Range(0, 100);
        var damage = Random.Range(5, 10);

        if (criticalDamageRand <= criticalDamageChance)
        {
            Debug.Log("Critical Damage");
            damage *= 2;
        }

        if (missRand <= missChance)
        {
            Debug.Log("Miss");
        }
        else
        {
            HealthUI.TakeDamage(damage);
        }
    }

    void OnPlayerMove()
    {

    }
}
