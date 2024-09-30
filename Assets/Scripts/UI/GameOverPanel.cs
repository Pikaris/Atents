using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public float alphaChangeSpeed = 2.0f;

    CanvasGroup canvasGroup;

    TextMeshProUGUI playTime;
    TextMeshProUGUI killCount;
    Button restart;

    Player player;

    // 1. 플레이어가 죽으면 alphaChangeSpeed속도에 따라 캔버스 그룹의 알파값이 증가한다.
    // 2. 플레이어가 죽으면 플레이어의 전체 플레이 시간과 킬 수를 UI에서 갱신해야 한다.
    // 3. 플레이어가 죽은 이후에만 버튼이 눌러져야 한다.
    // 4. 버튼을 누르면 모든 씬이 Unload된 이후에 LoadingScene을 로딩한다.

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        Transform child = transform.GetChild(1);
        playTime = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(2);
        killCount = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(3);
        restart = child.GetComponent<Button>();

        restart.onClick.AddListener( () => StartCoroutine(WaitUnloadAll()));
    }

    private void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        player = GameManager.Instance.Player;
        player.onDie += onPlayerDie;
    }

    private void onPlayerDie()
    {
        playTime.text = $"Total Play Time\n\r< {player.TotalPlayTime:f1} Sec >";
        killCount.text = $"Total Kill Count\n\r< {player.KillCount} Kill >";
        StartCoroutine(StartAlphaChange());
    }

    IEnumerator StartAlphaChange()
    {
        while (canvasGroup.alpha < 1.0f)
        {
            canvasGroup.alpha += alphaChangeSpeed * Time.deltaTime;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    IEnumerator WaitUnloadAll()
    {
        SubmapManager sub = GameManager.Instance.SubmapManager;
        while(!sub.IsUnloadAll)
        {
            yield return null;
        }
        SceneManager.LoadScene("LoadingScene");
    }
}
