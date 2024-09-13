using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : RecycleObject
{
    // 슬라임은 풀로 관리된다. 팩토리를 이용해 생성할 수 있다.

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    //private void OnEnable()
    //{
    //    // 스폰될 때 Phase 작동
    //}

    public void Die()
    {
        // 죽을 때 Dissolve 작동
    }


    /// <summary>
    /// 아웃라인을 보여줄지 말지 결정하는 함수
    /// </summary>
    /// <param name="isShow">true면 보여주고 false면 안 보여준다</param>
    public void ShowOutline(bool isShow = true)
    {

    }
}
