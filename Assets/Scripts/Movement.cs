using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlatformerCharacterController : MonoBehaviour
{
    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_dashAction;
    private InputAction m_groundpoundAction;

    private Rigidbody2D m_rigidbody;
    private Vector2 m_input;

    [SerializeField] private float m_playerSpeed = 8f;
    [SerializeField] private float m_jumpForce = 12f;
    [SerializeField] private float m_poundForce = 12f;

    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing;
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallRadius = 0.2f;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 12f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 1.5f);
    [SerializeField] private float wallJumpTime = 0.2f;

    private bool isGrounded;
    private bool isGroundPounding;

    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpTimer;
    private int wallDirection;

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_dashAction = InputSystem.actions.FindAction("Dash");
        m_groundpoundAction = InputSystem.actions.FindAction("Ground Pound");

        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        m_input = m_moveAction.ReadValue<Vector2>();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        isTouchingWallLeft = Physics2D.OverlapCircle(wallCheckLeft.position, wallRadius, wallLayer);
        isTouchingWallRight = Physics2D.OverlapCircle(wallCheckRight.position, wallRadius, wallLayer);

        if (!isGrounded && ((isTouchingWallRight && m_input.x > 0) || (isTouchingWallLeft && m_input.x < 0)))
        {
            isWallSliding = true;
            wallDirection = isTouchingWallRight ? 1 : -1;
        }
        else
        {
            isWallSliding = false;
        }

        if (m_jumpAction.WasPressedThisFrame())
        {
            if (isWallSliding)
            {
                isWallJumping = true;
                wallJumpTimer = wallJumpTime;

                m_rigidbody.linearVelocity = new Vector2(
                    -wallDirection * wallJumpDirection.x * wallJumpForce,
                    wallJumpDirection.y * wallJumpForce
                );
            }
            else if (isGrounded)
            {
                Jump();
            }
        }

        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0f)
            {
                isWallJumping = false;
            }
        }

        if (m_dashAction.WasPressedThisFrame() && canDash)
        {
            StartCoroutine(Dash());
        }

        if (m_groundpoundAction.WasPressedThisFrame() && !isGrounded && !isGroundPounding)
        {
            GroundPound();
        }

        if (isGrounded)
        {
            isGroundPounding = false;
            isWallJumping = false;
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f)
            return;

        if (isDashing)
            return;

        float moveX = m_input.x;

        if (isWallSliding)
        {
            moveX = 0f;

            if (m_rigidbody.linearVelocity.y < -wallSlideSpeed)
            {
                m_rigidbody.linearVelocity = new Vector2(0f, -wallSlideSpeed);
                return;
            }
        }

        if (!isWallJumping)
        {
            m_rigidbody.linearVelocity = new Vector2(
                moveX * m_playerSpeed,
                m_rigidbody.linearVelocity.y
            );
        }
    }

    private void Jump()
    {
        m_rigidbody.linearVelocity = new Vector2(
            m_rigidbody.linearVelocity.x,
            m_jumpForce
        );
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = m_rigidbody.gravityScale;
        m_rigidbody.gravityScale = 0f;

        float dashDirection = m_input.x == 0 ? transform.localScale.x : Mathf.Sign(m_input.x);

        m_rigidbody.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        m_rigidbody.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }

    private void GroundPound()
    {
        isGroundPounding = true;

        m_rigidbody.linearVelocity = new Vector2(
            m_rigidbody.linearVelocity.x,
            -m_poundForce
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheckLeft.position, wallRadius);
        }

        if (wallCheckRight != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(wallCheckRight.position, wallRadius);
        }
    }
}