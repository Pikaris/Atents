using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Node : IComparable<Node>
{
    /// <summary>
    /// 그리드 맵에서의 X좌표(왼 -> 오른쪽이 +)
    /// </summary>
    private int x;

    public int X => x;

    /// <summary>
    /// 그리드 맵에서의 Y좌표(위 -> 아래쪽이 +)
    /// </summary>
    private int y;

    public int Y => y;

    /// <summary>
    /// A* 알고리즘의 G값(시작 노드에서 이 노트까지의 실제 이동 거리)
    /// </summary>
    public float G;

    /// <summary>
    /// A* 알고리즘의 H값(이 노드에서 도착 노드까지의 예상 거리)
    /// </summary>
    public float H;

    /// <summary>
    /// G와 H의 합(시작노드에서 이 노드를 경유해서 도착 노드까지 가는 예상 거리)
    /// </summary>
    public float F => G + H;

    public enum NodeType
    {
        Plain = 0,      // 평지(이동 가능)
        Wall,           // 벽( 이동 불가능)
        Slime
    }

    /// <summary>
    /// 이 노드의 종류
    /// </summary>
    public NodeType nodeType = NodeType.Plain;

    /// <summary>
    /// 경로 상 앞에 있는 노드
    /// </summary>
    public Node prev;

    /// <summary>
    /// Node 생성자
    /// </summary>
    /// <param name="x">x위치</param>
    /// <param name="y">y위치</param>
    /// <param name="nodeType">노드종류(기본평지)</param>
    public Node(int x, int y, NodeType nodeType = NodeType.Plain)
    {
        this.x = x;
        this.y = y;
        this.nodeType = nodeType;
    }

    /// <summary>
    /// 초기화용 함수(길찾기 반복할 때 초기화 용도)
    /// </summary>
    public void ClearData()
    {
        G = float.MaxValue;     // G값은 작으면 갱신되어야 하므로 기본값은 무조건 커야 한다.
        H = float.MaxValue;
        prev = null;
    }

    /// <summary>
    /// 같은 타입간에 크기를 비교하는 함수(IComparable을 상속 받았으면 구현해야 함)
    /// </summary>
    /// <param name="other">비교대상</param>
    /// <returns>-1, 0, 1 중 하나</returns>
    public int CompareTo(Node other)
    {
        // 리턴 값이 나오는 경우의 수
        // -1(0보다 작다) : 내가 작다(this < other)
        // 0              : 나와 같다(this == other)
        // 1(0보다 크다)  : 내가 크다(this > other)

        if (other == null)      // other가 null이면 내가 작다(작은 순서대로 정렬하는 것이 목표니까)
            return -1;          

        return F.CompareTo(other.F);        // F값을 기준으로 순서를 정해라
    }

    public static bool operator == (Node left, Vector2Int right)
    {
        return left.x == right.x && left.y == right.y;
    }
    public static bool operator != (Node left, Vector2Int right)
    {
        return left.x != right.x || left.y != right.y;
    }

    public override bool Equals(object obj)
    {
        // obj is Node : obj가 Node 타입이어야 true
        // obj is Node other : obj는 Node 타입이고 임시로 other라고 이름을 붙인다.(x와 y에 접근을 위해 캐스팅)
        return obj is Node other && this.x == other.x && this.y == other.y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.x, this.y);        // x, y 위치값을 조합해서 해시 코드 만들기
    }
}
