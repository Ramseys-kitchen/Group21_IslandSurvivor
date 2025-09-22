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

    [Header("Warning Settings")]
    public float warningThreshold = 30f; 

    [Header("Player")]
    public Transform player;

    private Vector3 lastPlayerPosition;
    private bool isPlayerMoving = false;

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

        //hide warning prompt
        if (warningPrompt != null)
        {
            
            if (currentEnergy <= warningThreshold)
            {
                warningPrompt.SetActive(true);
            }
            else
            {
                warningPrompt.SetActive(false);
            }
        }
    }
}