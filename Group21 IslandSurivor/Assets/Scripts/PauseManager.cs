using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenu;
    public GameObject gameTab;
    public GameObject storyTab;
    public Button gameTabButton;
    public Button storyTabButton;

    [Header("Input Settings")]
    public KeyCode pauseKey = KeyCode.Tab;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        
        Time.timeScale = isPaused ? 0f : 1f;

        
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        
        if (isPaused)
        {
            SwitchToGameTab();
        }
    }

    public void SwitchToGameTab()
    {
        gameTab.SetActive(true);
        storyTab.SetActive(false);

        
        gameTabButton.interactable = false;
        storyTabButton.interactable = true;
    }

    public void SwitchToStoryTab()
    {
        gameTab.SetActive(false);
        storyTab.SetActive(true);

        
        gameTabButton.interactable = true;
        storyTabButton.interactable = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("0_StartScreen"); // My start screen scene name
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}