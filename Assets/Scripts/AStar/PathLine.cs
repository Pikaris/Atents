using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = new(Random.value, Random.value, Random.value);

    }

    /// <summary>
    /// 경로를 그리는 함수
    /// </summary>
    /// <param name="map">월드 좌표 계산용 맵</param>
    /// <param name="path">그리드 좌표로 이루어진 경로</param>
    public void DrawPath(TileGridMap map, List<Vector2Int> path)
    {
        if(map != null && path != null)
        {
            lineRenderer.positionCount = path.Count;        // 경로 개수만큼 라인렌더러의 위치 추가

            int index = 0;
            foreach(Vector2Int p in path)
            {
                Vector2 world = map.GridToWorld(p);         // 각 경로 위치를 월드좌표로 변환
                lineRenderer.SetPosition(index, world);     // 라인렌더러에 적용
                index++;                                    // 인덱스 증가
            }
        }
        else
        {
            ClearPath();    // 하나라도 없으면 안 보이게 만들기
        }
    }

    /// <summary>
    /// 경로를 안 보이게 만드는 함수(초기화)
    /// </summary>
    public void ClearPath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
