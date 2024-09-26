using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//// 1.LoadingText에 표시되는 글자가
////   "Loading", "Loading.", "Loading..", "Loading...", "Loading....", "Loading....."가 tickTime마다 변경되면서 반복된다

//// 2. slider의 value가 loadingBarSpeed의 속도로 증가한다
//// 3. slider의 value가 1이 되기 전에 로딩이 완료되면 slider의 value가 1이 될 때까지 기다린다
//// 4. 로딩이 완료되면 LoadingText에 표시되는 글자가 "Loading Complete!"로 변경된다
//// 5. 로딩이 완료된 이후에 마우스나 키보드 중 아무거나 눌려지면 씬 전환이 일어난다

public class AsyncLoadingBackground : MonoBehaviour
{
    /// <summary>
    /// 로딩할 다음 씬의 이름
    /// </summary>
    public string nextSceneName = "LoadSampleScene";

    /// <summary>
    /// loadingText에 글자가 변경되는 간격
    /// </summary>
    public float tickTIme = 0.2f;

    /// <summary>
    /// slider의 value가 증가하는 속도
    /// </summary>
    public float loadingBarSpeed = 1.0f;

    /// <summary>
    /// 로딩이 완료되었는지를 표시하는 변수(true면 로딩완료, false면 로딩 중)
    /// </summary>
    bool loadingDone = false;

    /// <summary>
    /// 비동기 명령 처리를 위해 필요한 객체
    /// </summary>
    AsyncOperation async;

    // UI
    TextMeshProUGUI loadingText;
    Slider loadingSlider;

    // 입력용
    PlayerInputActions inputActions;



    private void Awake()
    {
        inputActions = new PlayerInputActions();
        loadingText = GetComponentInChildren<TextMeshProUGUI>();
        loadingSlider = GetComponentInChildren<Slider>();
    }


    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.AnyButton.performed += OnPressAnyKey;
    }

    private void OnDisable()
    {
        inputActions.UI.AnyButton.performed -= OnPressAnyKey;
        inputActions.UI.Disable();
    }

    private void Start()
    {
        async = SceneManager.LoadSceneAsync(nextSceneName); // 비동기 로딩 시작
        async.allowSceneActivation = false;                 // 자동 씬 전환 막기

        StartCoroutine(LoadingTextProgress());              // 글자 표시용 코루틴 시작
        StartCoroutine(LoadingSliderProgress());            // 슬라이더 조절용 코루틴 시작
    }

    /// <summary>
    /// Loading 글자를 출력하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingTextProgress()
    {
        // 출력할 글자들 미리 준비하기
        string[] texts =
        {
            "Loading",
            "Loading.",
            "Loading..",
            "Loading...",
            "Loading....",
            "Loading....."
        };
        WaitForSeconds wait = new WaitForSeconds(tickTIme);     // tickTime동안 기다리기 위해 만들기

        int index = 0;          // 출력할 문자열의 인덱스
        while(!loadingDone)     // 로딩 중이면 계속 반복
        {
            loadingText.text = texts[index];        // tickTime 간격으로 인덱스 변경하며 문자열 출력하기
            index++;
            index %= texts.Length;
            yield return wait;
        }

        loadingText.text = "Loading\nComplete!";     // 로딩이 완료되었으면 완료 출력
    }

    /// <summary>
    /// 슬라이더를 변경하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingSliderProgress()
    {
        loadingSlider.value = 0;        // 초기값 설정

        while (async.progress < 0.9f)   // 로딩 완료 전에는 속도에 맞춰서 슬라이더 계속 증가
        {
            loadingSlider.value += Time.deltaTime * loadingBarSpeed;
            yield return null;
        }

        // 로딩이 완료된 이후의 처리(씬은 백그라운드에서 대기중)
        float elapsedTime = 0.0f;
        float remainTime = (1 - loadingSlider.value) / loadingBarSpeed;     // 슬라이더 증가 속도와 슬라이더의 남은 양에 따라 대기해야 할 시간 계산

        while(remainTime > elapsedTime)     // 남은 시간동안 슬라이더 증가 처리
        {
            elapsedTime += Time.deltaTime;  // 진행 시간 누적
            loadingSlider.value += Time.deltaTime * loadingBarSpeed;    // 슬라이더는 원래 증가 속도에 따라 계속 증가
            yield return null;
        }

        loadingDone = true;     // 로딩 완료 표시(Complete 글자 출력과 타이밍을 맞추기 위해 사용)
    }

    private void OnPressAnyKey(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        async.allowSceneActivation = loadingDone;   // UI에서 표시되는 것과 타이밍을 맞춰서 씬 전환 하기 위한 용도
    }




    //////////////////////////////////////////// 내 코드 ////////////////////////////////////////////

    //float elapsedTime;

    //int dotIndex = 0;

    //string loadingText = "Loading.....";
    //string loading;
    //string dot;

    //AsyncOperation async;

    //StringBuilder stringbuilder;

    //TextMeshProUGUI textMeshProUGUI;

    //Slider slider;

    //PlayerInputActions playerInputAction;

    //bool waitingNextScene = false;



    //private void Awake()
    //{
    //    Transform child = transform.GetChild(0);
    //    textMeshProUGUI = child.GetComponent<TextMeshProUGUI>();

    //    child = transform.GetChild(1);
    //    slider = child.GetComponent<Slider>();

    //    playerInputAction = new PlayerInputActions();

    //    loading = loadingText.Substring(0, 7);
    //    dot = loadingText.Substring(loadingText.IndexOf('g') + 1).Trim();

    //    stringbuilder = new StringBuilder();
    //    stringbuilder.Append(loading);

    //    slider.value = 0;
    //}
    //private void Start()
    //{
    //    StartCoroutine(LoadScene());
    //}

    //private void OnEnable()
    //{
    //    playerInputAction.UI.Enable();
    //    playerInputAction.UI.AnyButton.performed += OnAnyButton;
    //}


    //private void Update()
    //{
    //    elapsedTime += Time.deltaTime;

    //    if (slider.value < 1)
    //    {
    //        slider.value += loadingBarSpeed * Time.deltaTime;
    //        DisplayLoadingProgress();
    //    }
    //    else
    //    {
    //        waitingNextScene = true;
    //        textMeshProUGUI.text = "Loading Complete!";
    //    }
    //}

    //void DisplayLoadingProgress()
    //{
    //    if (elapsedTime > tickTIme)
    //    {
    //        if (dotIndex < dot.Length)
    //        {
    //            stringbuilder.Append(dot[dotIndex]);
    //            textMeshProUGUI.text = stringbuilder.ToString();
    //            elapsedTime = 0;
    //            dotIndex++;
    //        }
    //        else
    //        {
    //            stringbuilder.Clear();
    //            stringbuilder.Append(loading);
    //            dotIndex = 0;
    //        }
    //    }
    //}

    //private void OnAnyButton(InputAction.CallbackContext context)
    //{
    //    if (waitingNextScene)
    //    {
    //        async.allowSceneActivation = true;
    //    }
    //}

    //IEnumerator LoadScene()
    //{
    //    async = SceneManager.LoadSceneAsync(nextSceneName);
    //    async.allowSceneActivation = false;

    //    yield return null;
    //}
}
