using UnityEngine;
using UnityEngine.UI;

public class ThirstBar : MonoBehaviour
{
    [Header("Thirst Settings")]
    public float maxThirst = 100f;
    public float currentThirst = 100f;
    public float thirstLossRate = 0.5f; // Thirst lost per second over time
    public float waterRestoreAmount = 30f; // Amount restored when drinking

    [Header("UI")]
    public Slider thirstSlider;
    public GameObject warningPrompt; // "You're getting thirsty!" text

    [Header("Warning Settings")]
    public float warningThreshold = 25f; // Show warning when thirst drops below this

    void Start()
    {
        
        if (thirstSlider != null)
        {
            thirstSlider.maxValue = maxThirst;
            thirstSlider.value = currentThirst;
        }

        
        if (warningPrompt != null)
        {
            warningPrompt.SetActive(false);
        }
    }

    void Update()
    {
        UpdateThirst();
        UpdateUI();
    }

    void UpdateThirst()
    {
        
        currentThirst -= thirstLossRate * Time.deltaTime;

        
        currentThirst = Mathf.Clamp(currentThirst, 0f, maxThirst);
    }

    void UpdateUI()
    {
        if (thirstSlider != null)
        {
            thirstSlider.value = currentThirst;
        }

        
        if (warningPrompt != null)
        {
            if (currentThirst <= warningThreshold)
            {
                warningPrompt.SetActive(true);
            }
            else
            {
                warningPrompt.SetActive(false);
            }
        }
    }

   
    public void DrinkWater()
    {
        currentThirst += waterRestoreAmount;
        currentThirst = Mathf.Clamp(currentThirst, 0f, maxThirst);
        Debug.Log("Thirst restored!");
    }
}