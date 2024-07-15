using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("MoveSpeed")] 
    [SerializeField] private float walkSpeed;

    [Header("ClickPosition")]
    [SerializeField] private Transform clickTransform;

    #region PlayerComponent
    private Rigidbody _rigidBody;
    private NavMeshAgent _agent;
    private Animator _animator;
    #endregion

    #region PlayerValue
    private float _rotationVelocity;
    private bool isMove;
    #endregion

    private Ray _mouseRay;
    private Camera _mainCamera;
    public bool IsMove
    {
        get { return isMove; }
        set { isMove = value; }
    }
    

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        PlayerMovement();
    }

    // 초기화 메서드
    private void Initialize()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _mainCamera = Camera.main;
        _agent.speed = walkSpeed;
        clickTransform.gameObject.SetActive(false);
        isMove = true;
    }

    //플레이어 이동
    private void PlayerMovement()
    {
        if (isMove)
        {
            ClickMove();
        }
    }

    private void ClickMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_mouseRay, out RaycastHit hit, Mathf.Infinity))
            {
                _agent.SetDestination(hit.point);

                clickTransform.position = new Vector3(hit.point.x, 0.01f, hit.point.z);

                ActiveTargetObject(true);
            }
        }
        else if (_agent.remainingDistance < 0.1f)
        {
            ActiveTargetObject(false);
        }

        AnimationMoveMent();
    }

    //이동 애니메이션
    private void AnimationMoveMent()
    {
        Vector3 currentVelocity = _agent.velocity;

        float speed = currentVelocity.magnitude;

        _animator.SetFloat("Move", speed);
    }

    //타겟 오브젝트 활성화 여부
    private void ActiveTargetObject(bool isActive)
    {
        clickTransform.gameObject.SetActive(isActive);
    }

    // 회전을 처리하는 메서드 (현재 사용X, 삭제 X)
    private void RotateTowardsDirection(Vector3 direction)
    {
        if (_rigidBody.velocity == Vector3.zero) return;

        float targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity, 0.12f);

        transform.rotation = Quaternion.Euler(0, smoothRotation, 0);
    }
}
