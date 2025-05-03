using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.AI;

public class TargetPlayer : MonoBehaviour
{
    private float stopDistance = 3f;
    private float rangeBetween = 10f;
    private float distance;
    private float viewDistance = 5f;
    private float viewAngle = 120f;
    public Vector2 lastFacingDirection = Vector2.right;
    private EnemyMovement enemyMovement;
    public Transform player;
    //private NavMeshAgent agent;

    private WeaponShooter _weaponShooter;
    private WeaponManager _weaponManager;

    public bool hasLineOfSight = false;
    private float _nextFireTime;
    private float lostSightTimer = 0f;
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 forward = ((Vector3)lastFacingDirection).normalized;
        if (forward == Vector3.zero) forward = Vector3.right; 

        float halfAngle = viewAngle * 0.5f;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -halfAngle) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, halfAngle) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);

        // Line to player
        Gizmos.color = hasLineOfSight ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, player.position);
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //agent = GetComponent<NavMeshAgent>();
        enemyMovement = GetComponent<EnemyMovement>();
        _weaponManager = GetComponentInChildren<WeaponManager>();
        _weaponShooter = GetComponentInChildren<WeaponShooter>();
    }

    void Update()
    {
        //if (lostSightTimer > 0)
        //    lostSightTimer -= Time.deltaTime;
    }

    public bool PlayerIsInRangeOfEnemy()
    {
        if (!hasLineOfSight) return false;

        Vector2 toPlayer = (player.position - transform.position).normalized;
        Vector2 forward = lastFacingDirection == Vector2.zero ? Vector2.right : lastFacingDirection;

        float dot = Vector2.Dot(forward, toPlayer);
        float angleThreshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
        float distance = Vector2.Distance(transform.position, player.position);

        return dot >= angleThreshold && distance <= viewDistance;
    }

    public void EngageTarget()
    {
        distance = Vector2.Distance(transform.position, player.position);
        if (distance > stopDistance)
        {
            enemyMovement.SetTarget(player.position);
            enemyMovement.Walk();
        }
        if (!(Time.time >= _nextFireTime)) return;
        _weaponShooter.Shoot();
        _nextFireTime = Time.time + (1f / _weaponManager.CurrentWeapon.FireRate);

    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector2 origin = transform.position;
        Vector2 direction = (player.position - transform.position).normalized;

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int layerMask = ~(1 << enemyLayer);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rangeBetween + 5f, layerMask);
        //|| hit.collider.gameObject.layer != LayerMask.NameToLayer("Wall")
        if (hit.collider != null )
        {
            hasLineOfSight = hit.collider.CompareTag("Player");
            //if (hit.collider.CompareTag("Player"))
            //{
            //    hasLineOfSight = true;
            //    //lostSightTimer = 0f;
            //}
            //else
            //{
            //    //if (hasLineOfSight)
            //        //lostSightTimer = 10;
            //    hasLineOfSight = false;
            //}
        }
        else
        {
            //if (hasLineOfSight)
                //lostSightTimer = 10f;
            hasLineOfSight = false;
        }
    }
}
