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
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private bool isPlayerInRange = false;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Update()
    {
        if (target == null) return;
        IsTargetInRange();
        if (isPlayerInRange)
            EngageTarget();
    }
    private void IsTargetInRange()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= ingageDistance) //Player is in range
            isPlayerInRange = true;
        else
            isPlayerInRange = false; //Player is to far away
    }
    private void EngageTarget()
    {
        if (Vector2.Distance(transform.position, target.position) > distanceBetween)
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }
}
