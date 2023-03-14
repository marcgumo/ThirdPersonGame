using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum MovementStates
    {
        Initial, Onground, OnAir, Jumping, DoubleJumping, Dash, Attack
    }

    public MovementStates playerState { get; set; }

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

    int numberOfClicks = 0;

    [SerializeField] private SphereCollider attackCollider;

    Vector3 initialPosition;
    Quaternion initialRotation;

    Coroutine dashCoroutine;

    UIController UIController;

    void Start()
    {
        playerState = MovementStates.Initial;

        ChangeState(playerState);

        charControl = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;

        anim = GetComponentInChildren<Animator>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        UIController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
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
                anim.SetBool("doubleJumping", false);
                break;
            case MovementStates.Dash:
                anim.SetBool("dashing", false);
                anim.SetInteger("attack", 0);
                numberOfClicks = 0;
                break;
            case MovementStates.Attack:
                break;
        }

        //Incoming triggering condition
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

                anim.SetBool("doubleJumping", true);
                break;
            case MovementStates.Dash:
                Invoke("DashLater", 0.1f);

                anim.SetBool("dashing", true);

                attackCollider.enabled = false;
                break;
            case MovementStates.Attack:
                OnClickAttack();
                playerSpeed = 0;
                anim.SetFloat("speed", playerSpeed);
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

                if (!charControl.isGrounded && !UIController.gameIsPaused)
                    ChangeState(MovementStates.OnAir);

                if (Input.GetButtonDown("Fire3"))
                    ChangeState(MovementStates.Dash);

                if (Input.GetButtonDown("Fire1") && !UIController.gameIsPaused)
                    ChangeState(MovementStates.Attack);

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
                if (Input.GetButtonDown("Fire1") && !UIController.gameIsPaused)
                    OnClickAttack();
                if (Input.GetButtonDown("Fire3"))
                    ChangeState(MovementStates.Dash);
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

        if (!UIController.gameIsPaused)
            anim.SetBool("isGrounded", charControl.isGrounded);
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
        dashCoroutine = StartCoroutine(Dash());
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

    private void OnClickAttack()
    {
        numberOfClicks++;

        if (numberOfClicks == 1)
            anim.SetInteger("attack", 1);

        numberOfClicks = Mathf.Clamp(numberOfClicks, 0, 3);
    }

    public void CheckCombo()
    {
        if (playerState == MovementStates.Dash)
            return;

        if (numberOfClicks == 1 && anim.GetCurrentAnimatorStateInfo(0).IsName("attack_1"))
        {
            anim.SetInteger("attack", 0);
            numberOfClicks = 0;
            ChangeState(MovementStates.Onground);
        }

        if (numberOfClicks >= 2 && anim.GetCurrentAnimatorStateInfo(0).IsName("attack_1"))
        {
            anim.SetInteger("attack", 2);
        }

        if (numberOfClicks == 2 && anim.GetCurrentAnimatorStateInfo(0).IsName("attack_2"))
        {
            anim.SetInteger("attack", 0);
            numberOfClicks = 0;
            ChangeState(MovementStates.Onground);
        }

        if (numberOfClicks >= 3 && anim.GetCurrentAnimatorStateInfo(0).IsName("attack_2"))
        {
            anim.SetInteger("attack", 3);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack_3"))
        {
            anim.SetInteger("attack", 0);
            numberOfClicks = 0;
            ChangeState(MovementStates.Onground);
        }
    }

    public void SetAttackCollider(bool value)
    {
        if (playerState == MovementStates.Dash)
            return;

        attackCollider.enabled = value;
    }

    private void MoveCharacter(Vector3 position, Quaternion rotation)
    {
        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        charControl.Move(Vector3.zero);

        charControl.enabled = false;
        transform.position = position;
        transform.rotation = rotation;
        charControl.enabled = true;
        ChangeState(MovementStates.Initial);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<iTakeItem>() != null)
            other.GetComponent<iTakeItem>().TakeItem();

        if (other.gameObject.tag == "JumpingPlatform" && playerState != MovementStates.Dash)
        {
            if (playerState == MovementStates.DoubleJumping)
                anim.SetTrigger("jumpingPlatform");

            if (other.GetComponentInParent<EnemyController>())
            {
                verticalVelocity.y = jumpForce * 1.5f;
                other.GetComponentInParent<HealthController>().TakeDamage(30, other.transform.parent.tag);
            }
            else verticalVelocity.y = jumpForce * 2;

            ChangeState(MovementStates.OnAir);
        }

        if (other.tag == "DeadZone")
            MoveCharacter(initialPosition, initialRotation);

        if (other.tag == "CheckPoint")
        {
            initialPosition = other.transform.parent.GetChild(1).position;
            initialRotation = other.transform.parent.GetChild(1).rotation;
        }

        if (other.tag == "QuizPuzzle")
        {
            UIController.TextToDisplay(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "QuizPuzzle")
        {
            UIController.TextToDisplay(false);
        }
    }

    private void OnEnable()
    {
        AnimationEventController.onAnimationEvent += CheckCombo;
    }

    private void OnDisable()
    {
        AnimationEventController.onAnimationEvent -= CheckCombo;
    }
}
