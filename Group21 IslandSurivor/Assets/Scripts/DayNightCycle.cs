using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DayEvent : UnityEvent<int> { }

public class DayNightCycle : MonoBehaviour
{
    [Header("Skybox Materials")]
    public Material daySkybox;  
    public Material afternoonSkybox;
    public Material nightSkybox; 

    [Header("Lighting")]
    public Light sunLight; 
    public Gradient dayColor; 
    public Gradient afternoonColor;
    public Gradient nightColor; 

    [Header("Timing")]
    public float cycleDuration = 120f; // 2 minutes in seconds
    public int maxDays = 4;

    [Header("Sun Rotation")]
    public Transform sunTransform; 

    [Header("Day Events")]
    public DayEvent OnDayStart; 
    public DayEvent OnSpecificDay; 

    [Header("Debug")]
    public bool showDebugInfo = true;

    private float currentTime = 0f;
    private bool isDay = true;
    private int currentDay = 1;
    private bool[] dayEventsTriggered;
    private bool gameEnded = false;

  
    public int CurrentDay => currentDay;
    public bool IsGameEnded => gameEnded;
    public float TimeUntilNextDay => cycleDuration - (currentTime % cycleDuration);
    public float DayProgress => (currentTime % cycleDuration) / cycleDuration;

    void Start()
    {
        
        dayEventsTriggered = new bool[maxDays + 1]; 

        
        if (daySkybox != null)
            RenderSettings.skybox = daySkybox;

        
        SetupDefaultGradients();

        
        OnDayStart?.Invoke(currentDay);
        TriggerDaySpecificEvents(currentDay);

        if (showDebugInfo)
            Debug.Log($"Game Started - Day {currentDay}");
    }

    void Update()
    {
        if (gameEnded) return;

        
        currentTime += Time.deltaTime;

        
        int newDay = Mathf.FloorToInt(currentTime / cycleDuration) + 1;

        if (newDay != currentDay && newDay <= maxDays)
        {
            currentDay = newDay;
            OnDayStart?.Invoke(currentDay);
            TriggerDaySpecificEvents(currentDay);

            if (showDebugInfo)
                Debug.Log($"Day {currentDay} Started!");
        }
        else if (newDay > maxDays && !gameEnded)
        {
            EndGame();
            return;
        }

        
        float cycleProgress = (currentTime % cycleDuration) / cycleDuration;

        
        bool shouldBeDay = cycleProgress < 0.5f;

        
        if (shouldBeDay != isDay)
        {
            isDay = shouldBeDay;
            SwitchSkybox();

            if (showDebugInfo)
                Debug.Log($"Day {currentDay}: {(isDay ? "Day" : "Night")} time");
        }

        
        UpdateLighting(cycleProgress);

        
        RotateSun(cycleProgress);
    }

    void TriggerDaySpecificEvents(int day)
    {
        if (day > maxDays || dayEventsTriggered[day]) return;

        dayEventsTriggered[day] = true;

        // I will trigger the events down here
        switch (day)
        {
            case 1:
                // Day 1 events (game start)
                if (showDebugInfo)
                    Debug.Log("Day 1: Game begins!");
                break;

            case 2:
                // Day 2 events (e.g., rain)
                if (showDebugInfo)
                    Debug.Log("Day 2: Rain event triggered!");
                break;

            case 3:
                // Day 3 events
                if (showDebugInfo)
                    Debug.Log("Day 3: Special event triggered!");
                break;

            case 4:
                // Day 4 events 
                if (showDebugInfo)
                    Debug.Log("Day 4: Final day events!");
                break;
        }

        
        OnSpecificDay?.Invoke(day);
    }


    void EndGame()
    {
        gameEnded = true;
        if (showDebugInfo)
            Debug.Log("Game completed! All 4 days have passed.");

       
        
    }

    void SwitchSkybox()
    {
        if (isDay && daySkybox != null)
        {
            RenderSettings.skybox = daySkybox;
        }
        else if (!isDay && nightSkybox != null)
        {
            RenderSettings.skybox = nightSkybox;
        }

        
        DynamicGI.UpdateEnvironment();
    }

