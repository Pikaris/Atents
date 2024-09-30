using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    /// <summary>
    /// 포스트프로세스가 적용되는 볼륨
    /// </summary>
    Volume postProcessVolume;

    /// <summary>
    /// 볼륨 안에 있는 비네트
    /// </summary>
    Vignette vignette;

    /// <summary>
    /// 비네트 정도를 조절하기 위한 커브
    /// </summary>
    public AnimationCurve curve;

    private void Awake()
    {
        postProcessVolume = GetComponent<Volume>();
        postProcessVolume.profile.TryGet<Vignette>(out vignette);
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;
        player.onLifeTimeChange += OnLifeTimeChange;

        //Time.timeScale = 0.1f;
    }

    /// <summary>
    /// 플레이어 수명이 변경될 때마다 실행될 함수
    /// </summary>
    /// <param name="ratio"></param>
    private void OnLifeTimeChange(float ratio)
    {
        //vignette.intensity.value = 1 - ratio;
        vignette.intensity.value = curve.Evaluate(ratio);
    }
}
