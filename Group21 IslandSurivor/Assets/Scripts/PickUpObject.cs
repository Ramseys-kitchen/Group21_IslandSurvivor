using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupSpeed = 10f;
    [SerializeField] private float positionThreshold = 0.1f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool smoothPickup = true;

    private Rigidbody rb;
    private Transform holdPoint;
    private bool isBeingPickedUp = false;
    private bool isHeld = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isBeingPickedUp && !isHeld)
        {
            SmoothMoveToHoldPoint();
        }
    }

    public void PickUp(Transform holdPoint)
    {
        this.holdPoint = holdPoint;

        // Disable physics immediately
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // Prevent physics interference

        if (smoothPickup)
        {
            // Start smooth pickup
            isBeingPickedUp = true;
            targetPosition = holdPoint.position;
            targetRotation = holdPoint.rotation;
        }
        else
        {
            // Instant pickup
            transform.SetParent(holdPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            isHeld = true;
        }
    }

    private void SmoothMoveToHoldPoint()
    {
        if (holdPoint == null) return;

        // Update target position/rotation in case hold point moves
        targetPosition = holdPoint.position;
        targetRotation = holdPoint.rotation;

        // Smoothly move to target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, pickupSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if we're close enough to complete pickup
        if (Vector3.Distance(transform.position, targetPosition) <= positionThreshold)
        {
            CompletePickup();
        }
    }

    private void CompletePickup()
    {
        isBeingPickedUp = false;
        isHeld = true;

        // Parent to hold point
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        if (!isHeld && !isBeingPickedUp) return;

        // Reset state
        isHeld = false;
        isBeingPickedUp = false;
        holdPoint = null;

        // Re-enable physics
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void Throw(Vector3 impulse)
    {
        if (!isHeld && !isBeingPickedUp) return;

        // Reset state
        isHeld = false;
        isBeingPickedUp = false;
        holdPoint = null;

        // Re-enable physics and throw
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(impulse, ForceMode.Impulse);
    }

    // Legacy method for compatibility
    public void MoveToTheHoldPoint(Vector3 targetPosition)
    {
        if (rb.isKinematic)
        {
            transform.position = targetPosition;
        }
        else
        {
            rb.MovePosition(targetPosition);
        }
    }

    // Property to check if object can be picked up
    public bool CanBePickedUp => !isHeld && !isBeingPickedUp;

    // Property to check current state
    public bool IsHeld => isHeld;
}
