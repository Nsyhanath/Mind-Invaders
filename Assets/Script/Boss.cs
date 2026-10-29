using UnityEngine;

// Put this on the Boss, along with Health and a Box Collider 2D.
[RequireComponent(typeof(Health))]
public class Boss : MonoBehaviour
{
    public Bullet bulletPrefab;

    [Header("Attack")]
    public float timeBetweenShots = 1.5f; // Give the player some time to react to the shots against the boss
    public int bulletsPerShot = 1;      // 1 = single aimed shot, more = spread
    public float spreadAngle = 15f;     // angle between bullets in a spread

    [Header("Phase 2 (below half HP)")] // Second stage of the boss fight
    public float phase2TimeBetweenShots = 0.8f; // Increase fire rate
    public int phase2BulletsPerShot = 3; // Increase number of shots fired

    [Header("Movement")]
    public float bobHeight = 1f;        // how far it floats up and down
    public float bobSpeed = 1.5f; // speed of levatation for the boss

    [Header("Activation")]
    public float activationRange = 15f; // only attacks when the player is this close
    // Variables that I would need to use to initalize player in game
    Transform player;
    Collider2D playerCol;
    Collider2D col;
    Health health;
    Vector3 startPos;
    float shotTimer;
    // Start function
    void Start()
    {
        // Find player object in this case the player model
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {   // initalize player object if there is none
            player = p.transform;
            playerCol = p.GetComponent<Collider2D>();
        }
        // Get nesessary components such as health and unit collision for player
        col = GetComponent<Collider2D>();
        health = GetComponent<Health>();
        startPos = transform.position;
        shotTimer = timeBetweenShots;
    }

    void Update()
    {
        // Float up and down
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        // Stop the game if either of these objects are missing
        if (player == null || bulletPrefab == null) return;
        // Initialize vectors for the boss and player for their starting positions
        Vector2 bossCenter = col != null ? (Vector2)col.bounds.center : (Vector2)transform.position;
        Vector2 playerCenter = playerCol != null ? (Vector2)playerCol.bounds.center : (Vector2)player.position;
        // Boss attacks the player 
        if (Vector2.Distance(bossCenter, playerCenter) > activationRange) return;
        // Boss chooses between Phase 1 and phase 2 boss fight attacks when above a certain threshold of health
        bool phase2 = health.CurrentHP <= health.maxHP / 2;
        float delay = phase2 ? phase2TimeBetweenShots : timeBetweenShots;
        int count = phase2 ? phase2BulletsPerShot : bulletsPerShot;
        // Shoot projectile within a timer
        shotTimer -= Time.deltaTime;
        if (shotTimer <= 0)
        {
            shotTimer = delay;
            ShootAt(bossCenter, playerCenter, count);
        }
    }

    void ShootAt(Vector2 from, Vector2 target, int count)
    {
        // Target player position and then shoot
        Vector2 aim = (target - from).normalized;
        float baseAngle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg; // Direction of angle
        float start = baseAngle - spreadAngle * (count - 1) / 2f; // Figure out teh spread 
        // for loop initializes each bullet fired
        for (int i = 0; i < count; i++)
        {
            float a = (start + spreadAngle * i) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            Vector2 spawn = from + dir * 1f;

            Bullet b = Instantiate(bulletPrefab, spawn, Quaternion.identity);
            b.Fire(dir, gameObject);
        }
    }
}
