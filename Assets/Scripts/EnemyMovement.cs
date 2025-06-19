using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("Radius around the start position in which the enemy will pick random patrol points.")]
    public float patrolRadius = 10f;
    [Tooltip("Movement speed while patrolling.")]
    public float patrolSpeed = 3f;
    [Tooltip("How close to a patrol point before picking a new one.")]
    public float pointReachedThreshold = 0.5f;

    [Header("Detection & Chase Settings")]
    [Tooltip("Transform of the player to detect and chase. If left empty, will search for tag 'Player'.")]
    public Transform player;
    [Tooltip("Distance within which the enemy will detect the player.")]
    public float detectionRange = 15f;
    [Tooltip("Allowed vertical difference (Y) to detect the player on the same level.")]
    public float detectionHeight = 2f;
    [Tooltip("Movement speed while chasing the player.")]
    public float chaseSpeed = 6f;

    [Header("Collision Settings")]
    [Tooltip("Layers considered as walls. On collision, a new patrol point is chosen.")]
    public LayerMask wallLayer;

    [SerializeField] float _rotationSpeed;//Facu Morales

    private Rigidbody rb;
    private Vector3 originPosition;
    private Vector3 patrolPoint;
    private bool isChasing = false;
    private float normalChaseSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originPosition = transform.position;
        normalChaseSpeed = chaseSpeed;

        // If no player assigned, try to find by tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        ChooseNewPatrolPoint();
    }

    void Update()
    {
        if (player != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            float heightDiff = Mathf.Abs(transform.position.y - player.position.y);

            // Check detection conditions
            isChasing = (distToPlayer <= detectionRange && heightDiff <= detectionHeight);
        }

        if (isChasing && player != null)
        {
            MoveTowards(player, chaseSpeed);

            Quaternion placeToRotate = Quaternion.LookRotation(player.position, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, placeToRotate, _rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Patrol behavior
            //MoveTowards(patrolPoint, patrolSpeed); //Facu Morales

            // If reached patrol point, pick a new one
            if (Vector3.Distance(transform.position, patrolPoint) < pointReachedThreshold)
            {
                ChooseNewPatrolPoint();
            }
        }
    }

    /*private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 newPos = Vector3.MoveTowards(rb.position, target, speed * Time.deltaTime);
        rb.MovePosition(newPos);

        

    }*/

    private void MoveTowards(Transform target, float speed)
    {
        Vector3 newPos = (target.transform.position - rb.transform.position);
        if (newPos.magnitude > 10)
        {
            rb.MovePosition(rb.position + newPos.normalized * speed * Time.fixedDeltaTime);
        }

         //Cambiar el MoveTowars por un AddForce o .Velocity.
        
    }


    private void ChooseNewPatrolPoint()
    {
        // Random point in XZ plane around origin
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        patrolPoint = originPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    void OnCollisionEnter(Collision collision)
    {
        // If colliding with a wall layer, choose a new patrol point
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            ChooseNewPatrolPoint();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw patrol radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        // Draw detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw height detection limits as horizontal circles
        Gizmos.color = Color.blue;
        Vector3 top = transform.position + Vector3.up * detectionHeight;
        Vector3 bottom = transform.position + Vector3.down * detectionHeight;
        Gizmos.DrawWireSphere(top, detectionRange);
        Gizmos.DrawWireSphere(bottom, detectionRange);
    }
}
