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

    [SerializeField] public float distanceBetween;
    [SerializeField] public float ingageDistance;
    [SerializeField] public float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private bool isPlayerInRange = false;
    private float rangeBetween = 10f;

    private EnemyMovement enemyMovement;
    private Transform player;
    private Transform enemy;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
    }
    void Update()
    {
        
    }

    public bool PlayerIsInRangeOfEnemy()
    {
        float distance = Vector2.Distance(player.position, enemy.position);
        return distance <= rangeBetween;
    }
    
    public void EngageTarget()
    {
        if (Vector2.Distance(transform.position, player.position) > distanceBetween)
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }
}
