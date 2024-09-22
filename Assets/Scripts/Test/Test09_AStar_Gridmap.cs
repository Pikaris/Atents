using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test09_AStar_Gridmap : TestBase
{
    public int width = 7;
    public int height = 7;
    public Vector2Int start;
    public Vector2Int end;

    GridMap map;

    private void Start()
    {
        map = new GridMap(width, height);

        Node node;
        node = map.GetNode(map.IndexToGrid(17));
        node.nodeType = Node.NodeType.Wall;
        
        node = map.GetNode(map.IndexToGrid(24));
        node.nodeType = Node.NodeType.Wall;

        node = map.GetNode(map.IndexToGrid(31));
        node.nodeType = Node.NodeType.Wall;

        node = map.GetNode(map.IndexToGrid(37));
        node.nodeType = Node.NodeType.Wall;

        node = map.GetNode(map.IndexToGrid(38));
        node.nodeType = Node.NodeType.Wall;

        start = map.IndexToGrid(22);
        end = map.IndexToGrid(34);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        List<Vector2Int> path = AStar.PathFind(map, start, end);
        PrintList(path);
    }

    void PrintList(List<Vector2Int> list)
    {
        Debug.Log(map);
    }
}
