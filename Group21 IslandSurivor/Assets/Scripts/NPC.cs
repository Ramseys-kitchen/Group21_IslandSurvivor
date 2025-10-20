using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CrypticGuide : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text promptText;

    [Header("Visual Effects")]
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 1f;
    public Color glowColor = new Color(0.5f, 0.8f, 1f);

    [Header("Audio")]
    public AudioClip[] dialogueSounds; // Array for different voice lines (optional)
    public AudioClip interactionSound; // Single sound that plays on interact
    public AudioClip ambientHumSound; // Mystical hum when player is near
    public AudioSource audioSource;
    public AudioSource ambientAudioSource; // Separate source for ambient loop

    private Transform player;
    private bool playerInRange = false;
    private Vector3 startPos;
    private int dialogueIndex = 0;
    private bool isShowingDialogue = false;
    private Material sphereMaterial;

    // Cryptic advice that hints at the real world metaphor
    private string[] crypticAdvice = new string[]
    {
        "Rest comes only after exhaustion. Strange how the body knows when to stop... unlike the mind, which never learned that lesson.",

        "Water here must be earned, searched for, valued. Back there, it poured endlessly from silver mouths you passed without seeing.",

        "Night brings dangers that weren't here in daylight. Or were they always present, just wearing different masks?",

        "You want to leave this place. But leaving requires knowing what you're returning to. Can you remember clearly?",

        "Shelter. Water. Warmth. The basics demand nothing but your attention. No performance. No approval. Just existence.",

        "The forest doesn't measure you. The rain doesn't score your worth. Have you felt this kind of silence before?",

        "You wander without knowing the path forward. This uncertainty... does it feel foreign, or just more visible?",

        "The sun doesn't ask what you accomplished yesterday. You're still here. Still breathing. Is that not its own answer?",

        "Threats emerge only in darkness here. The ones you knew... they had no schedule, no mercy, no off switch.",

        "You call this imprisonment. Yet you built these walls yourself, didn't you? Sometimes we create exactly what we need.",

        "To escape, first ask: what did you escape to? This wasn't random. Some part of you chose simplicity.",

        "A cage with open doors. A freedom that feels like chains. The lost can't always name what they've found."
    };

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;

        // Set up sphere material for glow effect
        sphereMaterial = GetComponent<Renderer>().material;
        sphereMaterial.EnableKeyword("_EMISSION");
        sphereMaterial.SetColor("_EmissionColor", glowColor);

        // Setup audio sources
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Setup ambient audio source for looping hum
        if (ambientAudioSource == null && ambientHumSound != null)
        {
            ambientAudioSource = gameObject.AddComponent<AudioSource>();
            ambientAudioSource.clip = ambientHumSound;
            ambientAudioSource.loop = true;
            ambientAudioSource.volume = 0f; // Start silent
            ambientAudioSource.Play();
        }

        // Hide dialogue initially
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // Floating animation
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Slow rotation for mystical effect
        transform.Rotate(Vector3.up, 20f * Time.deltaTime);

        // Check player distance
        float distance = Vector3.Distance(player.position, transform.position);
        playerInRange = distance <= interactionRange;

        // Fade ambient hum based on distance
        if (ambientAudioSource != null && ambientHumSound != null)
        {
            float targetVolume = playerInRange ? 0.3f : 0f;
            ambientAudioSource.volume = Mathf.Lerp(ambientAudioSource.volume, targetVolume, Time.deltaTime * 2f);
        }

        // Show/hide interaction prompt
        if (promptText != null)
        {
            promptText.gameObject.SetActive(playerInRange && !isShowingDialogue);
            if (playerInRange)
                promptText.text = "Press E to seek guidance";
        }

        // Handle interaction
        if (playerInRange && Input.GetKeyDown(interactKey) && !isShowingDialogue)
        {
            ShowDialogue();
        }
        else if (isShowingDialogue && Input.GetKeyDown(interactKey))
        {
            HideDialogue();
        }

        // Pulse glow when player is near
        if (playerInRange)
        {
            float pulse = Mathf.PingPong(Time.time * 2f, 0.5f) + 0.5f;
            sphereMaterial.SetColor("_EmissionColor", glowColor * pulse);
        }
    }

    void ShowDialogue()
    {
        isShowingDialogue = true;
        dialoguePanel.SetActive(true);

        // Cycle through advice
        dialogueText.text = crypticAdvice[dialogueIndex];

        // Play audio
        if (audioSource != null)
        {
            // Option 1: Use different sound for each dialogue line (if you have multiple)
            if (dialogueSounds != null && dialogueSounds.Length > 0)
            {
                // Play corresponding sound or random sound
                AudioClip soundToPlay = dialogueSounds[dialogueIndex % dialogueSounds.Length];
                audioSource.PlayOneShot(soundToPlay);
            }
            // Option 2: Use single interaction sound
            else if (interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound);
            }
        }

        dialogueIndex = (dialogueIndex + 1) % crypticAdvice.Length;

        // Optional: Pause game or slow time during dialogue
        // Time.timeScale = 0.3f;
    }

    void HideDialogue()
    {
        isShowingDialogue = false;
        dialoguePanel.SetActive(false);
        // Time.timeScale = 1f;
    }

    // Visualize interaction range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}