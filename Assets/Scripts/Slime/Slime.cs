using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : RecycleObject
{
    /// <summary>
    /// 페이즈 진행 시간(등장 연출 시간)
    /// </summary>
    public float phaseDuration = 0.5f;

    /// <summary>
    /// 디졸브 진행 시간(사망 연출 시간)
    /// </summary>
    public float dissolveDuration = 1.0f;

    /// <summary>
    /// 아웃라인 기본 두깨
    /// </summary>
    public float VisibleOutlineThickness = 0.5f;

    /// <summary>
    /// 페이즈의 기본 두깨
    /// </summary>
    public float VisiblePhaseThickness = 0.1f;

    /// <summary>
    /// 머테리얼
    /// </summary>
    Material mainMaterial;

    // 쉐이더 프로퍼티용 ID들
    readonly int OutlineThicknessID = Shader.PropertyToID("_OutlineThickness");
    readonly int PhaseSplitID = Shader.PropertyToID("_PhaseSplit");
    readonly int PhaseThicknessID = Shader.PropertyToID("_PhaseThickness");
    readonly int DissolveFadeID = Shader.PropertyToID("_DessolveFade");

    // 슬라임은 풀로 관리된다. 팩토리를 이용해 생성할 수 있다.

    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        mainMaterial = spriteRenderer.material;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 스폰될 때 Phase 작동
        StartCoroutine(StartPhase());
    }


    protected override void OnReset()
    {
        ShowOutline(false);         // 아웃라인 끄기
        mainMaterial.SetFloat(PhaseThicknessID, VisiblePhaseThickness); // 페이즈 두깨 원상복구 시키기
        mainMaterial.SetFloat(PhaseSplitID, 1);                      // 페이즈 시작 값으로 설정
        mainMaterial.SetFloat(DissolveFadeID, 1);                    // 디졸브 시작 값으로 설정
    }

    protected override void OnDisable()
    {
        // ReturnToPool()에서 할 일을 여기로
        base.OnDisable();
    }

    IEnumerator StartPhase()
    {
        // PhaseSplitID를 1 -> 0으로 만들기

        float phaseNormalize = 1.0f / phaseDuration;    // 나누기 연산을 줄이기 위해 미리 계산
        float timeElapsed = 0.0f;                       // 시간 누적용


        while (timeElapsed < phaseDuration)             // 시간 될때까지 반복
        {
            timeElapsed += Time.deltaTime;              // 시간 누적

            //mainMaterial.SetFloat(PhaseSplitID, 1.0f - (timeElapsed / phaseDuration));
            mainMaterial.SetFloat(PhaseSplitID, 1.0f - (timeElapsed * phaseNormalize));     // split값을 1 -> 0으로 점점 감소시키기

            yield return null;
        }

        mainMaterial.SetFloat(PhaseThicknessID, 0);     // 페이즈 선 안보이게 만들기
        mainMaterial.SetFloat(PhaseSplitID, 0);         // 숫자 0으로 정리하기
    }

    public void Die()
    {
        // 죽을 때 Dissolve 작동
        StartCoroutine(StartDissolve());
    }

    IEnumerator StartDissolve()
    {
        // DissolveFade를 1 -> 0으로 만들기

        float fadeNormalize = 1.0f / dissolveDuration;    // 나누기 연산을 줄이기 위해 미리 계산
        float timeElapsed = 0.0f;                       // 시간 누적용


        while (timeElapsed < dissolveDuration)             // 시간 될때까지 반복
        {
            timeElapsed += Time.deltaTime;              // 시간 누적

            mainMaterial.SetFloat(DissolveFadeID, 1 - (timeElapsed * fadeNormalize));     // Fade값을 1 -> 0으로 점점 감소시키기

            yield return null;
        }

        mainMaterial.SetFloat(DissolveFadeID, 0);         // 숫자 0으로 정리하기

        gameObject.SetActive(false);                      // 게임 오브젝트 비활성화
    }


    /// <summary>
    /// 아웃라인을 보여줄지 말지 결정하는 함수
    /// </summary>
    /// <param name="isShow">true면 보여주고 false면 안 보여준다</param>
    public void ShowOutline(bool isShow = true)
    {
        //if (isShow)
        //{
        //    mainMaterial.SetFloat(OutlineThicknessID, 0.6f);
        //}
        //else
        //{
        //    mainMaterial.SetFloat(OutlineThicknessID, 0.0f);
        //}

        // isShow가 true면 VisibleOutlineThickness, 아니면 0으로 세팅
        mainMaterial.SetFloat(OutlineThicknessID, isShow ? VisibleOutlineThickness : 0);
    }
}
