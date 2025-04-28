using UnityEngine;

public class TargetPlayer : MonoBehaviour
{
    private float stopDistance = 3f;
    private float rangeBetween = 10f;
    private float distance;

    private EnemyMovement enemyMovement;
    public Transform player;
    private WeaponManager _weaponManager;


    private bool hasLineOfSight = false;
    private float _nextFireTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        enemyMovement = GetComponent<EnemyMovement>();
        _weaponManager = GetComponentInChildren<WeaponManager>();
        
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
        if (!(Time.time >= _nextFireTime)) return; 
        //_weaponManager.Shoot();
        _nextFireTime = Time.time + (1f/_weaponManager.CurrentWeapon.FireRate);
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
            //Debug.Log($"LOS: {hasLineOfSight}, Hit: {hit.collider.name}");
        }
        else
        {
            hasLineOfSight = false;
            //Debug.Log($"LOS: {hasLineOfSight}");
        }
    }
}
