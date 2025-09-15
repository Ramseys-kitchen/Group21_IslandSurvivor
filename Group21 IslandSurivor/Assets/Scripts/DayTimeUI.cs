using UnityEngine;
using UnityEngine.UI;

public class DayTimeUI : MonoBehaviour
{
    [Header("UI Text Component")]
    public Text uiText; 

    private DayNightCycle dayNightCycle;

    void Start()
    {
        
        dayNightCycle = FindObjectOfType<DayNightCycle>();
    }

    void Update()
    {
       
        if (dayNightCycle != null && uiText != null)
        {
            UpdateText();
        }
    }

    void UpdateText()
    {
        
        int day = dayNightCycle.CurrentDay;

        
        float progress = dayNightCycle.DayProgress;
        string timeOfDay = "";

        if (progress < 0.25f)
            timeOfDay = "Morning";
        else if (progress < 0.5f)
            timeOfDay = "Afternoon";
        else if (progress < 0.75f)
            timeOfDay = "Evening";
        else
            timeOfDay = "Night";

        
        uiText.text = timeOfDay + " Day " + day;
    }
}