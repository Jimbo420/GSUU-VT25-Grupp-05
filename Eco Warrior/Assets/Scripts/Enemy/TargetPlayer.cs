using UnityEngine;
//using UnityEngine.AI;

public class TargetPlayer : MonoBehaviour
{
    private float stopDistance = 3f;
    private float rangeBetween = 10f;
    private float distance;

    private EnemyMovement enemyMovement;
    public Transform player;
    //private NavMeshAgent agent;

    private WeaponManager _weaponManager;

    private bool hasLineOfSight = false;
    private float _nextFireTime;
    private float lostSightTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //agent = GetComponent<NavMeshAgent>();
        enemyMovement = GetComponent<EnemyMovement>();
        _weaponManager = GetComponentInChildren<WeaponManager>();
    }

    void Update()
    {
        //if (lostSightTimer > 0)
        //    lostSightTimer -= Time.deltaTime;
    }

    public bool PlayerIsInRangeOfEnemy()
    {
        float distance = Vector2.Distance(player.position, transform.position);
        if (hasLineOfSight /*|| lostSightTimer > 0*/)
            return distance <= rangeBetween;
        else
            return false;
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
        _weaponManager.Shoot();
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