    void UpdateLighting(float cycleProgress)
    {
        if (sunLight == null) return;

        
        float lightIntensity;
        Color lightColor;

        if (cycleProgress < 0.5f) 
        {
            float dayProgress = cycleProgress * 2f; 
            lightIntensity = Mathf.Lerp(0.3f, 1.2f, Mathf.Sin(dayProgress * Mathf.PI));
            lightColor = dayColor.Evaluate(dayProgress);
        }
        else 
        {
            float nightProgress = (cycleProgress - 0.5f) * 2f; 
            lightIntensity = Mathf.Lerp(0.1f, 0.3f, Mathf.Sin(nightProgress * Mathf.PI));
            lightColor = nightColor.Evaluate(nightProgress);
        }

        sunLight.intensity = lightIntensity;
        sunLight.color = lightColor;
    }

    void RotateSun(float cycleProgress)
    {
        if (sunTransform == null) return;

        
        float sunAngle = cycleProgress * 180f - 90f; 
        sunTransform.rotation = Quaternion.Euler(sunAngle, 30f, 0f);
    }

    void SetupDefaultGradients()
    {
        if (dayColor.colorKeys.Length == 0)
        {
            
            GradientColorKey[] dayColors = new GradientColorKey[3];
            dayColors[0] = new GradientColorKey(new Color(1f, 0.6f, 0.2f), 0f); // Sunrise
            dayColors[1] = new GradientColorKey(Color.white, 0.5f); // Noon
            dayColors[2] = new GradientColorKey(new Color(1f, 0.4f, 0.1f), 1f); // Sunset

            GradientAlphaKey[] dayAlphas = new GradientAlphaKey[2];
            dayAlphas[0] = new GradientAlphaKey(1f, 0f);
            dayAlphas[1] = new GradientAlphaKey(1f, 1f);

            dayColor.SetKeys(dayColors, dayAlphas);
        }

        if (nightColor.colorKeys.Length == 0)
        {
            
            GradientColorKey[] nightColors = new GradientColorKey[2];
            nightColors[0] = new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 0f);
            nightColors[1] = new GradientColorKey(new Color(0.05f, 0.05f, 0.2f), 1f);

            GradientAlphaKey[] nightAlphas = new GradientAlphaKey[2];
            nightAlphas[0] = new GradientAlphaKey(1f, 0f);
            nightAlphas[1] = new GradientAlphaKey(1f, 1f);

            nightColor.SetKeys(nightColors, nightAlphas);
        }
    }

    

    
    public void SetDay(int day)
    {
        if (day < 1 || day > maxDays) return;

        currentDay = day;
        currentTime = (day - 1) * cycleDuration;

        if (!dayEventsTriggered[day])
        {
            TriggerDaySpecificEvents(day);
        }
    }

    
    public void SkipToNextDay()
    {
        if (currentDay >= maxDays) return;

        currentTime = currentDay * cycleDuration;
    }

    
    public string GetTimeOfDayString()
    {
        float progress = (currentTime % cycleDuration) / cycleDuration;
        if (progress < 0.25f)
            return "Dawn";
        else if (progress < 0.5f)
            return "Day";
        else if (progress < 0.75f)
            return "Dusk";
        else
            return "Night";
    }

    
    public bool HasDayEventTriggered(int day)
    {
        if (day < 1 || day > maxDays) return false;
        return dayEventsTriggered[day];
    }

    
    public void SetTimeOfDay(float normalizedTime)
    {
        currentTime = (currentDay - 1) * cycleDuration + normalizedTime * cycleDuration;
    }

    
    public float GetNormalizedTime()
    {
        return (currentTime % cycleDuration) / cycleDuration;
    }

    
    public bool IsNight()
    {
        return GetNormalizedTime() >= 0.5f;
    }

    
    public float GetTimeRemainingInDay()
    {
        return cycleDuration - (currentTime % cycleDuration);
    }

}