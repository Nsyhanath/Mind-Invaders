using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public LayerMask GroundMask;
    public float stompBounce = 12f;
    public bool startToMoveLeft = true;

    Rigidbody2D rb;
    Collider2D col;
    int direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;
        direction = startToMoveLeft ? -1 : 1;
        UpdateFacing();
    }

    void FixedUpdate()
    {
        Bounds b = col.bounds;

        Vector2 frontMiddle = new Vector2(direction > 0 ? b.max.x : b.min.x, b.center.y);
        bool hitWall = Physics2D.Raycast(frontMiddle, Vector2.right * direction, 0.1f, GroundMask);

        Vector2 frontFoot = new Vector2(frontMiddle.x + 0.1f * direction, b.min.y + 0.05f);
        bool groundAhead = Physics2D.Raycast(frontFoot, Vector2.down, 0.3f, GroundMask);

        if (hitWall || !groundAhead)
        {
            direction *= -1;
            UpdateFacing();
        }

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.rigidbody;
        Bounds playerBounds = collision.collider.bounds;

        // Player's feet are above our middle = they jumped on us
        if (playerBounds.min.y > col.bounds.center.y)
        {
            if (playerRb != null)
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounce);

            Destroy(gameObject);
        }
        else
        {
            // Touched from the side: restart the level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void UpdateFacing()
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (direction > 0 ? 1 : -1);
        transform.localScale = s;
    }
}