using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public ArenaGenerator GridData;
    private SimplePriorityQueue<Vector3, int> priorityQueue;
    // Keep track of all visited notes + which node we got from
    // Necessary later for building the path
    Dictionary<Vector3, Vector3> nodeParents;
    // Start is called before the first frame update
    void Start()
    {
        priorityQueue = new SimplePriorityQueue<Vector3, int>();
        nodeParents = new Dictionary<Vector3, Vector3>();
    }

    public Dictionary<Vector3,Vector3> StartAStar(Vector3 StartTile)
    {
        ClearLists();

        uint nodeVisitCount = 0;
        float timeNow = Time.realtimeSinceStartup;

        // A* tries to minimize f(x) = g(x) + h(x), where g(x) is the distance from the start to node "x" and
        //    h(x) is some heuristic that must be admissible, meaning it never overestimates the cost to the next node.
        //    There are formal logical proofs you can look up that determine how heuristics are and are not admissible.

        IEnumerable<Vector3> validNodes;
        List<Vector3> temp = new List<Vector3>();
        foreach (var node in GridData.GetNodes())
        {
            if (node.nodeType != NodeType.Wall)
            {
                temp.Add(node.position);
            }
        }
        validNodes = temp;

        var enemPos = GridData.GoalNode;
        var walkableNodes = GridData.GetNodes().FindAll(e => e.nodeType != NodeType.Wall);

        // Represents h(x) or the score from whatever heuristic we're using
        IDictionary<Vector3, int> heuristicScore = new Dictionary<Vector3, int>();

        // Represents g(x) or the distance from start to node "x" (Same meaning as in Dijkstra's "distances")
        IDictionary<Vector3, int> distanceFromStart = new Dictionary<Vector3, int>();

        foreach (Vector3 vertex in validNodes)
        {
            if (!heuristicScore.ContainsKey(new KeyValuePair<Vector3, int>(vertex, int.MaxValue).Key))
            {
                heuristicScore.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));
                distanceFromStart.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));

            }
        }

        heuristicScore[enemPos] = DistanceEstimate(StartTile, enemPos);
        distanceFromStart[enemPos] = 0;

        // The item dequeued from a priority queue will always be the one with the lowest int value
        //    In this case we will input nodes with their calculated distances from the start g(x),
        //    so we will always take the node with the lowest distance from the queue.
        SimplePriorityQueue<Vector3, int> priorityQueue = new SimplePriorityQueue<Vector3, int>();
        priorityQueue.Enqueue(enemPos, heuristicScore[enemPos]);

        while (priorityQueue.Count > 0)
        {
            // Get the node with the least distance from the start
            Vector3 curr = priorityQueue.Dequeue();
            nodeVisitCount++;

            // If our current node is the goal then stop
            if (curr == StartTile)
            {
                print("A*" + " time: " + (Time.realtimeSinceStartup - timeNow).ToString());
                print(string.Format("A* visits: {0} ({1:F2}%)", nodeVisitCount, (nodeVisitCount / (double)walkableNodes.Count) * 100));
                //GridData.BuildPath(nodeParents);
                //return;
                return nodeParents;
            }

            IList<Vector3> neighbors = GridData.GetWalkableNodes(curr);

            foreach (Vector3 node in neighbors)
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

    int DistanceEstimate(Vector3 StartTile, Vector3 node)
    {
        var goal = StartTile;
        var cost = GridData.GetNodes().Find(tile => tile.position.x == node.x && tile.position.z == node.z).weight;
        return (int)Math.Ceiling(Mathf.Sqrt(Mathf.Pow(node.x - goal.x, 2) +
            Mathf.Pow(node.y - goal.y, 2) +
            Mathf.Pow(node.z - goal.z, 2)) * cost );
    }

    private void ClearLists()
    {
        priorityQueue.Clear();
        nodeParents.Clear();
    }
}
