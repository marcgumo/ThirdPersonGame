using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum MovementStates
    {
        Initial, Onground, OnAir, Jumping, DoubleJumping, Dash, Attack
    }

    private MovementStates playerState;

    [Header("General settings")]
    [SerializeField] private float speedMove = 4f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -15f;

    CharacterController charControl;
    Vector3 moveDirection;
    Vector3 moveDir;
    float smoothRotationVelocity;

    Vector3 verticalVelocity;

    [SerializeField] private float dashDuration = 0.6f;
    [SerializeField] private float dashForce = 8f;

    [Header("Animation settings")]
    [SerializeField] private float acceleration = 2.5f;
    [SerializeField] private float deceleration = 5f;
    float playerSpeed;

    Animator anim;


    void Start()
    {
        playerState = MovementStates.Initial;

        ChangeState(playerState);

        charControl = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;

        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        StateUpdate();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
#endif
    }

    private void ChangeState(MovementStates newState)
    {
        //Outcoming triggering condition
        switch (playerState)
        {
            case MovementStates.Initial:
                break;
            case MovementStates.Onground:
                break;
            case MovementStates.OnAir:
                break;
            case MovementStates.Jumping:
                break;
            case MovementStates.DoubleJumping:
                break;
            case MovementStates.Dash:
                break;
            case MovementStates.Attack:
                break;
        }

        //Outcoming triggering condition
        switch (newState)
        {
            case MovementStates.Initial:
                break;
            case MovementStates.Onground:
                break;
            case MovementStates.OnAir:
                break;
            case MovementStates.Jumping:
                verticalVelocity.y = jumpForce;
                break;
            case MovementStates.DoubleJumping:
                verticalVelocity.y = jumpForce;
                break;
            case MovementStates.Dash:
                Invoke("DashLater", 0.1f);
                break;
            case MovementStates.Attack:
                break;
        }

        playerState = newState;
    }

    private void StateUpdate()
    {
        switch (playerState)
        {
            case MovementStates.Initial:
                PlayerMovement();

                if (charControl.isGrounded)
                    ChangeState(MovementStates.Onground);

                break;
            case MovementStates.Onground:
                PlayerMovement();

                if (Input.GetButtonDown("Jump"))
                    ChangeState(MovementStates.Jumping);

                if (!charControl.isGrounded)
                    ChangeState(MovementStates.OnAir);

                if (Input.GetButtonDown("Fire3"))
                    ChangeState(MovementStates.Dash);

                break;
            case MovementStates.OnAir:
                PlayerMovement();

                if (charControl.isGrounded)
                    ChangeState(MovementStates.Onground);

                if (Input.GetButtonDown("Jump"))
                    ChangeState(MovementStates.DoubleJumping);

                break;
            case MovementStates.Jumping:
                PlayerMovement();

                if (charControl.isGrounded)
                    ChangeState(MovementStates.Onground);

                if (Input.GetButtonDown("Jump"))
                    ChangeState(MovementStates.DoubleJumping);

                break;
            case MovementStates.DoubleJumping:
                PlayerMovement();

                if (charControl.isGrounded)
                    ChangeState(MovementStates.Onground);

                break;
            case MovementStates.Dash:
                break;
            case MovementStates.Attack:
                break;
        }
    }

    private void PlayerMovement()
    {
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothRotationVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            charControl.Move(speedMove * Time.deltaTime * moveDir.normalized);

            if (playerSpeed < 1)
                playerSpeed += acceleration * Time.deltaTime;
            else
                playerSpeed = 1;
        }
        else
        {
            if (playerSpeed > 0)
                playerSpeed -= deceleration * Time.deltaTime;
            else
                playerSpeed = 0;
        }

        ConstantGravity();

        anim.SetFloat("speed", playerSpeed);
    }

    private void ConstantGravity()
    {
        verticalVelocity.y += gravity * Time.deltaTime;
        charControl.Move(verticalVelocity * Time.deltaTime);

        if (charControl.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -5f;
        }
    }

    private void DashLater()
    {
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;

        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        Vector3 moveDash = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        if (moveDirection.magnitude < 0.1f)
            moveDash = transform.forward;
        else
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        while (Time.time < startTime + dashDuration)
        {
            charControl.Move(dashForce * Time.deltaTime * moveDash.normalized);
            ConstantGravity();
            yield return null;
        }

        ChangeState(MovementStates.Onground);
    }
}
