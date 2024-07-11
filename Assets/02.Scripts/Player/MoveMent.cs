using UnityEngine;

public class MoveMent : MonoBehaviour
{
    [Header("MoveSpeed")] [SerializeField] private float walkSpeed;

    [Header("ChangeSpeedValue")] [SerializeField]
    private float changeSpeedValue;

    #region PlayerComponent

    private Rigidbody _rigidBody;

    #endregion

    #region PlayerValue

    private float _targetSpeed; // player TargetSpeed
    private float _mySpeed; // player Speed
    private float _speedOffSet = 0.1f; // speed OffSet
    private float rotationVelocity; // rotationVelocity
    private Vector3 clickRayPosition; // click RayPosition
    private bool isMoving; // isMoving

    #endregion


    private void Start()
    {
        InitPlayer();
    }

    private void Update()
    {
        MouseClick();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void InitPlayer()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void MouseClick()
    {
        bool isClick = Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);

        if (isClick)
        {
            Ray rayPosition = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayPosition, out RaycastHit hit, Mathf.Infinity))
            {
                clickRayPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                isMoving = true;
            }
        }
    }

    private void PlayerMovement()
    {
        if (isMoving)
        {
            Vector3 moveDirection = (clickRayPosition - transform.position).normalized;

            if (Vector3.Distance(transform.position, clickRayPosition) < _speedOffSet)
            {
                _rigidBody.velocity = Vector3.zero;
                isMoving = false;

                return;
            }

            _targetSpeed = walkSpeed;

            float currentVelocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z).magnitude;

            if (currentVelocity < _targetSpeed - _speedOffSet || currentVelocity > _targetSpeed + _speedOffSet)
            {
                _mySpeed = Mathf.Lerp(currentVelocity, _targetSpeed, changeSpeedValue * Time.fixedDeltaTime);

                _mySpeed = Mathf.Round(_mySpeed * 1000f) / 1000f;
            }
            else
                _mySpeed = _targetSpeed;

            if (_rigidBody.velocity != Vector3.zero)
            {
                float targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

                float smoothRotation =
                    Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, 0.12f);

                transform.rotation = Quaternion.Euler(0, smoothRotation, 0);
            }

            _rigidBody.velocity = moveDirection * _mySpeed;
        }
    }
}