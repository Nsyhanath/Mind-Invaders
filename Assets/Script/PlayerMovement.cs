using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Set speed and jump speed
    [Header("Movement")]
    public float speed = 8f;
    public float jump = 15f;

    [Header("Gravity")]
    public bool fastFall = true;        // untick to use the Rigidbody 2D's Gravity Scale instead
    public float riseGravity = 3f;
    public float fallGravity = 6f;
    // declare and initialize player's slide physics and hotkey
    [Header("Slide")]
    public KeyCode slideKey = KeyCode.LeftShift;
    public float slide = 14f;           // slide speed
    public float slideDuration = 0.4f;  // how long a slide lasts (seconds)
    public float slideCooldown = 0.5f;  // wait time before you can slide again
    [Range(0.2f, 1f)]
    public float slideHeight = 0.5f;    // collider height while sliding (0.5 = half height)
    public Sprite slideSprite;          // image shown while sliding

    [Header("Sprite")]
    public SpriteRenderer playerSprite; // drag PlayerSprite here
    public bool flipWithDirection = true;

    [Header("Ground Check")] // check if there is a ground layer
    public Transform ground;
    public LayerMask GroundMask;
    public float radius = 0.2f;

    [Header("Drop Through Platforms")]
    public float dropTime = 0.3f;       // how long you fall through a platform after pressing Down

    float input;
    int facing = 1;
    public int Facing => facing;        // used by PlayerShoot to know which way to fire
    bool isOnTheGround;
    // set sliding variables
    bool isSliding;
    float slideTimer;
    float cooldownTimer;
    // set asset variables
    Rigidbody2D rb;
    BoxCollider2D box;
    Vector2 normalSize;
    Vector2 normalOffset;
    Sprite normalSprite;
    // collider variables
    Collider2D myCollider;
    Collider2D standingOn;              // what the ground check is touching
    Collider2D droppingThrough;         // platform we're currently falling through

    void Start()
    {
        // get components
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        myCollider = GetComponent<Collider2D>();
        if (box != null)
        {
            normalSize = box.size;
            normalOffset = box.offset;
        }
        // set player sprite
        if (playerSprite == null) playerSprite = GetComponentInChildren<SpriteRenderer>();
        if (playerSprite != null) normalSprite = playerSprite.sprite;
    }

    void Update()
    {
        // get input update
        input = Input.GetAxisRaw("Horizontal");
        if (input != 0) facing = input > 0 ? 1 : -1;
        if (flipWithDirection && playerSprite != null) playerSprite.flipX = facing < 0;

        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        // Start a slide: on the ground, moving, key pressed, cooldown done
        if (!isSliding && isOnTheGround && input != 0 && cooldownTimer <= 0 && Input.GetKeyDown(slideKey))
        {
            StartSlide();
        }

        // End the slide when time runs out
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0) EndSlide();
        }

        // Drop down through a one-way platform (Down Arrow or S)
        if (isOnTheGround && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
        {
            TryDropThrough();
        }

        // Jump (also cancels a slide)
        if (isOnTheGround && Input.GetKeyDown(KeyCode.Space))
        {
            if (isSliding) EndSlide();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
        }
    }

    void FixedUpdate()
    {
        standingOn = Physics2D.OverlapCircle(ground.position, radius, GroundMask);
        // While falling through a platform, don't count it as ground
        isOnTheGround = standingOn != null && standingOn != droppingThrough;

        if (isSliding)
            rb.linearVelocity = new Vector2(facing * slide, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(input * speed, rb.linearVelocity.y);

        if (fastFall)
            rb.gravityScale = rb.linearVelocity.y < 0 ? fallGravity : riseGravity;
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        if (playerSprite != null && slideSprite != null) playerSprite.sprite = slideSprite;

        // Shrink the collider so the player can slide under low things
        if (box != null)
        {
            float newHeight = normalSize.y * slideHeight;
            box.size = new Vector2(normalSize.x, newHeight);
            box.offset = new Vector2(normalOffset.x, normalOffset.y - (normalSize.y - newHeight) / 2f);
        }
    }

    void EndSlide()
    {
        isSliding = false;
        cooldownTimer = slideCooldown;

        if (playerSprite != null && normalSprite != null) playerSprite.sprite = normalSprite;

        if (box != null)
        {
            box.size = normalSize;
            box.offset = normalOffset;
        }
    }

    void TryDropThrough()
    {
        // Only platforms with a Platform Effector 2D can be dropped through,
        // so you can't fall through the main floor.
        if (standingOn == null || myCollider == null) return;
        if (standingOn.GetComponent<PlatformEffector2D>() == null) return;

        if (isSliding) EndSlide();
        StartCoroutine(DropThrough(standingOn));
    }

    IEnumerator DropThrough(Collider2D platform)
    {
        droppingThrough = platform;
        Physics2D.IgnoreCollision(myCollider, platform, true);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f); // small push down

        yield return new WaitForSeconds(dropTime);

        if (platform != null) Physics2D.IgnoreCollision(myCollider, platform, false);
        droppingThrough = null;
    }

}
