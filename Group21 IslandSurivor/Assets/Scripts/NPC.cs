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

    private Transform player;
    private bool playerInRange = false;
    private Vector3 startPos;
    private int dialogueIndex = 0;
    private bool isShowingDialogue = false;
    private Material sphereMaterial;

    // Cryptic advice that hints at the real world metaphor
    private string[] crypticAdvice = new string[]
    {
        "To survive, you must rest... but rest only comes when you're truly exhausted. Just like deadlines that never let you sleep...",

        "The water here is scarce, precious. In another world, it flows freely from metal mouths. You never appreciated those fountains, did you?",

        "The darkness brings monsters. Or perhaps... the monsters were always there, wearing the faces of expectations and grades.",

        "You seek an escape from this island. But what if this island IS the escape? What were you running from?",

        "A shelter, some food, stay warm. Funny how survival is so simple here. No essays. No exams. No disappointment.",

        "The trees don't judge you. The water doesn't give you grades. Have you noticed how peaceful it is when nothing expects anything from you?",

        "You're confused, lost, unsure of the way forward. Tell me... is this feeling new to you? Or just more honest?",

        "Every morning the sun rises without asking if you've earned it. Every night you're still alive. That's enough here. Was it ever enough there?",

        "The enemies only come at night. Your real enemies... they came at all hours, didn't they? In reminders, notifications, dreams.",

        "You think you're trapped here. But you built this place in your mind. This island, this isolation... it's your sanctuary from the storm.",

        "To leave, you must first understand why you arrived. You wished for this. For simplicity. For survival to mean something tangible.",

        "Is this a prison or a privilege? Is your real life a privilege or a prison? The confused cannot tell the difference."
    };

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;

        // Set up sphere material for glow effect
        sphereMaterial = GetComponent<Renderer>().material;
        sphereMaterial.EnableKeyword("_EMISSION");
        sphereMaterial.SetColor("_EmissionColor", glowColor);

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