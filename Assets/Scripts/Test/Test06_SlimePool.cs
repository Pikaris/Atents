using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test06_SlimePool : TestBase
{
    public SpriteRenderer spriterRenderer;

    Material material;

    readonly int OutlineThickness_ID = Shader.PropertyToID("_OutlineThickness");

    private void Start()
    {
        material = spriterRenderer.sharedMaterial;

        material.SetFloat(OutlineThickness_ID, 0);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // 팩토리 이용해서 슬라임을 하나 꺼내기((-8, -4) ~ (8, 4) 영역 안에서 랜덤) 
        Factory.Instance.GetSlime(new Vector3(Random.Range(-8, 8), Random.Range(-4, 4), 0));
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // 씬에 있는 모든 슬라임의 아웃라인이 보인다.
        material.SetFloat(OutlineThickness_ID, 0.6f);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // 씬에 있는 모든 슬라임의 아웃라인이 안보인다.
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        // 씬에 있는 모든 슬라임이 죽는다.
    }
}
