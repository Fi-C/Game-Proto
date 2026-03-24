using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerCharacterController : MonoBehaviour
{
    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_groundpoundAction;

    private Rigidbody2D rb;
    private Vector2 input;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 14f;
    [SerializeField] private float accel = 80f;
    [SerializeField] private float decel = 100f;
    [SerializeField] private float airAccel = 60f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float gravity = 60f;
    [SerializeField] private float fallMultiplier = 1.8f;
    [SerializeField] private float lowJumpMultiplier = 2.5f;

    [Header("Ground Pound")]
    [SerializeField] private float poundForce = 30f;
    private bool isGroundPounding;

    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Jump")]
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallRadius = 0.2f;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float wallJumpForce = 14f;
    [SerializeField] private float wallJumpControlLock = 0.1f;

    private bool isGrounded;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpTimer;
    private int wallDir;

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_groundpoundAction = InputSystem.actions.FindAction("Ground Pound");

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        input = m_moveAction.ReadValue<Vector2>();

        GroundCheck();
        WallCheck();
        HandleWallSlide();
        HandleJump();
        HandleGroundPound();
        ApplyBetterGravity();

        if (isGrounded)
        {
            isGroundPounding = false;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float targetSpeed = input.x * maxSpeed;

        float accelRate = isGrounded
            ? (Mathf.Abs(targetSpeed) > 0.01f ? accel : decel)
            : airAccel;

        if (isWallJumping)
        {
            wallJumpTimer -= Time.fixedDeltaTime;
            if (wallJumpTimer <= 0f)
                isWallJumping = false;
        }

        float controlMultiplier = isWallJumping ? 0.5f : 1f;

        float newVelX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            accelRate * controlMultiplier * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (m_jumpAction.WasPressedThisFrame())
        {
            if (isWallSliding)
            {
                isWallJumping = true;
                wallJumpTimer = wallJumpControlLock;

                float jumpDir = -wallDir;

                rb.linearVelocity = new Vector2(
                    jumpDir * wallJumpForce,
                    jumpForce
                );
            }
            else if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        if (m_jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    void ApplyBetterGravity()
    {
        float yVel = rb.linearVelocity.y;

        if (yVel < 0)
            yVel -= gravity * fallMultiplier * Time.deltaTime;
        else if (yVel > 0 && !m_jumpAction.IsPressed())
            yVel -= gravity * lowJumpMultiplier * Time.deltaTime;
        else
            yVel -= gravity * Time.deltaTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, yVel);
    }

    void HandleGroundPound()
    {
        if (m_groundpoundAction.WasPressedThisFrame() && !isGrounded && !isGroundPounding)
        {
            isGroundPounding = true;
            rb.linearVelocity = new Vector2(0f, -poundForce);
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    void WallCheck()
    {
        bool left = Physics2D.OverlapCircle(wallCheckLeft.position, wallRadius, wallLayer);
        bool right = Physics2D.OverlapCircle(wallCheckRight.position, wallRadius, wallLayer);

        if (!isGrounded && (left || right))
        {
            isWallSliding = true;
            wallDir = right ? 1 : -1;
        }
        else
        {
            isWallSliding = false;
        }
    }

    void HandleWallSlide()
    {
        if (isWallSliding && rb.linearVelocity.y < -wallSlideSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
    }
}