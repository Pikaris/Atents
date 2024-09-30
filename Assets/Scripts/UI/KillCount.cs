using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCount : MonoBehaviour
{
    /// <summary>
    /// 숫자 증가 속도
    /// </summary>
    public float countingSpeed = 5.0f;

    /// <summary>
    /// 목표 점수
    /// </summary>
    float target = 0.0f;

    /// <summary>
    /// 현재 보일 점수
    /// </summary>
    float current = 0.0f;

    ImageNumber imageNumber;

    private void Awake()
    {
        imageNumber = GetComponent<ImageNumber>();
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;
        player.onKillCountChange += OnKillCountChange;
    }

    private void Update()
    {
        current += Time.deltaTime * countingSpeed;      // 속도에 따라 증가
        if (current > target)
        {
            current = target;                           // target까지만 증가
        }
        imageNumber.Number = Mathf.FloorToInt(current);
    }

    private void OnKillCountChange(int count)
    {
        //imageNumber.Number = count;
        target = count;
    }
}
