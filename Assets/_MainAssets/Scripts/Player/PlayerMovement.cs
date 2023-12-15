using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player movement. Includes WASD movment, jumping, crouching, air control, slope handling, climbing, and grapple movement.
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool canJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Climbing")]
    public LayerMask climbMask;
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;
    public bool climbing;
    public bool canClimb = true;

    [Header("Climbing - Wall Detection")]
    public float wallDetectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;
    private RaycastHit frontWallHit;
    private bool wallInFront;
    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;
    // Exiting
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Grappling")]
    public bool freeze;
    public bool activeGrapple;


    [Header("Misc.")]
    public Transform orientation;
    Rigidbody rb;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDir;

    //Footstep Control
    //private float nextStep = 10.0f;
    //private float currentStep = 0.0f;




    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        climbing,
        air,
        freeze
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        canJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.05f, groundMask);
        if (grounded)
        {
            canClimb = true;
        }

        MyInput();
        SpeedControl();
        StateHandler();

        // climbing
        ClimbWallCheck();
        CheckClimbState();

        if (climbing && !exitingWall)
        {
            ClimbingMovement();
        }

        // handle drag
        if (grounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
            rb.angularDrag = 0;
        }

        // //Check if the next footstep was be taken
        // if (currentStep > nextStep)
        // {
        //     //Play sound
        //     SoundManager.PlaySound(gameObject, SoundEffect.Footstep_Stone, 0.5f);
        //     //reset footstep
        //     currentStep = 0.0f;
        // }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        // Mode - Climbing
        else if (climbing)
        {
            state = MovementState.climbing;
        }
        // Mode - Crouching
        else if (Input.GetKey(PlayerKeybinds.CROUCH_KEY))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        // Mode - Sprinting
        else if (grounded && Input.GetKey(PlayerKeybinds.SPRINT_KEY))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;

            // //increase footstep
            // currentStep += 0.25f;
        }
        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;

            // //increase footstep
            // currentStep += 0.1f;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // jumping
        if (Input.GetKey(PlayerKeybinds.JUMP_KEY) && canJump && grounded)
        {
            canJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // crouching
        if (Input.GetKeyDown(PlayerKeybinds.CROUCH_KEY))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            // rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(PlayerKeybinds.CROUCH_KEY))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;
        if (exitingWall) return;

        // calculate movement direction
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // on ground
        if (grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        // in air
        else if (!grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // disable gravity while on slope (prevents sliding)
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;

        // limit speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        // limit speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    private void Jump()
    {
        exitingSlope = true;
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
        canClimb = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    #region climbing
    private void ClimbWallCheck()
    {
        wallInFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, wallDetectionLength, climbMask);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if ((wallInFront && newWall) || grounded)
        {
            climbTimer = maxClimbTime;
        }
    }

    private void StartClimbing()
    {
        climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }
    private void StopClimbing()
    {
        climbing = false;
        canClimb = false;
    }

    private void CheckClimbState()
    {
        // Climbing
        if (wallInFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall && canClimb)
        {
            if (!climbing && climbTimer > 0)
            {
                StartClimbing();
            }

            // CLIMBING TIMER
            if (climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }
            if (climbTimer < 0)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
                StopClimbing();
            }

        }
        // Exiting Climb
        else if (exitingWall)
        {
            if (climbing)
            {
                StopClimbing();
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer < 0) exitingWall = false;
        }
        // Not Climbing
        else
        {
            if (climbing) StopClimbing();
        }

    }
    #endregion

    #region grapplinghook
    public void GrappleJumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateGrappleJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    private Vector3 velocityToSet;
    private bool enableMovementOnNextTouch;

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            Tool activeTool = GameObject.FindGameObjectWithTag("PlayerCameraHolder").GetComponentInChildren<Camera>().GetComponent<ToolbeltController>().GetActiveTool();
            if (activeTool.GetType() == typeof(GrapplingHook))
            {
                GrapplingHook gh = (GrapplingHook)activeTool;
                gh.StopGrapple();
            }
        }
    }

    public Vector3 CalculateGrappleJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
    #endregion
}
