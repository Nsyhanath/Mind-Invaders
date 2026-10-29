using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class Health : MonoBehaviour
{
    public int maxHP = 10;
    public Slider hpBar;                     // UI slider for health
    public float invincibleTime = 0f;        // seconds of invincibility after a hit (try 1 for the player)
    public bool restartSceneOnDeath = false; // instantly restart the level on death (leave OFF if using the Game Over screen)

    [Header("Death")]
    public UnityEvent onDeath;               // things to run when this dies (e.g. GameManager.ShowGameOver)
    // check current hp values and set invincible frame
    public int CurrentHP { get; private set; }
    bool invincible;
    bool dead;
    SpriteRenderer[] sprites;

    void Start()
    {
        // Initalize health component
        CurrentHP = maxHP;
        sprites = GetComponentsInChildren<SpriteRenderer>();
        if (hpBar != null)
        {   // declare hp values
            hpBar.minValue = 0;
            hpBar.maxValue = maxHP;
            hpBar.value = maxHP;
        }
    }

    public void TakeDamage(int amount)
    {
        // check if player is dead or invincible
        if (invincible || dead) return;
        // take damage to health value
        CurrentHP = Mathf.Max(CurrentHP - amount, 0);
        if (hpBar != null) hpBar.value = CurrentHP;
        // check if hp is less than or equal to 0
        if (CurrentHP <= 0)
        {   // call unalive function
            Die();
            return;
        }

        StartCoroutine(HitFlash());
    }

    IEnumerator HitFlash()
    {
        // invulnerable is set to true
        if (invincibleTime > 0) invincible = true;
        float t = 0f;
        float duration = Mathf.Max(invincibleTime, 0.1f);
        while (t < duration)
        {
            // set invulnerable time to player
            foreach (var s in sprites) s.enabled = !s.enabled;
            yield return new WaitForSeconds(0.05f);
            t += 0.05f;
        }
        foreach (var s in sprites) s.enabled = true;
        invincible = false; // end invulnerable frame
    }

    void Die()
    {
        // is dead is set to true
        dead = true;
        onDeath.Invoke();
        // call panel to game over
        if (restartSceneOnDeath)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
            Destroy(gameObject);
    }
}
