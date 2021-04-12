using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    public Bullet Bullet;
    public int FireRate;
    public int MaxRange;

    private List<Node> CurrentPath;
    private Node NextNode;
    private bool IsMoving;
    public float MoveSpeed = 0.5f;
    private AStar AStar;
    private PlayerControl Player;
    private float TimeLimitPerPathFind = 1f;
    private Transform PlayerTransform;

    private float FireTimer = 0f;
    private Vector3 PlayerLastSeen;
    public GameObject HealthTextObj;
    private TextMesh HealthText;
    private EnemyState state = EnemyState.Idle;
    // Start is called before the first frame update   

    public int missChance = 10;
    public int criticalDamageChance = 80;

    private int health = 100;
    void Start()
    {
        CurrentPath = new List<Node>();
        IsMoving = false;
        AStar = GameObject.FindWithTag("astar").GetComponent<AStar>();
        Player = GameObject.FindWithTag("PlayerTag").GetComponent<PlayerControl>();
        PlayerTransform = Player.GetComponent<Transform>();
        PlayerLastSeen = new Vector3(Player.PositionVector.x, Player.PositionVector.y, Player.PositionVector.z);
        HealthText = Instantiate(HealthTextObj, transform.position, Quaternion.identity, transform).GetComponent<TextMesh>();        
        HealthText.text = GetDisplayText();
    }

    private void CheckPosition()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HealthText.transform.LookAt(Camera.main.transform);
        Vector3 targetDirection = PlayerTransform.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        if (TimeLimitPerPathFind <= 0 && !Player.IsMoving && HasPlayerMoved())
        {
            BuildPath(AStar.StartAStar(new Node(new Vector3(transform.position.x, 0, transform.position.z), NodeType.Tile), new Node(new Vector3(Player.PositionVector.x, 0, Player.PositionVector.z), NodeType.Tile)),
               new Node(new Vector3(Player.PositionVector.x, 0, Player.PositionVector.z), NodeType.Tile));
            TimeLimitPerPathFind = 1f;
        }
        else
        {
            TimeLimitPerPathFind -= Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Space))
        {

        }

        if (state == EnemyState.Idle)
        {
            return;
        }

        if (!new Node(transform.position, NodeType.Tile).Equals(NextNode))
        {
            if (!IsInRange())
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(NextNode.position.x, 0.5f, NextNode.position.z), Time.deltaTime * MoveSpeed);
            }
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
                state = EnemyState.Idle;
            }
        }
        if (IsInRange())
        {
            Shoot();
        }
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
                return;
            }
        }

        if (path.Count > 0)
        {
            path.RemoveAt(0);
            SetMovement(path);
        }
    }

    private void Shoot()
    {
        FireTimer += Time.deltaTime;
        if (FireTimer * 10 >= FireRate)
        {            
            FireTimer = 0f;

            var bulletPosition = transform.position + transform.forward * 1.5f;
            bulletPosition.y = 1.5f;

            var BulletGameObj = Instantiate(Bullet, bulletPosition, Quaternion.identity);
            var BulletRigidBody = BulletGameObj.GetComponent<Rigidbody>();
            BulletRigidBody.velocity = transform.forward * 20f;            
        }

    }

    private bool IsInRange()
    {
        float distance = GetDistance(Player.PositionVector, transform.position);
        return distance < MaxRange;
    }

    private float GetDistance(Vector3 A, Vector3 B)
    {
        return Mathf.Sqrt(Mathf.Pow(A.x - B.x, 2f) + Mathf.Pow(A.z - B.z, 2f));
    }

    private bool HasPlayerMoved()
    {
        return PlayerLastSeen != new Vector3(Player.PositionVector.x, Player.PositionVector.y, Player.PositionVector.z);
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
            damage *= 2;
        }

        if (missRand > missChance)
        {
            health -= damage;
            HealthText.text = GetDisplayText();
            if(health <=0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetMovement(List<Node> path)
    {
        CurrentPath = path;
        if (CurrentPath.Count > 0)
        {
            state = EnemyState.Chasing;
            NextNode = CurrentPath[0];
        }
        else
        {
            state = EnemyState.Idle;
        }
        HealthText.text = GetDisplayText();
    }

    public string GetDisplayText()
    {
        var stateStr = "";
        switch(state)
        {
            case (EnemyState.Idle):
                stateStr = "Idle";
                break;
            case (EnemyState.Chasing):
                stateStr = "Chasing";
                break;
        }
        return health.ToString() + "\n" + Mathf.Abs(GetHashCode()) + "\n" + stateStr;
    }
}


public enum EnemyState
{
    Chasing,
    Idle
}