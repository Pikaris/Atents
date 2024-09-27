using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ImageNumber : MonoBehaviour
{
    /// <summary>
    /// 숫자 스프라이트들(0~9)
    /// </summary>
    public Sprite[] numberImages;

    /// <summary>
    /// 자리수별 이미지 컴포넌트들(0: 1자리, 1: 10자리, 2: 100자리, 3: 1000자리, 4: 10000자리)
    /// </summary>
    Image[] digits;

    /// <summary>
    /// 보여줄 숫자
    /// </summary>
    int number = -1;

    /// <summary>
    /// 보여줄 숫자를 확인하고 설정하는 프로퍼티
    /// </summary>
    public int Number
    {
        get => number;
        set
        {
            if (number != value)
            {
                number = Mathf.Clamp(value, 0, 99999);
            }

            // number에 설정된 값대로 이미지를 설정한다
            // number의 범위는 0~99999
            // number가 123일 경우 만자리와 천자리는 없어야 한다
            // number가 0일 경우 일의 자리에는 0이 나와야 한다

            int temp = number;

            //digits[0].sprite = numberImages[number % 10];

            for (int i = 0; i < digits.Length; i++)
            {
                if (temp != 0 || i == 0)
                {
                    digits[i].sprite = numberImages[temp % 10];
                    temp /= 10;
                    digits[i].gameObject.SetActive(true);
                }
                else
                {
                    digits[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void Awake()
    {
        digits = GetComponentsInChildren<Image>();
    }
}
