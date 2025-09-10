using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f; 
    public float detectionRange = 15f;
    public float stopDistance = 1.5f;

    [Header("Ground Check")]
    public float groundCheckDistance = 5f; 
    public LayerMask groundLayer = 1;

    private Transform player;
    private Rigidbody rb;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("SimpleEnemy: No GameObject with 'Player' tag found!");
        }

        
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        
        SnapToGround();
    }

    void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 10f, groundLayer))
        {
            transform.position = hit.point + Vector3.up * 0.5f; 
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }

       
        if (isChasing && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
        }
        else if (isChasing && distanceToPlayer <= stopDistance)
        {
           
            LookAtPlayer();
        }

        
        if (distanceToPlayer > detectionRange * 2f)
        {
            isChasing = false;
        }
    }

    void ChasePlayer()
    {
        
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Keep enemy on ground
        directionToPlayer = directionToPlayer.normalized;

        
        Vector3 movement = directionToPlayer * moveSpeed * Time.deltaTime;

        
        Vector3 newPosition = transform.position + movement;
        if (HasGroundBelow(newPosition))
        {
            transform.Translate(movement, Space.World);

           
            KeepOnGround();
        }

        
        LookAtPlayer();
    }

    void KeepOnGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f, groundLayer))
        {
            
            float targetY = hit.point.y + 1f;
            if (Mathf.Abs(transform.position.y - targetY) > 0.1f)
            {
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
            }
        }
    }

    void LookAtPlayer()
    {
        
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0; 

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f); 
        }
    }

    bool HasGroundBelow(Vector3 position)
    {
        
        RaycastHit hit;
        Vector3 rayStart = position + Vector3.up * 0.5f;

        return Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
      
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

      
        if (isChasing && player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }

        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * groundCheckDistance);
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy touched player!");
            
        }
    }
}