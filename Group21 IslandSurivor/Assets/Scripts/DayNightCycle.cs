using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Skybox Materials")]
    public Material daySkybox;  // Assign your day1 skybox material
    public Material nightSkybox; // Assign your night2 skybox material

    [Header("Lighting")]
    public Light sunLight; // Your main directional light (sun)
    public Gradient dayColor; // Color gradient for day lighting
    public Gradient nightColor; // Color gradient for night lighting

    [Header("Timing")]
    public float cycleDuration = 120f; // 2 minutes in seconds

    [Header("Sun Rotation")]
    public Transform sunTransform; // Transform of your sun light

    private float currentTime = 0f;
    private bool isDay = true;

    void Start()
    {
        // Initialize with day settings
        if (daySkybox != null)
            RenderSettings.skybox = daySkybox;

        // Set up default gradients if not assigned
        SetupDefaultGradients();
    }

    void Update()
    {
        // Update time
        currentTime += Time.deltaTime;

        // Calculate progress through the cycle (0 to 1)
        float cycleProgress = (currentTime % cycleDuration) / cycleDuration;

        // Determine if it's day or night
        bool shouldBeDay = cycleProgress < 0.5f;

        // Switch skybox if day/night state changed
        if (shouldBeDay != isDay)
        {
            isDay = shouldBeDay;
            SwitchSkybox();
        }

        // Update lighting based on time
        UpdateLighting(cycleProgress);

        // Rotate sun
        RotateSun(cycleProgress);
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

        // Apply skybox changes immediately
        DynamicGI.UpdateEnvironment();
    }

    void UpdateLighting(float cycleProgress)
    {
        if (sunLight == null) return;

        // Calculate lighting intensity and color based on time
        float lightIntensity;
        Color lightColor;

        if (cycleProgress < 0.5f) // Day time (0 to 0.5)
        {
            float dayProgress = cycleProgress * 2f; // Convert to 0-1 range for day
            lightIntensity = Mathf.Lerp(0.3f, 1.2f, Mathf.Sin(dayProgress * Mathf.PI));
            lightColor = dayColor.Evaluate(dayProgress);
        }
        else // Night time (0.5 to 1)
        {
            float nightProgress = (cycleProgress - 0.5f) * 2f; // Convert to 0-1 range for night
            lightIntensity = Mathf.Lerp(0.1f, 0.3f, Mathf.Sin(nightProgress * Mathf.PI));
            lightColor = nightColor.Evaluate(nightProgress);
        }

        sunLight.intensity = lightIntensity;
        sunLight.color = lightColor;
    }

    void RotateSun(float cycleProgress)
    {
        if (sunTransform == null) return;

        // Rotate sun across the sky (180 degrees over full cycle)
        float sunAngle = cycleProgress * 180f - 90f; // -90 to 90 degrees
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
            // Default night colors: dark blue
            GradientColorKey[] nightColors = new GradientColorKey[2];
            nightColors[0] = new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 0f);
            nightColors[1] = new GradientColorKey(new Color(0.05f, 0.05f, 0.2f), 1f);

            GradientAlphaKey[] nightAlphas = new GradientAlphaKey[2];
            nightAlphas[0] = new GradientAlphaKey(1f, 0f);
            nightAlphas[1] = new GradientAlphaKey(1f, 1f);

            nightColor.SetKeys(nightColors, nightAlphas);
        }
    }

    // Optional: Method to set specific time of day
    public void SetTimeOfDay(float normalizedTime)
    {
        currentTime = normalizedTime * cycleDuration;
    }

   
    public float GetNormalizedTime()
    {
        return (currentTime % cycleDuration) / cycleDuration;
    }
}