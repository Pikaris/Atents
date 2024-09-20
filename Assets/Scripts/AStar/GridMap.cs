using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GridMap
{
    /// <summary>
    /// 이 맵이 가지는 모든 노드
    /// </summary>
    protected Node[] nodes;

    /// <summary>
    /// 이 맵의 가로 길이
    /// </summary>
    protected int width;

    /// <summary>
    /// 이 맵의 세로 길이
    /// </summary>
    protected int height;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="width">맵의 가로 길이</param>
    /// <param name="height">맵의 세로 길이</param>
    public GridMap(int width, int height)
    {
        this.width = width;
        this.height = height;

        nodes = new Node[width * height];

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                nodes[y * width + x] = new Node(x, y);
            }
        }
    }

    /// <summary>
    /// 모든 노드를 대상으로 A* 계산용 데이터 클리어
    /// </summary>
    public void ClearMapData()
    {
        foreach(Node node in nodes)
        {
            node.ClearData();
        }
    }

    /// <summary>
    /// 특정 위치에 있는 노드를 리턴하는 함수
    /// </summary>
    /// <param name="x">x좌표</param>
    /// <param name="y">y좌표</param>
    /// <returns>찾은 노드(잘못된 좌표면 null)</returns>
    public Node GetNode(int x, int y)
    {
        Node result = null;
        if(GridToIndex(x,y,out int? index))
        {
            result = nodes[index.Value];
        }
        return result;
    }

    /// <summary>
    /// 특정 위치에 있는 노드를 리턴하는 함수
    /// </summary>
    /// <param name="grid">좌표</param>
    /// <returns>찾은 노드(잘못된 좌표면 null)</returns>
    public Node GetNode(Vector2Int grid)
    {
        return GetNode(grid.x, grid.y);
    }

    /// <summary>
    /// 특정 위치가 평지인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>true면 평지, false면 평지 아님</returns>
    public bool IsPlain(int x, int y)
    {
        Node node = GetNode(x, y);
        return node != null && node.nodeType == Node.NodeType.Plain;
    }

    /// <summary>
    /// 특정 위치가 평지인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="grid"></param>
    /// <returns>true면 평지, false면 평지 아님</returns>
    public bool IsPlain(Vector2Int grid)
    {
        return IsPlain(grid.x, grid.y);
    }

    /// <summary>
    /// 특정 위치가 벽인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>true면 벽, false면 벽 아님</returns>
    public bool IsWall(int x, int y)
    {
        Node node = GetNode(x, y);
        return node != null && node.nodeType == Node.NodeType.Plain;
    }

    /// <summary>
    /// 특정 위치가 벽인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="grid"></param>
    /// <returns>true면 벽, false면 벽 아님</returns>
    public bool IsWall(Vector2Int grid)
    {
        return IsWall(grid.x, grid.y);
    }

    /// <summary>
    /// 특정 위치가 슬라임인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>true면 슬라임, false면 슬라임 아님</returns>
    public bool IsSlime(int x, int y)
    {
        Node node = GetNode(x, y);
        return node != null && node.nodeType == Node.NodeType.Plain;
    }

    /// <summary>
    /// 특정 위치가 슬라임인지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="grid"></param>
    /// <returns>true면 슬라임, false면 슬라임 아님</returns>
    public bool IsSlime(Vector2Int grid)
    {
        return IsSlime(grid.x, grid.y);
    }

    /// <summary>
    /// 그리드 좌표를 인덱스 값으로 변경해주는 함수
    /// </summary>
    /// <param name="x">x좌표</param>
    /// <param name="y">y좌표</param>
    /// <param name="index">변경된 인덱스 값(출력용)</param>
    /// <returns>변경이 성공했으면 true, 아니면 false</returns>
    protected bool GridToIndex(int x, int y, out int? index)
    {
        bool result = false;
        index = null;               // IsValidPosition가 false일 때를 대비해서 값 설정

        if (IsValidPosition(x, y))  // x,y가 맵 안인지 확인
        {
            index = CalcIndex(x,y); // 맵 안이면 index 계산
            result = true;          // 성공으로 체크
        }

        return result;
    }

    /// <summary>
    /// 인덱스 값을 그리드 좌표로 변경해주는 함수
    /// </summary>
    /// <param name="index">변경할 인덱스</param>
    /// <returns>변경된 그리드 좌표</returns>
    public Vector2Int IndexToGrid(int index)
    {
        return new(index % width, index / width);
    }

    /// <summary>
    /// 간단하게 인덱스를 계산하는 함수
    /// </summary>
    /// <param name="x">x좌표</param>
    /// <param name="y">y좌표</param>
    /// <returns>계산된 인덱스</returns>
    protected virtual int CalcIndex(int x, int y)
    {
        return x + y * width;
    }

    /// <summary>
    /// 맵 안인지 확인하는 함수
    /// </summary>
    /// <param name="x">x좌표</param>
    /// <param name="y">y좌표</param>
    /// <returns>true면 맵 안, false면 맵 밖</returns>
    public virtual bool IsValidPosition(int x, int y)
    {
        return x < width && y < height && x > -1 && y > -1;
    }
}
