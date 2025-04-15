using UnityEngine;

public class TargetPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform target;
    private GameObject bullet;
    private Transform aim;
    public float fireDamage = 1F;
    public float fireCooldown = 0.25F;
    public float fireTime = 0.25F;

    [SerializeField] public float moveSpeed = 2f;
    private float stopDistance = 3f;
    private float rangeBetween = 10f;
    private float distance;

    private EnemyMovement enemyMovement;
    public Transform player;
    private Transform enemy;

    private bool hasLineOfSight = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
        enemyMovement = GetComponent<EnemyMovement>();
    }
    void Update()
    {
        
    }
    public bool PlayerIsInRangeOfEnemy()
    {
        if(hasLineOfSight)
        {
            float distance = Vector2.Distance(player.position, transform.position); //Calculates the distance between player and enemy
            return distance <= rangeBetween;
        }
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
    }

    private void FixedUpdate()
    {
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        if(raycast.collider != null)
        {
            hasLineOfSight = raycast.collider.CompareTag("Player");
        }
    }
}
