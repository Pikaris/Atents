using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 정해진 범위 내에서 일정 시간 간격으로 한마리씩 스폰된다
    // 정해진 최대 마리수까지만 스폰된다
    // 스폰 가능한 위치에서만 스폰이 일어난다(Plain인 노드에서만 스폰이 된다)

    /// <summary>
    /// 스폰되는 간격
    /// </summary>
    public float interval = 1.0f;

    /// <summary>
    /// 스폰 영역의 크기(x:오른쪽, y:위쪽, 피봇은 transform.position)
    /// </summary>
    public Vector2 size;

    /// <summary>
    /// 스포너에서 최대로 유지 가능한 슬라임의 수
    /// </summary>
    public int capacity = 3;

    /// <summary>
    /// 마지막 스폰에서 진행된 시간
    /// </summary>
    float elapsedTime = 0.0f;

    /// <summary>
    /// 스포너에서 스폰된 슬라임 중 살아있는 슬라임의 수
    /// </summary>
    int count = 0;

    /// <summary>
    /// 스폰 영역에서 Plain인 노드의 목록
    /// </summary>
    List<Node> spawnAreaList;

    /// <summary>
    /// 그리드맵, 타일맵 관리하는 객체
    /// </summary>
    MapArea mapArea;

    private void Start()
    {
        mapArea = GetComponentInParent<MapArea>();
        spawnAreaList = mapArea.ClacSpawnArea(transform.position, size);
    }

    private void Update()
    {
        if (count < capacity)       // 캐퍼시티가 남아있고
        {
            elapsedTime += Time.deltaTime;  // 시간 누적(캐퍼시티가 남아있을 때만 증가 => 캐퍼시티가 가득 차있다가 남는 공간이 생기면 그때부터 카운팅)
            if (elapsedTime > interval)     // 스폰간격 확인
            {
                Spawn();                    // 둘 다 통과됐으면 스폰
                elapsedTime = 0.0f;
            }
        }
    }

    /// <summary>
    /// 슬라임 한마리만 스폰하는 함수
    /// </summary>
    void Spawn()
    {
        if(IsSpawnAvailable(out Vector3 spawnPosition))
        {
            Slime slime = Factory.Instance.GetSlime(spawnPosition);
            slime.Initialize(mapArea.GridMap, spawnPosition);
            slime.transform.SetParent(transform);
            slime.onDie += () =>
            {
                count--;        // 슬라임이 죽었을 때 count감소
            };
            count++;            // 생성했으니 count 증가

            // 1. 중간에 count가 capacity 이하로 내려가면 다시 스폰을 시작한다.
            // 2. 슬라임이 스폰될 때 슬라임의 부모가 스폰한 스포너로 변경된다.
            // 3. 슬라임이 디스폰될 때 다시 풀을 슬라임의 부모로 설정한다.
        }
    }

    /// <summary>
    /// 스폰 가능한 지역이 있는지 확인하고 리턴해주는 함수
    /// </summary>
    /// <param name="spawnablePosition">스폰 가능한 위치(월드 좌표, 출력용)</param>
    /// <returns>스폰 가능한 지역이 있으면 true, 없으면 false</returns>
    bool IsSpawnAvailable(out Vector3 spawnablePosition)
    {
        bool result = false;

        List<Node> positions = new List<Node>();        // 지금 스폰 가능한 지역의 목록

        foreach (Node node in spawnAreaList)
        {
            if(node.nodeType == Node.NodeType.Plain)    // 지금 평지인 지역 찾기
            {
                positions.Add(node);                    // 찾았으면 지금 스폰 가능한 지역으로 등록
            }
        }

        if (positions.Count > 0)
        {
            // 스폰 가능한 노드가 있다
            int index = Random.Range(0, positions.Count);

            Node target = positions[index];     // 스폰 가능한 지역 중 하나 선택

            spawnablePosition = mapArea.GridToWorld(target.X, target.Y);    // 월드 좌표로 변경해서 기록

            result = true;
        }
        else
        {
            // 스폰 가능한 노드가 없다
            spawnablePosition = Vector3.zero;   // out 파라메터는 반드시 설정되어야 해서 추가
        }

        return result;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 p0 = new(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y));
        Vector3 p1 = p0 + Vector3.right * size.x;
        Vector3 p2 = p0 + (Vector3)size;
        Vector3 p3 = p0 + Vector3.up * size.y;

        Handles.color = Color.red;
        Handles.DrawLine(p0, p1, 5);
        Handles.DrawLine(p1, p2, 5);
        Handles.DrawLine(p2, p3, 5);
        Handles.DrawLine(p3, p0, 5);
    }
#endif
}
