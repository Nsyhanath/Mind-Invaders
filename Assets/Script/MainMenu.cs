using UnityEngine;
using UnityEngine.SceneManagement;


// Hook the Play button to StartGame() and the Quit button to QuitGame().
public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "SampleScene";  // the scene with your level
    public bool pressAnyKeyToStart = true;        // Enter or Space also starts the game

    void Update()
    {
        if (pressAnyKeyToStart && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
