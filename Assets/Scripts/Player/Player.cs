using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 이동 속도
    /// </summary>
    public float speed = 3.0f;

    /// <summary>
    /// 공격 간격(쿨타임)
    /// </summary>
    public float attackInterval = 1.0f;

    /// <summary>
    /// 현재 공격 쿨타임
    /// </summary>
    float attackCoolTime = 0.0f;

    /// <summary>
    /// 공격 쿨타임이 다 되었는지 확인하기 위한 프로퍼티
    /// </summary>
    bool IsAttackReady => attackCoolTime < 0.0f;

    /// <summary>
    /// 현재 속도
    /// </summary>
    float currentSpeed = 3.0f;

    /// <summary>
    /// AttackSensor의 축
    /// </summary>
    Transform attackSensorAxis;

    /// <summary>
    /// 지금 공격이 유효한 상태인지 확인하는 변수
    /// </summary>
    bool isAttackValid = false;

    /// <summary>
    /// 현재 내 공격 범위 안에 있는 모든 슬라임의 목록
    /// </summary>
    List<Slime> attackTargetList;

    AttackSensor sensor;

    /// <summary>
    /// 입력 받은 방향
    /// </summary>
    Vector2 inputDirection = Vector2.zero;

    // 인풋 액션
    PlayerInputActions inputActions;

    SubmapManager submapManager;

    // 필요 컴퍼넌트들
    Rigidbody2D rigid;
    Animator animator;

    // 애니메이션용 해시
    readonly int InputX_Hash = Animator.StringToHash("InputX");
    readonly int InputY_Hash = Animator.StringToHash("InputY");
    readonly int IsMove_Hash = Animator.StringToHash("IsMove");
    readonly int Attack_Hash = Animator.StringToHash("Attack");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();

        attackSensorAxis = transform.GetChild(0);
        sensor = attackSensorAxis.GetComponentInChildren<AttackSensor>();
        sensor.onSlimeEnter += (slime) =>       // 공격 범위에 슬라임이 들어왔을 때
        {
            if(isAttackValid)
            {
                slime.Die();                    // 공격이 유효할 때 영역안에 들어오면 즉시 사망
            }
            else
            {
                attackTargetList.Add(slime);    // 공격이 유효하지 않으면 일단 리스트에 추가
            }
            
            slime.ShowOutline(true);            // 아웃라인 표시하기
        };
        sensor.onSlimeExit += (slime) =>        // 공격 범위에서 슬라임이 나갔을 때
        {
            attackTargetList.Remove(slime);     // 공격 대상 리스트에서 제거
            slime.ShowOutline(false);           // 아웃라인 끄기
        };
        
        attackTargetList = new List<Slime>(4);

        inputActions = new PlayerInputActions();

        currentSpeed = speed;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        submapManager = FindAnyObjectByType<SubmapManager>();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();          // 입력 받은 방향 저장

        animator.SetFloat(InputX_Hash, inputDirection.x);       // 애니메이터에 방향 전달
        animator.SetFloat(InputY_Hash, inputDirection.y);       
        animator.SetBool(IsMove_Hash, true);                    // 애니메이터에 움직이기 시작했다고 알림

        Debug.Log(inputDirection);

        AttackSensorRotate(inputDirection);
    }

    private void OnStop(InputAction.CallbackContext context)
    {
        inputDirection = Vector2.zero;                          // 입력 방향 초기화

        // x,y 세팅 안함(이동 방향을 계속 바라보게 하기 위해)

        animator.SetBool(IsMove_Hash, false);                   // 애니메이터에 멈췄다고 알림
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (IsAttackReady)
        {
            animator.SetTrigger(Attack_Hash);                       // 애니메이터에 공격 알림
            attackCoolTime = attackInterval;
            currentSpeed = 0.0f;
        }
    }
    private void Update()
    {
        attackCoolTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + inputDirection * Time.fixedDeltaTime * currentSpeed);
        //좀 무식
        //submapManager.RefreshScenes(submapManager.WorldToGrid(rigid.position).x, submapManager.WorldToGrid(rigid.position).y);
    }

    /// <summary>
    /// 공격 애니메이션 진행 중에 공격이 유효해지면 애니메이션 이벤트로 실행 할 함수
    /// </summary>
    void AttackValid()
    {
        isAttackValid = true;                  // 유효하다고 표시하고
        foreach (var slime in attackTargetList)
        {
            slime.Die();                        // 범위 안에 있던 모든 슬라임 죽이기
        }
        attackTargetList.Clear();
    }

    void AttackNotValid()
    {
        isAttackValid = false;
    }

    /// <summary>
    /// 속도를 원상복귀 시키는 함수
    /// </summary>
    public void RestoreSpeed()
    {
        currentSpeed = speed;
    }

    /// <summary>
    /// 입력 방향에 따라 AttackSensor를 회전시키는 함수
    /// </summary>
    /// <param name="direction"></param>
    void AttackSensorRotate(Vector2 direction)
    {
        if (direction.y < 0.0f)
        {
            attackSensorAxis.rotation = Quaternion.identity;
        }
        else if (direction.y > 0.0f)
        {
            attackSensorAxis.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction.x < 0.0f)
        {
            attackSensorAxis.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (direction.x > 0.0f)
        {
            attackSensorAxis.rotation = Quaternion.Euler(0, 0, 90);
        }

        //if (direction.sqrMagnitude > 0.01f)
        //{
        //    float angle = Vector2.SignedAngle(Vector2.down, direction);
        //    attackSensorAxis.rotation = Quaternion.Euler(0, 0, angle);
        //}
    }
}

// 1. 캐릭터 실제로 이동 시키기
// 2. 공격 쿨타임 추가하기
// 3. 공격 중 이동 안하기
