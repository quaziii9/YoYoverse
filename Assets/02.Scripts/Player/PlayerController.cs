using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("MoveSpeed")] 
    [SerializeField] private float walkSpeed;

    #region PlayerComponent
    private Rigidbody _rigidBody;
    private NavMeshAgent _playerAgent;
    private NavMeshPath _path;
    #endregion

    #region PlayerValue
    private float _speedOffSet = 0.1f; // speed OffSet
    private float _rotationVelocity; // rotationVelocity
    private Vector3 _clickRayPosition; // click RayPosition
    private bool _isMoving;
    private float _fixedYPosition; // 고정된 y 위치
    #endregion

    private Camera _mainCamera;

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        HandleMouseClick();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        FixYPosition(); // y 위치 고정
    }

    // 초기화 메서드
    private void Initialize()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _playerAgent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
        _mainCamera = Camera.main;
        _fixedYPosition = transform.position.y; // 현재 y 위치를 고정된 y 위치로 설정
    }

    // 마우스 클릭 처리 메서드
    private void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButton(1)) return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;

        _clickRayPosition = new Vector3(hit.point.x, _fixedYPosition, hit.point.z); // y 값을 고정된 위치로 설정
        _isMoving = true;
    }

    // 플레이어 이동 처리 메서드
    private void MovePlayer()
    {
        if (!_isMoving) return;

        Vector3 direction = (_clickRayPosition - transform.position).normalized;

        if (HasReachedTarget())
        {
            StopMovement();
            return;
        }

        RotateTowardsDirection(direction);
        _rigidBody.velocity = new Vector3(direction.x * walkSpeed, _rigidBody.velocity.y, direction.z * walkSpeed); // y 속도는 유지
    }

    // 목표 지점에 도착했는지 확인하는 메서드
    private bool HasReachedTarget()
    {
        return Vector3.Distance(transform.position, _clickRayPosition) < _speedOffSet;
    }

    // 이동을 멈추는 메서드
    private void StopMovement()
    {
        _rigidBody.velocity = Vector3.zero;
        _isMoving = false;
    }

    // 회전을 처리하는 메서드
    private void RotateTowardsDirection(Vector3 direction)
    {
        if (_rigidBody.velocity == Vector3.zero) return;

        float targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity, 0.12f);

        transform.rotation = Quaternion.Euler(0, smoothRotation, 0);
    }

    // y 위치를 고정하는 메서드
    private void FixYPosition()
    {
        transform.position = new Vector3(transform.position.x, _fixedYPosition, transform.position.z);
    }
}
