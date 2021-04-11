using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public ArenaGenerator GridData;
    private SimplePriorityQueue<Node, int> priorityQueue;
    // Keep track of all visited notes + which node we got from
    // Necessary later for building the path
    Dictionary<Node, Node> nodeParents;
    // Start is called before the first frame update
    void Start()
    {
        priorityQueue = new SimplePriorityQueue<Node, int>();
        nodeParents = new Dictionary<Node, Node>();
    }

    public Dictionary<Node, Node> StartAStar(Node StartTile, Node GoalTile)
    {
        ClearLists();

        uint nodeVisitCount = 0;
        float timeNow = Time.realtimeSinceStartup;

        // A* tries to minimize f(x) = g(x) + h(x), where g(x) is the distance from the start to node "x" and
        //    h(x) is some heuristic that must be admissible, meaning it never overestimates the cost to the next node.
        //    There are formal logical proofs you can look up that determine how heuristics are and are not admissible.

        IEnumerable<Node> validNodes;
        List<Node> temp = new List<Node>();
        foreach (var node in GridData.GetNodes())
        {
            if (node.nodeType == NodeType.Tile)
            {
                temp.Add(node);
            }
        }
        validNodes = temp;

        var enemPos = GoalTile;

        // Represents h(x) or the score from whatever heuristic we're using
        IDictionary<Node, int> heuristicScore = new Dictionary<Node, int>();

        // Represents g(x) or the distance from start to node "x" (Same meaning as in Dijkstra's "distances")
        IDictionary<Node, int> distanceFromStart = new Dictionary<Node, int>();

        foreach (Node vertex in validNodes)
        {
            if (!heuristicScore.ContainsKey(new KeyValuePair<Node, int>(vertex, int.MaxValue).Key))
            {
                heuristicScore.Add(new KeyValuePair<Node, int>(vertex, int.MaxValue));
                distanceFromStart.Add(new KeyValuePair<Node, int>(vertex, int.MaxValue));

            }
        }
        foreach(KeyValuePair<Node, int> keyValuePair in heuristicScore)
        {
            if(keyValuePair.Key == enemPos)
            {
                Debug.Log("Found");
            }
        }
        heuristicScore[enemPos] = DistanceEstimate(StartTile, enemPos);
        
        distanceFromStart[enemPos] = 0;

        // The item dequeued from a priority queue will always be the one with the lowest int value
        //    In this case we will input nodes with their calculated distances from the start g(x),
        //    so we will always take the node with the lowest distance from the queue.
        SimplePriorityQueue<Node, int> priorityQueue = new SimplePriorityQueue<Node, int>();
        priorityQueue.Enqueue(enemPos, heuristicScore[enemPos]);

        while (priorityQueue.Count > 0)
        {
            // Get the node with the least distance from the start
            Node curr = priorityQueue.Dequeue();
            nodeVisitCount++;

            // If our current node is the goal then stop
            if (curr == StartTile)
            {
                print("A*" + " time: " + (Time.realtimeSinceStartup - timeNow).ToString());
                print(string.Format("A* visits: {0} ({1:F2}%)", nodeVisitCount, (nodeVisitCount / (float)validNodes.Count() ) * 100));
                //GridData.BuildPath(nodeParents);
                //return;
                return nodeParents;
            }

            IList<Node> neighbors = GridData.GetWalkableNodes(curr);

            foreach (Node node in neighbors)
            {
                // Get the distance so far, add it to the distance to the neighbor
                int currScore = distanceFromStart[curr] + 1;

                // If our distance to this neighbor is LESS than another calculated shortest path
                //    to this neighbor, set a new node parent and update the scores as our current
                //    best for the path so far.
                if (currScore < distanceFromStart[node])
                {
                    nodeParents[node] = curr;
                    distanceFromStart[node] = currScore;

                    int hScore = distanceFromStart[node] + DistanceEstimate(StartTile, node);
                    heuristicScore[node] = hScore;

                    // If this node isn't already in the queue, make sure to add it. Since the
                    //    algorithm is always looking for the smallest distance, any existing entry
                    //    would have a higher priority anyway.
                    if (!priorityQueue.Contains(node))
                    {
                        priorityQueue.Enqueue(node, hScore);
                    }
                }
            }
        }

        return nodeParents;
    }

    int DistanceEstimate(Node StartTile, Node node)
    {
        var goal = StartTile;
        //var cost = GridData.GetNodes().Find(tile => tile == node).weight;
        return (int)Math.Ceiling(Mathf.Sqrt(Mathf.Pow(node.position.x - goal.position.x, 2) +
            Mathf.Pow(node.position.y - goal.position.y, 2) +
            Mathf.Pow(node.position.z - goal.position.z, 2)));
    }

    private void ClearLists()
    {
        priorityQueue.Clear();
        nodeParents.Clear();
    }
}
