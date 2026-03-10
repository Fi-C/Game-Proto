using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerCharacterController : MonoBehaviour
{
    private InputAction m_moveAction;
    private InputAction m_jumpAction;

    private Rigidbody2D m_rigidbody;

    private Vector2 m_input;

    [SerializeField] private float m_playerSpeed = 8f;
    [SerializeField] private float m_jumpForce = 12f;

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");

        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        m_input = m_moveAction.ReadValue<Vector2>();

        if (m_jumpAction.WasPressedThisFrame() && Mathf.Abs(m_rigidbody.linearVelocity.y) < 0.01f)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f)
            return;

        m_rigidbody.linearVelocity = new Vector2(
            m_input.x * m_playerSpeed,
            m_rigidbody.linearVelocity.y
        );
    }

    private void Jump()
    {
        m_rigidbody.linearVelocity = new Vector2(
            m_rigidbody.linearVelocity.x,
            m_jumpForce
        );
    }
}