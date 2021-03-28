using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class MovementHandler : MonoBehaviour
{
    private CharacterController characterController;
    private Rigidbody rb;
    private Player player;

    public float CurrentSpeed { get; private set; }
    private Vector3 direction;
    private bool isDashing = false;
    private Vector3 dashDirection;
    Vector3 knockback = Vector3.zero;

    //ROTATION
    private float turnSmoothTime = 0.01f;
    private float turnSmoothVelocity;

    //DEBUG
    public bool isGrounded;

    // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        dashDirection = transform.rotation * Vector3.forward;
        CurrentSpeed = player.moveSpeed;
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if ((direction.x != 0) || (direction.z != 0))
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            dashDirection = direction;
            CurrentSpeed = player.moveSpeed;
        }
        else
        {
            CurrentSpeed = 0;
        }

        if (!characterController.isGrounded)
        {
            direction += Physics.gravity;
        }

        Move(direction);
    }

    private void Move(Vector3 direction)
    {
        if (knockback.magnitude > 0.2F)
        {
            ApplyKnockback();
        }
        else
        {
            Vector3 move = direction * CurrentSpeed * Time.deltaTime;
            DoMovement(move);
        }
    }

    private void DoMovement(Vector3 movement)
    {
        characterController.Move(movement);
    }

    private void ApplyKnockback()
    {
        DoMovement(knockback * Time.deltaTime);
        knockback = Vector3.Lerp(knockback, Vector3.zero, 5 * Time.deltaTime);
    }

    public void SetMoveDirection(Vector3 direction)
    {
        direction = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * direction;
        this.direction = direction;
    }

    public void StartDash()
    {
        isDashing = true;
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        float dashTime = Time.time + player.dashTime;

        CurrentSpeed = player.dashSpeed;
        while (Time.time < dashTime)
        {
            Move(dashDirection);
            yield return null;
        }

        CurrentSpeed = player.moveSpeed;
        isDashing = false;
    }

    public void AddKnockback(Vector3 dir, float force)
    {
        knockback += dir * force;
    }
}
