using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test13_Scene_AsyncLoad : TestBase
{
    public string sceneName = "LoadSampleScene";

    AsyncOperation async;       // 비동기 관련 정보나 명령을 내리기 위한 객체. 비동기 함수의 return값으로 받음

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(sceneName);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;     // 비동기 씬 로딩 작업이 완료되어도 자동으로 씬 전환을 하지 않는다.
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        async.allowSceneActivation = true;      // 비동기 씬 로딩이 완료되면 자동으로 씬 전환을 한다.
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine()
    {
        async = SceneManager.LoadSceneAsync(sceneName); // 비동기 로딩 시작
        async.allowSceneActivation = false;             // 자동으로 씬 전환 되는 걸 막기

        while(async.progress < 0.9f)                    // allowSceneActivation가 false면 progress는 0.9가 최대
        {
            Debug.Log($"Progress : {async.progress}");
            yield return null;
        }
        Debug.Log("Loading Complete");
    }
}
