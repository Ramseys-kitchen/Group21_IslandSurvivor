using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public CanvasGroup descriptionScreen;
    public CanvasGroup thankYouScreen;
    public UnityEngine.UI.Button exitButton;

    private bool showingThankYou = false;

    void Start()
    {
    
        descriptionScreen.alpha = 1f;
        thankYouScreen.alpha = 0f;
        exitButton.gameObject.SetActive(false);

        
        exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        // Wait for click to show thank you screen
        if (!showingThankYou && Input.GetMouseButtonDown(0))
        {
            ShowThankYou();
        }
    }

    void ShowThankYou()
    {
        showingThankYou = true;
        descriptionScreen.alpha = 0f;
        thankYouScreen.alpha = 1f;
        exitButton.gameObject.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}