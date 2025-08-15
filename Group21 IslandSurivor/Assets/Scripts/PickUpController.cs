using UnityEngine;
using System.Collections.Generic;

public class PickupController : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float pickupRange = 5f;
    [SerializeField] private LayerMask pickupLayer = -1;
    [SerializeField] private float sphereRadius = 0.5f; // For SphereCast detection

    [Header("Hold Point")]
    [SerializeField] private Transform holdPoint;

    [Header("UI")]
    [SerializeField] private GameObject pickupPrompt; // Optional UI element

    [Header("Input")]
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private KeyCode throwKey = KeyCode.Q;

    private PickupObject currentHeldObject;
    private PickupObject currentTargetObject;

    void Start()
    {
        // Create hold point if not assigned
        if (holdPoint == null)
        {
            GameObject holdPointGO = new GameObject("HoldPoint");
            holdPoint = holdPointGO.transform;
            holdPoint.SetParent(playerCamera.transform);
            holdPoint.localPosition = new Vector3(0, -0.5f, 1.5f); // Adjust as needed
        }

        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

    void Update()
    {
        if (currentHeldObject == null)
        {
            DetectPickupObjects();
            HandlePickupInput();
        }
        else
        {
            HandleThrowInput();
        }

        UpdateUI();
    }

    void DetectPickupObjects()
    {
        PickupObject previousTarget = currentTargetObject;
        currentTargetObject = null;

        // Use SphereCast for more forgiving detection
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

        // Try multiple detection methods for best results
        RaycastHit[] hits = Physics.SphereCastAll(ray.origin, sphereRadius, ray.direction, pickupRange, pickupLayer);

        float closestDistance = float.MaxValue;
        PickupObject closestPickupable = null;

        foreach (RaycastHit hit in hits)
        {
            PickupObject pickup = hit.collider.GetComponent<PickupObject>();
            if (pickup != null && pickup.CanBePickedUp)
            {
                float distance = Vector3.Distance(playerCamera.transform.position, hit.point);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPickupable = pickup;
                }
            }
        }

        // Fallback: Check for pickups in a sphere around the camera
        if (closestPickupable == null)
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(playerCamera.transform.position, pickupRange, pickupLayer);

            foreach (Collider col in nearbyColliders)
            {
                PickupObject pickup = col.GetComponent<PickupObject>();
                if (pickup != null && pickup.CanBePickedUp)
                {
                    // Check if it's roughly in front of the player
                    Vector3 directionToObject = (col.transform.position - playerCamera.transform.position).normalized;
                    float dotProduct = Vector3.Dot(playerCamera.transform.forward, directionToObject);

                    if (dotProduct > 0.3f) // Object is roughly in front (adjust threshold as needed)
                    {
                        float distance = Vector3.Distance(playerCamera.transform.position, col.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestPickupable = pickup;
                        }
                    }
                }
            }
        }

        currentTargetObject = closestPickupable;
    }

    void HandlePickupInput()
    {
        if (Input.GetKeyDown(pickupKey) && currentTargetObject != null)
        {
            PickupObject(currentTargetObject);
        }
    }

    void HandleThrowInput()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            DropObject();
        }
        else if (Input.GetKeyDown(throwKey))
        {
            ThrowObject();
        }
    }

    void PickupObject(PickupObject obj)
    {
        currentHeldObject = obj;
        currentTargetObject = null;
        obj.PickUp(holdPoint);
    }

    void DropObject()
    {
        if (currentHeldObject != null)
        {
            currentHeldObject.Drop();
            currentHeldObject = null;
        }
    }

    void ThrowObject()
    {
        if (currentHeldObject != null)
        {
            Vector3 throwForce = playerCamera.transform.forward * 10f; // Adjust force as needed
            currentHeldObject.Throw(throwForce);
            currentHeldObject = null;
        }
    }

    void UpdateUI()
    {
        if (pickupPrompt != null)
        {
            bool shouldShowPrompt = currentTargetObject != null && currentHeldObject == null;
            pickupPrompt.SetActive(shouldShowPrompt);
        }
    }

    // Gizmos for debugging
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerCamera.transform.position, pickupRange);

            Gizmos.color = Color.red;
            Vector3 forward = playerCamera.transform.forward * pickupRange;
            Gizmos.DrawRay(playerCamera.transform.position, forward);

            if (holdPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(holdPoint.position, Vector3.one * 0.1f);
            }
        }
    }
}