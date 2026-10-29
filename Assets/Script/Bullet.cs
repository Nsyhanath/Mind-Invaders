using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    // initialize and declare stats of bullet such as speed, damage and how long it lasts
    public float speed = 15f;
    public int damage = 1;
    public float lifetime = 3f;

    GameObject owner;

    void Awake()
    {
        // Initializes the physics of the movement of hte bullet
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;  // dynamic so it detects ground, walls and the boss
        rb.gravityScale = 0f;                    // fly straight
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // don't skip through thin things
        GetComponent<Collider2D>().isTrigger = true;
    }

    // Called by whoever fires the bullet
    public void Fire(Vector2 direction, GameObject shooter)
    {
        owner = shooter;
        direction.Normalize();
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;

        // Point the bullet in the direction it's travelling
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return; // ignore other bullets and trigger zones

        Health target = other.GetComponentInParent<Health>();

        // Don't hit whoever fired it
        if (target != null && target.gameObject == owner) return;
        if (owner != null && other.transform.IsChildOf(owner.transform)) return;

        if (target != null) target.TakeDamage(damage);

        Destroy(gameObject); // hit something solid (enemy, ground, wall)
    }
}
