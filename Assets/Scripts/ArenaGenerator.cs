using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaGenerator : MonoBehaviour
{
    public int Width;
    public int Height;

    public GameObject Player;
    public GameObject Enemy;
    public GameObject Wall;
    public GameObject PlayerHealthUI;
    public int NumberOfEnemies;
    public float SpawnedHeight;
    public int NumberOfObstacles;
    public Mesh PlayerMesh;
    public List<GameObject> TileTypes;
    //[HideInInspector]
    //public List<Vector3> ArenaTiles;
    //[HideInInspector]
    //public List<Vector3> Enemies;
    //[HideInInspector]
    //public List<Vector3> Walls;
    //[HideInInspector]
    [HideInInspector]
    List<Node> Nodes;
    public Vector3 StartTile;
    public Vector3 GoalNode;

    private EnemyBehaviour EnemyControl;

    // Start is called before the first frame update
    void Start()
    {
        Nodes = new List<Node>();
        GenerateArena();
        EnemyControl = GameObject.FindWithTag("Enemy").GetComponent<EnemyBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {        
    }

    private void GenerateArena()
    {
        CleanLists();

        GenerateWalls();

        GenerateObstcles();

        GenerateTiles();

        GameObject spawnedPlayer;
        while ((spawnedPlayer = SpawnAtRandomTile(Player, SpawnedHeight)) == null) ;
        Instantiate(PlayerHealthUI, transform);

        GoalNode = spawnedPlayer.transform.position;

        int numEnemiesToSpawn = NumberOfEnemies;

        while (numEnemiesToSpawn > 0)
        {
            GameObject spawnedEnemy = SpawnAtRandomTile(Enemy, SpawnedHeight);
            if (spawnedEnemy != null)
            {
                //Enemies.Add(spawnedEnemy.transform.position);
                //Nodes.Add(new Node(spawnedEnemy.transform.position, NodeType.Enemy));
                numEnemiesToSpawn--;
                StartTile = spawnedEnemy.transform.position;
            }
        }
    }

    private void GenerateTiles()
    {
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {

                if (!IsTileOccupied(new Vector3(x, 0, z)))
                {
                    var randomTileType = Mathf.CeilToInt(Random.Range(0, TileTypes.Count));
                    var tile = Instantiate(TileTypes[randomTileType], new Vector3(x, 0, z), Quaternion.identity, transform);
                    var randomWeight = Random.Range(0, 5);
                    //ArenaTiles.Add(tile.transform.position);
                    Nodes.Add(new Node(tile.transform.position, NodeType.Tile));
                }
            }
        }
    }

    private void GenerateWalls()
    {
        for (int z = -1; z <= Height; z++)
        {

            var rightWall = Instantiate(Wall, new Vector3(-1, 0, z), Quaternion.identity, transform);
            var leftWall = Instantiate(Wall, new Vector3(Width, 0, z), Quaternion.identity, transform);
            Nodes.Add(new Node(rightWall.transform.position, NodeType.Wall));
            Nodes.Add(new Node(leftWall.transform.position, NodeType.Tile));

            if (z > -1)
            {
                var bottomWall = Instantiate(Wall, new Vector3(z, 0, -1), Quaternion.identity, transform);
                var topWall = Instantiate(Wall, new Vector3(z, 0, Width), Quaternion.identity, transform);
                Nodes.Add(new Node(bottomWall.transform.position, NodeType.Wall));
                Nodes.Add(new Node(topWall.transform.position, NodeType.Wall));
            }
        }
    }

    private void GenerateObstcles()
    {
        int numObstacles = NumberOfObstacles;
        while (numObstacles > 0)
        {
            GameObject obstacle = SpawnAtRandomTile(Wall, Wall.transform.localScale.y / 2);
            if (obstacle != null)
            {
                //Walls.Add(obstacle.transform.position);
                Nodes.Add(new Node(obstacle.transform.position, NodeType.Wall));
                numObstacles--;
            }
        }
    }

    public List<Node> GetNodes()
    {
        return Nodes;
    }


    public IList<Vector3> GetWalkableNodes(Vector3 curr)
    {
        IList<Vector3> walkableNodes = new List<Vector3>();
        IList<Vector3> possibleNodes = new List<Vector3>(){
            new Vector3(curr.x +1,0,curr.z),
            new Vector3(curr.x,0,curr.z+1),
            new Vector3(curr.x-1,0,curr.z),
            new Vector3(curr.x,0,curr.z-1)
            //new Vector3(curr.x+1,0,curr.z+1),
            //new Vector3(curr.x+1,0,curr.z-1),
            //new Vector3(curr.x-1,0,curr.z+1),
            //new Vector3(curr.x-1,0,curr.z-1)
        };

        foreach (Vector3 node in possibleNodes)
        {
            if (!IsTileOccupied(node) && (node.x >= 0 && node.x <= Width - 1) && (node.z >= 0 && node.z <= Height - 1))
            {
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }

    private GameObject SpawnAtRandomTile(GameObject prefab, float positionY)
    {
        var posX = Random.Range(0, Width);
        var posZ = Random.Range(0, Height);

        if (IsTileOccupied(new Vector3(posX, positionY, posZ)))
        {
            return null;
        }
        else
        {
            return Instantiate(prefab, new Vector3(posX, positionY, posZ), Quaternion.identity, transform);
        }
    }

    private bool IsTileOccupied(Vector3 position)
    {
        foreach (var node in Nodes)
        {
            if (node.position.x == position.x && node.position.z == position.z && node.nodeType != NodeType.Tile)
            {
                return true;
            }
        }

        return false;
    }

    public void BuildPath(IDictionary<Vector3, Vector3> nodeParents)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curr = StartTile;
        while (curr != GoalNode)
        {
            path.Add(curr);
            curr = nodeParents[curr];
        }

        if (path.Count > 0)
        {
            EnemyControl.SetMovement(path);
        }
    }

    private void CleanLists()
    {
        DestroyChildren(transform);
        Nodes.Clear();
    }

    private void DestroyChildren(Transform parent)
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }


}
