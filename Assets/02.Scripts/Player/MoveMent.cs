using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMent : MonoBehaviour
{
    [Header("MoveSpeed")]
    [SerializeField] private float walkSpeed;

    [Header("ChangeSpeedValue")]
    [SerializeField] private float changeSpeedValue;

    #region PlayerComponent
    private Rigidbody rigidBody;

    #endregion

    #region PlayerValue
    //player TargetSpeed
    private float targetSpeed;
    //player Speed
    private float mySpeed;
    //speed OffSet
    private float speedOffSet = 0.1f;
    //rotationVelocity
    private float rotationVelocity;
    //click RayPosition
    private Vector3 clickRayPosition;
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
        rigidBody = GetComponent<Rigidbody>();  
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
            }
        }
    }

    private void PlayerMovement()
    {
        targetSpeed = walkSpeed;

        if(rigidBody.velocity == Vector3.zero)
        {
            targetSpeed = 0;
        }

        float currentVelocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z).magnitude;

        if (currentVelocity < targetSpeed - speedOffSet || currentVelocity > targetSpeed + speedOffSet)
        {
            mySpeed = Mathf.Lerp(currentVelocity, targetSpeed, changeSpeedValue * Time.fixedDeltaTime);

            mySpeed = Mathf.Round(mySpeed * 1000f) / 1000f;
        }
        else
            mySpeed = targetSpeed;

        Vector3 rotateDirection = (clickRayPosition - transform.position).normalized;

        if(rigidBody.velocity != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(rotateDirection.x, rotateDirection.z) * Mathf.Rad2Deg;

            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, 0.12f);

            transform.rotation = Quaternion.Euler(0, smoothRotation, 0);
        }

        Vector3 moveDirection = (clickRayPosition - transform.position).normalized;

        rigidBody.velocity = moveDirection * mySpeed;

    }


}
