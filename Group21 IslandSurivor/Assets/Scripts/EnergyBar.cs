using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float energyLossRate = 1f;
    public float energyRegenRate = 2f;

    [Header("UI")]
    public Slider energySlider;
    public GameObject warningPrompt;
    public Image energyWarningImage; // Your blue water droplet or energy icon

    [Header("Warning Settings")]
    public float warningThreshold = 30f;
    public float popupFadeSpeed = 2f; // How fast the popup fades in/out

    [Header("Audio")]
    public AudioClip energyWarningSound; // Your downloaded audio
    public AudioSource audioSource;

    [Header("Player")]
    public Transform player;

    private Vector3 lastPlayerPosition;
    private bool isPlayerMoving = false;
    private bool hasPlayedWarning = false;
    private float targetAlpha = 0f;
    private CanvasGroup warningCanvasGroup;

    void Start()
    {
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }

        if (warningPrompt != null)
        {
            warningPrompt.SetActive(false);
        }

        // Setup energy warning popup
        if (energyWarningImage != null)
        {
            // Add CanvasGroup for smooth fading
            warningCanvasGroup = energyWarningImage.GetComponent<CanvasGroup>();
            if (warningCanvasGroup == null)
            {
                warningCanvasGroup = energyWarningImage.gameObject.AddComponent<CanvasGroup>();
            }
            warningCanvasGroup.alpha = 0f;
        }

        // Setup audio
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    void Update()
    {
        CheckIfPlayerIsMoving();
        UpdateEnergy();
        UpdateUI();
        UpdateWarningPopup();
    }

    void CheckIfPlayerIsMoving()
    {
        if (player != null)
        {
            float distanceMoved = Vector3.Distance(player.position, lastPlayerPosition);
            isPlayerMoving = distanceMoved > 0.01f;
            lastPlayerPosition = player.position;
        }
    }

    void UpdateEnergy()
    {
        if (isPlayerMoving)
        {
            // Player is moving - lose energy
            currentEnergy -= energyLossRate * Time.deltaTime;
        }
        else
        {
            // Player is not moving - gain energy
            currentEnergy += energyRegenRate * Time.deltaTime;
        }

        // Keep energy between 0 and max
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
    }

    void UpdateUI()
    {
        if (energySlider != null)
        {
            energySlider.value = currentEnergy;
        }

        // Check if we need to show warning
        if (currentEnergy <= warningThreshold)
        {
            if (warningPrompt != null)
            {
                warningPrompt.SetActive(true);
            }

            // Play audio warning once
            if (!hasPlayedWarning && energyWarningSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(energyWarningSound);
                hasPlayedWarning = true;
            }

            targetAlpha = 1f; // Show warning image
        }
        else
        {
            if (warningPrompt != null)
            {
                warningPrompt.SetActive(false);
            }

            hasPlayedWarning = false;
            targetAlpha = 0f; // Hide warning image
        }
    }

    void UpdateWarningPopup()
    {
        if (warningCanvasGroup != null)
        {
            // Smoothly fade in/out
            warningCanvasGroup.alpha = Mathf.Lerp(
                warningCanvasGroup.alpha,
                targetAlpha,
                Time.deltaTime * popupFadeSpeed
            );
        }
    }
}