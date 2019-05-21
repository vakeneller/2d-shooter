﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI2 : MonoBehaviour
{
    APIScript api;
    GameController gc;
    List<int> myUnits; // These are the players you can control
    int myTeamId; //This id is used for certain API calls and is unique for your team
    bool[,] map;
    int mapSize;
    private Node[,] initialized_map;

    public bool debugPath = false;

    void Start()
    {
        api = gameObject.GetComponent<APIScript>();
        map = api.GetMap();
        myTeamId = api.teamId;
        myUnits = api.GetPlayers(myTeamId);
        mapSize = (int)Mathf.Sqrt(map.Length);
        Init();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (int unitId in myUnits)
        {

            //Execute your code

        }
    }

    public void Init()
    {
        initialized_map = new Node[mapSize, mapSize];

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                bool trav = map[i, j];
                Vector2 worldPos = api.GetWorldPosFromGridPos(i, j);
                initialized_map[i, j] = new Node(worldPos, i, j, trav);
            }
        }
        //Just some tests of A*:::
        Node testnode1 = initialized_map[0, 0];
        Node testnode2 = initialized_map[mapSize - 1, (int)mapSize / 2];
 
        List<Node> apath = FindPath(testnode1, testnode2);


        if (debugPath)
        {
            ShowPath(apath);
        }
    }

    public List<Node> FindPath(Node start, Node goal)
    {
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        open.Add(start);

        while (open.Count > 0)
        {
            Node current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if ((open[i].gScore + open[i].hScore) < (current.gScore + current.hScore) || (open[i].gScore + open[i].hScore) == (current.gScore + current.hScore) && open[i].hScore < current.hScore)
                {
                    current = open[i];
                }
            }
            open.Remove(current);
            closed.Add(current);
            if (current.position == goal.position)
            {
                return CreatePath(start, goal);

            }
            foreach (Node neighbour in Neighbours(current))
            {
                if (!neighbour.traversable || closed.Contains(neighbour) || IsChoke(current, neighbour))
                {
                    continue;
                }
                float newCost = current.gScore + current.hScore; // add heuristic instead of hscore!
                if (newCost < neighbour.gScore || !open.Contains(neighbour))
                {
                    neighbour.gScore = newCost;
                    neighbour.hScore = Vector2.Distance(goal.position, neighbour.position);// Distance (goal, neighbour);
                    neighbour.cameFrom = current;

                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }
    public List<Node> CreatePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;
        while (current != start && initialized_map[current.x_grid, current.y_grid].cameFrom != null)
        {
            path.Add(current);
            current = initialized_map[current.x_grid, current.y_grid].cameFrom;
        }
        path.Reverse();
        return path;
    }

    public List<Node> Neighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int adjacentX = (int)node.x_grid + i;
                int adjacentY = (int)node.y_grid + j;
                if (adjacentX >= 0 && adjacentX < mapSize && adjacentY >= 0 && adjacentY < mapSize)
                {
                    neighbours.Add(initialized_map[adjacentX, adjacentY]);
                }
            }
        }
        return neighbours;
    }

    public bool IsChoke(Node frompos, Node toPos)
    {
        Vector2 direction = new Vector2(toPos.x_grid - frompos.x_grid, toPos.y_grid - frompos.y_grid);
        if (direction.x != 0 && direction.y != 0)
        {
            if (!initialized_map[frompos.x_grid, toPos.y_grid].traversable && !initialized_map[toPos.x_grid, frompos.y_grid].traversable)
            {
                return true;
            }
        }
        return false;

    }
    

    void ShowPath(List<Node> path)
    {
        int i = 0;
        LineRenderer show = GetComponent<LineRenderer>();

        show.positionCount = path.Count;
        while (i < path.Count)
        {
            Vector3 position = new Vector3(path[i].position.x, path[i].position.y, -5f);
            show.SetPosition(i, position);
            i++;
        }
    }

    public class Node
    {
        public Vector2 position;
        public int x_grid;
        public int y_grid;

        public float gScore;
        public float hScore;

        public Node cameFrom;

        public bool traversable;

        public Node(Vector2 worldPos, int gridX, int gridY, bool traverse)
        {
            this.position = worldPos;
            this.x_grid = gridX;
            this.y_grid = gridY;
            this.traversable = traverse;
        }
    }
}
