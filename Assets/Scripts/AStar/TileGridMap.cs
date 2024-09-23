using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridMap : GridMap
{
    /// <summary>
    /// 맵의 원점
    /// </summary>
    Vector2Int origin;

    /// <summary>
    /// 배경 타일맵(크기 확인 및 좌표계산용으로 사용)
    /// </summary>
    Tilemap background;

    /// <summary>
    /// 이동 가능한 지역(평지)의 모음
    /// </summary>
    Vector2Int[] movablePositions;

    /// <summary>
    /// 타일맵을 이용해 그리드맵을 생성하는 생성자
    /// </summary>
    /// <param name="background">배경용 타일(가로, 세로, 맵 위치 등등)</param>
    /// <param name="obstacle">장애물용 타일맵(길찾기 할 때 못가는 지역)</param>
    public TileGridMap(Tilemap background, Tilemap obstacle)
    {
        this.background = background;           // background 저장

        width = background.size.x;              // 가로/세로 길이 설정
        height = background.size.y;

        origin = (Vector2Int)background.origin; // 원점 저장

        nodes = new Node[width * height];       // 노드 배열 만들기

        Vector2Int min = (Vector2Int)background.cellBounds.min;             // for을 위한 최소/최대 값. 코드가 길어지는 것을 방지하기 위한 용도
        Vector2Int max = (Vector2Int)background.cellBounds.max;
        List<Vector2Int> movable = new List<Vector2Int>(width * height);    // 이동 가능한 지역을 임시로 기록할 리스트

        for (int y = min.y; y < max.y; y++)         // background 위치와 크기에 맞춰서 그리드 순회
        {
            for (int x = min.x; x < max.x; x++)
            {
                Node.NodeType nodeType = Node.NodeType.Plain;   // 기본 값(tile이 없으면 평지)
                TileBase tile = obstacle.GetTile(new(x, y));    // 장애물 타일맵에서 타일 가져오기 시도
                if(tile != null)
                {
                    nodeType = Node.NodeType.Wall;              // 있으면 그곳은 못가는 지역
                }
                else
                {
                    movable.Add(new Vector2Int(x, y));          // 없으면 갈 수 있는 지역이다.
                }

                nodes[CalcIndex(x, y)] = new Node(x, y, nodeType);  // 인덱스 위치에 노드 생성
            }
        }

        movablePositions = movable.ToArray();   // 임시 리스트를 배열로 변경해서 저장
    }

    public override int CalcIndex(int x, int y)
    {
        // 원래 식 : x + y * width;
        // 원점이 변경 됨 : (x - origin.x) + (y - origin.y) * width;
        // y축이 반대     : (x - origin.x) + ((height - 1) - (y - origin.y)) * width;
        return (x - origin.x) + ((height - 1) - (y - origin.y)) * width;
    }

    public override bool IsValidPosition(int x, int y)
    {
        // 원래 식 : x < width && y < height && x > -1 && y > -1
        return x < (width + origin.x) && y < (height + origin.y) && x >= origin.x && y >= origin.y;
    }

    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변경하는 함수
    /// </summary>
    /// <param name="world">월드 좌표</param>
    /// <returns>타일맵의 셀 좌표</returns>
    public Vector2Int WorldToGrid(Vector3 world)
    {
        return (Vector2Int)background.WorldToCell(world);
    }

    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변경하는 함수
    /// </summary>
    /// <param name="grid">그리드 좌표</param>
    /// <returns>셀의 가운데 위치(월드 좌표)</returns>
    public Vector2 GridToWorld(Vector2Int grid)
    {
        return background.CellToWorld((Vector3Int)grid) + new Vector3(0.5f, 0.5f);  // CellToWorld는 셀의 왼쪽 아래의 월드 좌표를 리턴
    }

    /// <summary>
    /// 이동 가능한 위치 중 랜덤으로 선택해서 리턴하는 함수
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetRandomMovablePosition()
    {
        int index = Random.Range(0, movablePositions.Length);
        return movablePositions[index];
    }

    /// <summary>
    /// 월드 좌표를 통해 해당 위치에 있는 노드를 리턴하는 함수
    /// </summary>
    /// <param name="world">확인할 위치(월드 좌표)</param>
    /// <returns>해당 위치에 있는 노드</returns>
    public Node GetNode(Vector3 world)
    {
        return GetNode(WorldToGrid(world));
    }

#if UNITY_EDITOR
    public int Test_CalcIndex(int x, int y)
    {
        return CalcIndex(x, y);
    }
#endif
}
