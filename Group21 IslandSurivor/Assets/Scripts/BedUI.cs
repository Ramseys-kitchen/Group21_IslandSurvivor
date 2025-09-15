using UnityEngine;

public class BedUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject sleepPrompt; // I will put "Press F to sleep" text
    public GameObject dangerPrompt; // I will put my "enemies nearby" text"

    void Start()
    {
       
        if (sleepPrompt != null)
            sleepPrompt.SetActive(false);
        if (dangerPrompt != null)
            dangerPrompt.SetActive(false);
    }

    public void ShowSleepPrompt()
    {
        if (sleepPrompt != null)
            sleepPrompt.SetActive(true);
        if (dangerPrompt != null)
            dangerPrompt.SetActive(false);
    }

    public void ShowDangerPrompt()
    {
        if (sleepPrompt != null)
            sleepPrompt.SetActive(false);
        if (dangerPrompt != null)
            dangerPrompt.SetActive(true);
    }

    public void HideAllPrompts()
    {
        if (sleepPrompt != null)
            sleepPrompt.SetActive(false);
        if (dangerPrompt != null)
            dangerPrompt.SetActive(false);
    }
}