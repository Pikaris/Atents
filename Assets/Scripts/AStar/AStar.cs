using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// 옆으로 이동하는 거리
    /// </summary>
    //const float sideDistance = 1.0f;
    const float sideDistance = 10.0f;

    /// <summary>
    /// 대각선으로 이동하는 거리
    /// </summary>
    //const float diagonalDistance = 1.414213f;
    const float diagonalDistance = 14.0f;


    /// <summary>
    /// 경로를 찾아주는 함수
    /// </summary>
    /// <param name="map">경로를 찾을 맵</param>
    /// <param name="start">시작 위치</param>
    /// <param name="end">도착 위치</param>
    /// <returns>시작 위치에서 도착 위치까지의 경로(길을 못 찾으면 null)</returns>
    public static List<Vector2Int> PathFind(GridMap map, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = null;

        Vector2Int current;

        Node node;

        var openLists = new List<Vector2Int>();
        var closeLists = new List<Vector2Int>();

        openLists.Add(start);

        current = start;

        openLists.Add(start);

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (map.IsValidPosition(current.x + (int)(x * sideDistance), current.y + (int)(y * sideDistance)))
                {
                    if (map.IsWall(current.x + (int)(x * sideDistance), current.y + (int)(y * sideDistance)))
                    {
                        continue;
                    }

                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    node = map.GetNode(current.x + (int)(x * sideDistance), current.y + (int)(y * sideDistance));

                    node.H = GetHeuristic(node, end);
                    //node.G = 

                    //openLists.Add(new Vector2Int(current.x + (int)(x * sideDistance), current.y + (int)(y * sideDistance)));

                }
            }
        }

        //closeLists.Add(start);

        return path;
    }

    /// <summary>
    /// A* 알고리즘의 휴리스틱 값을 계산하는 함수(현재 위치에서 목적지까지의 예상 거리)
    /// </summary>
    /// <param name="current">현재 노드</param>
    /// <param name="end">목적지</param>
    /// <returns>예상 거리</returns>
    private static float GetHeuristic(Node current, Vector2Int end)
    {
        current.H = (end.x - current.X) + (end.y - current.Y);
        return current.H;
    }
}
