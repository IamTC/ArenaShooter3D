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
    [HideInInspector]
    List<Node> Nodes;
    public Node GoalNode;

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

        GoalNode = new Node(new Vector3(spawnedPlayer.transform.position.x, 0, spawnedPlayer.transform.position.z), NodeType.Tile);

        int numEnemiesToSpawn = NumberOfEnemies;

        while (numEnemiesToSpawn > 0)
        {
            GameObject spawnedEnemy = SpawnAtRandomTile(Enemy, SpawnedHeight);
            if (spawnedEnemy != null)
            {
                //Nodes.Add(new Node(new Vector3(spawnedEnemy.transform.position.x, 0, spawnedEnemy.transform.position.z), NodeType.Enemy));
                numEnemiesToSpawn--;
            }
        }
    }

    private void GenerateTiles()
    {
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {

                if (!IsTileOccupied(new Node(new Vector3(x, 0, z), NodeType.Tile)))
                {
                    var randomTileType = Mathf.CeilToInt(Random.Range(0, TileTypes.Count));
                    var tile = Instantiate(TileTypes[randomTileType], new Vector3(x, 0, z), Quaternion.identity, transform);
                    var cost = tile.GetComponent<TileWeight>();
                    Nodes.Add(new Node(new Vector3(tile.transform.position.x, 0, tile.transform.position.z), NodeType.Tile, cost.Weight));
                }
            }
        }
    }

    private void GenerateWalls()
    {
        for (int z = -1; z <= Height; z++)
        {

            var rightWall = Instantiate(Wall, new Vector3(-1, 1, z), Quaternion.identity, transform);
            var leftWall = Instantiate(Wall, new Vector3(Width, 1, z), Quaternion.identity, transform);
            Nodes.Add(new Node(new Vector3(rightWall.transform.position.x, 0, rightWall.transform.position.z), NodeType.Wall));
            Nodes.Add(new Node(new Vector3(leftWall.transform.position.x, 0, leftWall.transform.position.z), NodeType.Wall));

            if (z > -1)
            {
                var bottomWall = Instantiate(Wall, new Vector3(z, 1, -1), Quaternion.identity, transform);
                var topWall = Instantiate(Wall, new Vector3(z, 1, Width), Quaternion.identity, transform);
                Nodes.Add(new Node(new Vector3(bottomWall.transform.position.x, 0, bottomWall.transform.position.z), NodeType.Wall));
                Nodes.Add(new Node(new Vector3(topWall.transform.position.x, 0, topWall.transform.position.z), NodeType.Wall));
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
                Nodes.Add(new Node(new Vector3(obstacle.transform.position.x, 0, obstacle.transform.position.z), NodeType.Wall));
                numObstacles--;
            }
        }
    }

    public List<Node> GetNodes()
    {
        return Nodes;
    }


    public IList<Node> GetWalkableNodes(Node curr)
    {
        IList<Node> walkableNodes = new List<Node>();
        IList<Node> possibleNodes = new List<Node>(){
            new Node(new Vector3(curr.position.x +1,0,curr.position.z),NodeType.Tile),
            new Node(new Vector3(curr.position.x,0,curr.position.z+1),NodeType.Tile),
            new Node(new Vector3(curr.position.x-1,0,curr.position.z),NodeType.Tile),
            new Node(new Vector3(curr.position.x,0,curr.position.z-1),NodeType.Tile)
            //new Vector3(curr.x+1,0,curr.z+1),
            //new Vector3(curr.x+1,0,curr.z-1),
            //new Vector3(curr.x-1,0,curr.z+1),
            //new Vector3(curr.x-1,0,curr.z-1)
        };

        foreach (Node node in possibleNodes)
        {
            if (!IsTileOccupied(node) && (node.position.x >= 0 && node.position.x <= Width - 1) && (node.position.z >= 0 && node.position.z <= Height - 1))
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

        if (IsTileOccupied(new Node(new Vector3(posX, 0, posZ), NodeType.Tile)))
        {
            return null;
        }
        else
        {
            return Instantiate(prefab, new Vector3(posX, positionY, posZ), Quaternion.identity, transform);
        }
    }

    private bool IsTileOccupied(Node position)
    {
        foreach (var node in Nodes)
        {
            if (node.position.x == position.position.x && node.position.z == position.position.z && node.nodeType != NodeType.Tile)
            {
                return true;
            }
        }

        return false;
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
