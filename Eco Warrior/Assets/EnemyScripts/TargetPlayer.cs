using UnityEngine;

public class TargetPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform target;
    private GameObject bullet;
    private Transform aim;


    //[SerializeField] private SpriteRenderer _toolSpriteRenderer;
    //[SerializeField] private WeaponManager _weaponManager;
    //[SerializeField] private Transform _toolTransform;
    //[SerializeField] private Transform _firePointTransform;

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
        //Sprite selectedSprite = _weaponManager.CurrentWeapon.WeaponSprite;
    }
    void Update()
    {
        
    }
    public bool PlayerIsInRangeOfEnemy()
    {
        if (hasLineOfSight)
        {
            float distance = Vector2.Distance(player.position, transform.position); //Calculates the distance between player and enemy
            return distance <= rangeBetween;
        }
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
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector2 origin = transform.position;
        Vector2 direction = (player.position - this.transform.position).normalized;

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int layerMask = ~(1 << enemyLayer);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rangeBetween+5f, layerMask);

        if (hit.collider != null)
        {
            hasLineOfSight = hit.collider.CompareTag("Player");
            Debug.Log($"LOS: {hasLineOfSight}, Hit: {hit.collider.name}");
        }
        else
        {
            hasLineOfSight = false;
            Debug.Log($"LOS: {hasLineOfSight}");
        }
    }
}
