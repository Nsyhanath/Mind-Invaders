using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Put this on an empty object called GameManager in your level scene.
// Player's Health -> On Death -> GameManager.ShowGameOver
// Boss's Health   -> On Death -> GameManager.ShowVictory
public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public string titleSceneName = "TitleScreen";
    public float delayBeforeScreen = 1f;   // short pause so the death can be seen

    bool gameEnded;

    void Start()
    {
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        // Keyboard shortcut: Enter restarts / plays again once a screen is showing
        if (gameEnded && Time.timeScale == 0f && Input.GetKeyDown(KeyCode.Return))
            Restart();
    }

    public void ShowGameOver()
    {
        if (gameEnded) return;
        gameEnded = true;
        StartCoroutine(ShowPanel(gameOverPanel));
    }

    public void ShowVictory()
    {
        if (gameEnded) return;
        gameEnded = true;
        StartCoroutine(ShowPanel(victoryPanel));
    }

    IEnumerator ShowPanel(GameObject panel)
    {
        yield return new WaitForSeconds(delayBeforeScreen);
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;   // freeze the game behind the screen
    }

    // Hook these to buttons
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
}
