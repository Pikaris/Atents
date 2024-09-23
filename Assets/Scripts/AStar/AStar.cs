using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// 옆으로 이동하는 거리
    /// </summary>
    const float sideDistance = 1.0f;
    //const float sideDistance = 10.0f;

    /// <summary>
    /// 대각선으로 이동하는 거리
    /// </summary>
    const float diagonalDistance = 1.414213f;
    //const float diagonalDistance = 14.0f;


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

        // 시작 위치와 도착 위치가 맵 안이고 벽이 아닐 때만 실행
        if (map.IsValidPosition(start) && map.IsValidPosition(end) && !map.IsWall(start) && !map.IsWall(end))
        {
            map.ClearMapData(); // 맵 데이터 초기화

            List<Node> open = new List<Node>(8);        // open list : 앞으로 탐색할 노드들의 리스트
            List<Node> close = new List<Node>(8);       // close list : 탐색이 완료된 노드들의 리스트

            // A* 알고리즘 시작
            Node current = map.GetNode(start);          // 시작 노드를 open리스트에 추가
            current.G = 0;                              // 노드가 open리스트에 들어갈 때는 F값을 구해야 한다(G값과 H값을 계산해야 한다)
            current.H = GetHeuristic(current, end);
            open.Add(current);

            // A* 루프 시작(알고리즘 핵심부분)
            while(open.Count > 0)       // open 리스트에 노드가 남아있으면 계속 반복(open 리스트가 비었는데 도착 지점에 도달하지 못 했으면 실패)
            {
                open.Sort();            // F값을 기준으로 정렬
                current = open[0];      // F값을 기준으로 정렬되었기 때문에 제일 앞에 있는 것이 F값이 가장 작다
                open.RemoveAt(0);       // open리스트의 첫번째 노드 제거

                if(current != end)      // 도착 지점인지 확인
                {
                    // 목적지가 아니다
                    close.Add(current); // close리스트에 current추가(current가 탐색 완료되었다고 표시)

                    // current의 주변 노드 중 open 리스트에 들어갈 수 있는 노드들을 open리스트에 추가하기
                    for(int y = -1; y < 2; y++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            Node node = map.GetNode(current.X + x, current.Y + y);

                            // 스킵할 노드 확인
                            if (node == null) continue;                          // 맵 밖이다
                            if (node == current) continue;                       // 자기 자신이다
                            if (node.nodeType == Node.NodeType.Wall) continue;   // 벽이다
                            if (close.Contains(node)) continue;                  // close 리스트에 있다
                                                                                 //if(close.Exists((x) => x == node)) continue;      // close 리스트에 있다

                            // 대각선으로 이동하는데 옆에 벽이 있는 경우
                            //Mathf.Abs <- 절대값 구하는 함수
                            bool isDiagonal = (x * y) != 0;     // 대각선인지 확인(true면 대각선)
                            if (isDiagonal && 
                                (map.IsWall(current.X + x, current.Y) || map.IsWall(current.X, current.Y + y)))
                                continue;   // 대각선이고 최소 한쪽이 벽이면 스킵

                            // current에서 (x,y)만큼 추가로 이동하는데 걸리는 거리 결정(대각선은 diagonalDistance, 옆은 sideDistance)
                            float distance = isDiagonal ? diagonalDistance : sideDistance;

                            // 윗줄이랑 똑같음(내부 처리도 똑같음)
                            //if(isDiagonal)
                            //{
                            //    distance = diagonalDistance;
                            //}
                            //else
                            //{
                            //    distance = sideDistance;
                            //}


                            // node는 이미 open리스트에 있거나 어느 리스트에도 들어가지 않았다
                            if(node.G > current.G + distance)   // 노드가 가진 G값이 current를 거쳐서 이동한 것 보다 크다(open리스트에 아직 들어가지 않은 경우에도 true가 된다)
                            {
                                if(node.prev == null)                   // prev가 null이면 아직 open 리스트에 들어간 적이 없다
                                {
                                    // prev가 null이면 아직 open 리스트에 들어간 적이 없다
                                    node.H = GetHeuristic(node, end);   // 휴리스틱 계산
                                    open.Add(node);                     // 새로 open 리스트에 추가
                                }

                                // 공통 처리(G값의 설정 및 갱신이 필요하다)
                                node.G = current.G + distance;          // G값 갱신
                                node.prev = current;                    // 경로상 이전 노드 설정
                            }
                        }
                    }
                }
                else
                {
                    break;      // 도착 지점에 도착했으니 루프 종료
                }
            }

            // 마무리 작업(도착 지점에 도착 or 길을 못 찾았다)
            if(current == end)
            {
                // 도착 지점에 도착 => 경로 만들기
                path = new List<Vector2Int>();
                Node result = current;
                while (result != null)  // result가 null이 될 때까지 반복(start위치에 있는 node는 prev가 null)
                {
                    path.Add(new Vector2Int(result.X, result.Y));   // current 위치 추가(역으로 들어가게 됨)
                    result = result.prev;
                }
                path.Reverse();     // 도착 지점에서 시작 지점까지 역으로 경로가 들어있던 것을 뒤집기
            }
        }

        return path;

        //Vector2Int current;

        //Node node;

        //var openLists = new List<Vector2Int>();
        //var closeLists = new List<Vector2Int>();

        //openLists.Add(start);

        //current = start;

        //openLists.Add(start);

        //for (int x = -1; x < 2; x++)
        //{
        //    for (int y = -1; y < 2; y++)
        //    {
        //        if (map.IsValidPosition(current.x + (int)(x * sideDistance), current.y + (int)(y * sideDistance)))
        //        {
        //            if (map.IsWall(current.x + (int)(x * sideDistance), current.y + (int)(y * sideDistance)))
        //            {
        //                continue;
        //            }

        //            if (x == 0 && y == 0)
        //            {
        //                continue;
        //            }

        //            node = map.GetNode(current.x + (int)(x * sideDistance), current.y + (int)(y * sideDistance));

        //            node.H = GetHeuristic(node, end);
        //            //node.G = 

        //            //openLists.Add(new Vector2Int(current.x + (int)(x * sideDistance), current.y + (int)(y * sideDistance)));

        //        }
        //    }
        //}

        //closeLists.Add(start);

        //return path;
    }

    /// <summary>
    /// A* 알고리즘의 휴리스틱 값을 계산하는 함수(현재 위치에서 목적지까지의 예상 거리)
    /// </summary>
    /// <param name="current">현재 노드</param>
    /// <param name="end">목적지</param>
    /// <returns>예상 거리</returns>
    private static float GetHeuristic(Node current, Vector2Int end)
    {
        //current.H = (end.x - current.X) + (end.y - current.Y);
        return Mathf.Abs(end.x - current.X) + Mathf.Abs(end.y - current.Y);
    }
}
