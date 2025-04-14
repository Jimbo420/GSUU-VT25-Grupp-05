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
    private Transform player;
    private Transform enemy;


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
        float distance = Vector2.Distance(player.position, transform.position);
        return distance <= rangeBetween;
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
}
