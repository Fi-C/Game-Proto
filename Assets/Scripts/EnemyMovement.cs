using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    private int direction = 1;

    private Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        CheckEdge();
    }

    void CheckEdge()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        if (!hit)
        {
            FlipDirection();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            FlipDirection();
        }
    }

    void FlipDirection()
    {
        direction *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}