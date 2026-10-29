using UnityEngine;


public class PlayerShoot : MonoBehaviour
{
    public Bullet bulletPrefab;
    public KeyCode shootKey = KeyCode.X;
    public float fireRate = 6f;          // bullets per second while holding the key
    public float heightOffset = 0f;      // move the spawn point up/down to line up with the hand

    [Header("Shooting Pose")]
    public Sprite shootSprite;           // image shown while shooting
    public SpriteRenderer playerSprite;  // drag PlayerSprite here
    public float poseTime = 0.2f;        // how long the pose stays after the last shot
    // set player shooting variables
    PlayerMovement movement;
    Collider2D col;
    float nextShotTime;
    float poseTimer;
    Sprite normalSprite;
    bool showingPose;


    [Header("Sound")]
    public AudioClip shootSound;         // drag your MP3 here
    [Range(0f, 1f)]
    public float shootVolume = 0.7f;
    public bool randomizePitch = true;   // small pitch changes so rapid shots don't sound robotic

    AudioSource audioSource;
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        col = GetComponent<Collider2D>();

        // Use the Player's Audio Source, or add one automatically if there isn't one
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        if (playerSprite == null) playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (bulletPrefab != null && Input.GetKey(shootKey) && Time.time >= nextShotTime)
        {
            nextShotTime = Time.time + 1f / fireRate;
            Shoot();
            PlayShootSound();
            ShowPose();
        }

        // Go back to the normal image shortly after you stop shooting
        if (showingPose)
        {
            poseTimer -= Time.deltaTime;
            if (poseTimer <= 0) HidePose();
        }
    }

    void Shoot()
    {
        int facing = movement != null ? movement.Facing : 1;

        // Spawn just in front of the player's collider
        Bounds b = col.bounds;
        Vector2 spawn = new Vector2(b.center.x + facing * (b.extents.x + 0.2f), b.center.y + heightOffset);

        Bullet bullet = Instantiate(bulletPrefab, spawn, Quaternion.identity);
        bullet.Fire(new Vector2(facing, 0), gameObject);
    }

    void PlayShootSound()
    {
        if (shootSound == null || audioSource == null) return;

        audioSource.pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
        // PlayOneShot lets sounds overlap when firing quickly
        audioSource.PlayOneShot(shootSound, shootVolume);
    }

    void ShowPose()
    {
        poseTimer = poseTime;
        if (showingPose || playerSprite == null || shootSprite == null) return;

        normalSprite = playerSprite.sprite;
        playerSprite.sprite = shootSprite;
        showingPose = true;
    }

    void HidePose()
    {
        showingPose = false;
        if (playerSprite != null && normalSprite != null && playerSprite.sprite == shootSprite)
            playerSprite.sprite = normalSprite;
    }
}
