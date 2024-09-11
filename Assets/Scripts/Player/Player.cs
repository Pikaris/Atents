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
    /// 입력 받은 방향
    /// </summary>
    Vector2 inputDirection = Vector2.zero;

    // 인풋 액션
    PlayerInputActions inputActions;

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

    private void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();          // 입력 받은 방향 저장

        animator.SetFloat(InputX_Hash, inputDirection.x);       // 애니메이터에 방향 전달
        animator.SetFloat(InputY_Hash, inputDirection.y);       
        animator.SetBool(IsMove_Hash, true);                    // 애니메이터에 움직이기 시작했다고 알림
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

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + inputDirection * Time.fixedDeltaTime * currentSpeed);
    }
    private void Update()
    {
        attackCoolTime -= Time.deltaTime;
    }

    public void RestoreSpeed()
    {
        currentSpeed = speed;
    }
}

// 1. 캐릭터 실제로 이동 시키기
// 2. 공격 쿨타임 추가하기
// 3. 공격 중 이동 안하기
