using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadingBackground : MonoBehaviour
{
    public string nextSceneName = "LoadSampleScene";

    public float tickTIme = 0.2f;

    /// <summary>
    /// slider의 value가 증가하는 속도
    /// </summary>
    public float loadingBarSpeed = 1.0f;

    float elapsedTime;

    int dotIndex = 0;

    string loadingText = "Loading.....";
    string loading;
    string dot;

    AsyncOperation async;

    StringBuilder stringbuilder;

    TextMeshProUGUI textMeshProUGUI;

    Slider slider;

    PlayerInputActions playerInputAction;

    bool waitingNextScene = false;

    // 1.LoadingText에 표시되는 글자가
    //   "Loading", "Loading.", "Loading..", "Loading...", "Loading....", "Loading....."가 tickTime마다 변경되면서 반복된다

    // 2. slider의 value가 loadingBarSpeed의 속도로 증가한다
    // 3. slider의 value가 1이 되기 전에 로딩이 완료되면 slider의 value가 1이 될 때까지 기다린다
    // 4. 로딩이 완료되면 LoadingText에 표시되는 글자가 "Loading Complete!"로 변경된다
    // 5. 로딩이 완료된 이후에 마우스나 키보드 중 아무거나 눌려지면 씬 전환이 일어난다

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        textMeshProUGUI = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(1);
        slider = child.GetComponent<Slider>();

        playerInputAction = new PlayerInputActions();

        loading = loadingText.Substring(0, 7);
        dot = loadingText.Substring(loadingText.IndexOf('g') + 1).Trim();

        stringbuilder = new StringBuilder();
        stringbuilder.Append(loading);

        slider.value = 0;
    }
    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    private void OnEnable()
    {
        playerInputAction.UI.Enable();
        playerInputAction.UI.AnyButton.performed += OnAnyButton;
    }


    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (slider.value < 1)
        {
            slider.value += loadingBarSpeed * Time.deltaTime;
            DisplayLoadingDots();
        }
        else
        {
            waitingNextScene = true;
            textMeshProUGUI.text = "Loading Complete!";
        }
    }

    void DisplayLoadingDots()
    {
        if (elapsedTime > tickTIme)
        {
            if (dotIndex < dot.Length)
            {
                stringbuilder.Append(dot[dotIndex]);
                textMeshProUGUI.text = stringbuilder.ToString();
                elapsedTime = 0;
                dotIndex++;
            }
            else
            {
                stringbuilder.Clear();
                stringbuilder.Append(loading);
                dotIndex = 0;
            }
        }
    }

    private void OnAnyButton(InputAction.CallbackContext context)
    {
        if (waitingNextScene)
        {
            async.allowSceneActivation = true;
        }
    }

    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false;

        yield return null;
    }
}
