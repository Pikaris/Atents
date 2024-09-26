using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmapManager : MonoBehaviour
{
    /// <summary>
    /// 서브맵의 세로 개수
    /// </summary>
    const int HeightCount = 3;

    /// <summary>
    /// 서브맵의 가로 개수
    /// </summary>
    const int WidthCount = 3;

    /// <summary>
    /// 서브맵의 가로 길이
    /// </summary>
    const float submapWidthSize = 20.0f;
    
    /// <summary>
    /// 서브맵의 세로 길이
    /// </summary>
    const float submapHeightSize = 20.0f;

    /// <summary>
    /// 월드(모든 서브맵의 합)의 원점(월드의 왼쪽 아래 끝)
    /// </summary>
    readonly Vector2 worldOrigine = new Vector2(-submapWidthSize * WidthCount * 0.5f, -submapHeightSize * HeightCount * 0.5f);

    /// <summary>
    /// 씬 이름 조합용 기본 문자열
    /// </summary>
    const string SceneNameBase = "Seemless";

    /// <summary>
    /// 모든 씬의 이름을 저장해 놓은 배열
    /// </summary>
    string[] sceneNames;

    enum SceneLoadState : byte
    {
        Unload = 0,     // 로딩이 안되어 있는 상태(로딩 해제 완료된 상태)
        PendingUnload,  // 로딩 해제 진행 중인 상태
        PendingLoad,    // 로딩 진행 중인 상태
        Loaded          // 로딩 완료된 상태
    }

    /// <summary>
    /// 모든 씬의 로딩 진행 상태를 저장해 놓은 배열
    /// </summary>
    SceneLoadState[] sceneLoadStates;

    /// <summary>
    /// 로딩 요청이 들어온 씬의 목록
    /// </summary>
    List<int> loadWork = new List<int>();

    /// <summary>
    /// 로딩이 완료된 씬의 목록
    /// </summary>
    List<int> loadWorkComplete = new List<int>();

    /// <summary>
    /// 로딩 해제 요청이 들어온 씬의 목록
    /// </summary>
    List<int> unloadWork = new List<int>();

    /// <summary>
    /// 로딩 해제가 완료된 씬의 목록
    /// </summary>
    List<int> unloadWorkComplete = new List<int>();

    /// <summary>
    /// 모든 씬이 언로드 되었음을 확인해주는 프로퍼티(모든 씬이 Unload 상태면 true, 아니면 false)
    /// </summary>
    public bool IsUnloadAll
    {
        get
        {
            bool result = true;
            foreach(SceneLoadState state in sceneLoadStates)
            {
                if(state != SceneLoadState.Unload)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 처음 생성되었을 때 한번만 실행되는 함수
    /// </summary>
    public void PreInitialize()
    {
        int mapCount = HeightCount * WidthCount;
        sceneNames = new string[mapCount];
        sceneLoadStates = new SceneLoadState[mapCount];

        for (int y = 0; y < HeightCount; y++)
        {
            for (int x = 0; x < WidthCount; x++)
            {
                int index = GetIndex(x, y);
                sceneNames[index] = $"{SceneNameBase}_{x}_{y}";
                sceneLoadStates[index] = SceneLoadState.Unload;
            }
        }
    }

    /// <summary>
    /// 씬이 Single모드로 로드될 때마다 호출될 초기화 함수
    /// </summary>
    public void Initialize()
    {
        for(int i = 0; i < sceneLoadStates.Length; i++)
        {
            sceneLoadStates[i] = SceneLoadState.Unload;     // 맵 로드 상태 전부 unload로 초기화
        }

        // 플레이어 관련 초기화
    }

    /// <summary>
    /// 특정 서브맵의 비동기 로딩을 요청하는 함수
    /// </summary>
    /// <param name="x">서브맵의 x위치</param>
    /// <param name="y">서브맵의 y위치</param>
    void RequestAsyncSceneLoad(int x, int y)
    {
        int index = GetIndex(x, y);
        if (sceneLoadStates[index] == SceneLoadState.Unload)    // 해당 서브맵이 Unload 상태일 때만 작업 리스트에 추가
        {
            loadWork.Add(index);
        }
    }

    /// <summary>
    /// 특정 서브맵의 비동기 로딩해제를 요청하는 함수
    /// </summary>
    /// <param name="x">서브맵의 x위치</param>
    /// <param name="y">서브맵의 y위치</param>
    void RequestAsyncSceneUnload(int x, int y)
    {
        int index = GetIndex(x, y);
        if (sceneLoadStates[index] == SceneLoadState.Loaded)    // 해당 서브맵이 Loaded 상태일 때만 작업 리스트에 추가
        {
            unloadWork.Add(index);
        }
    }

    void AsyncSceneLoad(int index)
    {
        if (sceneLoadStates[index] == SceneLoadState.Unload)        // 이미 로딩이 시작되었거나 완료된 것은 처리 안하기 위해
        {
            sceneLoadStates[index] = SceneLoadState.PendingLoad;    // 진행 중이라고 표시

            AsyncOperation async = SceneManager.LoadSceneAsync(sceneNames[index], LoadSceneMode.Additive);
            async.completed += (_) =>       // 비동기 작업이 완료되면 실행되는 델리게이트에 람다 함수 추가
            {
                sceneLoadStates[index] = SceneLoadState.Loaded; // 완료되었으니 Loaded 상태로 변경
                loadWorkComplete.Add(index);                    // 완료 리스트에 인덱스 추가
            };
        }
    }

    void AsyncSceneUnload(int index)
    {
        if (sceneLoadStates[index] == SceneLoadState.Loaded)        // 이미 로딩해체이 시작되었거나 완료된 것은 처리 안하기 위해
        {
            sceneLoadStates[index] = SceneLoadState.PendingUnload;    // 진행 중이라고 표시

            // 해제할 씬에 있는 슬라임을 풀로 되돌리기(씬 언로드로 삭제되는 것 방지)
            Scene scene = SceneManager.GetSceneByName(sceneNames[index]);
            if(scene.isLoaded)
            {
                GameObject[] sceneRoots = scene.GetRootGameObjects();   // 루트 오브젝트 모두 찾기(부모가 없는 오브젝트 모두 찾기)
                if(sceneRoots != null && sceneRoots.Length > 0)
                {
                    // 서브맵은 루트오브젝트가 Grid하나만 있음
                    Slime[] slimes = sceneRoots[0].GetComponentsInChildren<Slime>();    // 루트 오브젝트의 자손으로 있는 모든 슬라임 찾기
                    foreach(Slime slime in slimes)
                    {
                        slime.ReturnToPool();
                    }
                }

            }

            // 씬 로딩 해제
            AsyncOperation async = SceneManager.UnloadSceneAsync(sceneNames[index]);
            async.completed += (_) =>       // 비동기 작업이 완료되면 실행되는 델리게이트에 람다 함수 추가
            {
                sceneLoadStates[index] = SceneLoadState.Unload;     // 완료되었으니 Loaded 상태로 변경
                unloadWorkComplete.Add(index);                    // 완료 리스트에 인덱스 추가
            };
        }
    }

    private void Update()
    {
        // 완료된 작업은 작업리스트에서 제거
        foreach(int index in loadWorkComplete)
        {
            loadWork.RemoveAll((x) => x == index);  // loadWork에서 값이 index인 항목은 모두 제거
        }
        loadWorkComplete.Clear();       // 다 제거했으니 리스트 비우기

        // 들어온 요청 처리
        foreach(int index in loadWork)
        {
            AsyncSceneLoad(index);      // 비동기 로딩 시작
        }

        // 완료된 작업은 작업리스트에서 제거
        foreach (int index in unloadWorkComplete)
        {
            unloadWork.RemoveAll((x) => x == index);  // loadWork에서 값이 index인 항목은 모두 제거
        }
        unloadWorkComplete.Clear();     // 다 제거했으니 리스트 비우기

        // 들어온 요청 처리
        foreach (int index in unloadWork)
        {
            AsyncSceneUnload(index);    // 비동기 로딩 해제 시작
        }
    }

    /// <summary>
    /// 지정된 위치 주변 맵은 로딩 요청하고, 그 외의 맵은 로딩해제를 요청하는 함수
    /// </summary>
    /// <param name="x">서브맵의 X위치</param>
    /// <param name="y">서브맵의 Y위치</param>
    public void RefreshScenes(int subX, int subY)
    {
        // (0, 0) ~ (WidthCount, HeightCount) 사이만 범위로 설정
        int startX = Mathf.Max(0, subX - 1);
        int endX = Mathf.Min(WidthCount, subX + 2);
        int startY = Mathf.Max(0, subY - 1);
        int endY = Mathf.Min(WidthCount, subY + 2);

        List<Vector2Int> opens = new List<Vector2Int>(9);
        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                RequestAsyncSceneLoad(x, y);        // start ~ end 안에 있는 것은 모두 로딩 요청
                opens.Add(new Vector2Int(x, y));
            }
        }

        for (int y = 0; y < HeightCount; y++)
        {
            for (int x = 0; x < WidthCount; x++)
            {
                if (!opens.Contains(new Vector2Int(x, y)))  // 로딩 요청한 것이 아니면 모두 로딩 해제
                {
                    RequestAsyncSceneUnload(x, y);
                }
            }
        }
    }

    /// <summary>
    /// 맵의 그리드 좌표를 인덱스로 변경해주는 함수
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>배열용 인덱스 값</returns>
    int GetIndex(int x, int y)
    {
        return x + y * WidthCount;
    }

    /// <summary>
    /// 월드 좌표가 어떤 맵에 속하는지 계산하는 함수
    /// </summary>
    /// <param name="world">확인할 월드 좌표</param>
    /// <returns>서브맵의 좌표((0,0)~(2,2))</returns>
    public Vector2Int WorldToGrid(Vector3 world)
    {
        Vector2 offset = (Vector2)world - worldOrigine;
        return new Vector2Int((int)(offset.x / submapWidthSize), (int)(offset.y / submapHeightSize));
    }

#if UNITY_EDITOR
    public void Test_LoadScene(int x, int y)
    {
        RequestAsyncSceneLoad(x, y);
    }

    public void Test_UnloadScene(int x, int y)
    {
        RequestAsyncSceneUnload(x, y);
    }

    public void Test_UnloadAll()
    {
        for(int y = 0; y < HeightCount;y++)
        {
            for(int x = 0; x < WidthCount;x++)
            {
                RequestAsyncSceneUnload(x, y);
            }
        }
    }

    public void Test_RefreshScenes(int x, int y)
    {
        RefreshScenes(x, y);
    }
#endif
}
