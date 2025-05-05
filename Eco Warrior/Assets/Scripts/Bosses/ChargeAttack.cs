using UnityEngine;

public class ChargeAttack : BossSpecialAttack
{
    [Header("Charge Attack Settings")]
    public float chargeDistance = 10f;
    public float chargeSpeed = 20f;
    public int maxChargesAfterGasCan = 3;
    public float pauseBetweenCharges = 1f;

    [Header("Fire Trail Settings")]
    public GameObject fireTrailPrefab;
    public float fireTrailLifetime = 5f;
    public float fireTrailMinScale = 2f;
    public float fireTrailMaxScale = 3f;
    public bool fireTrailRandomRotation = true;
    public float fireTrailSpawnInterval = 0.1f;

    [Header("Wall Detection Settings")]
    public LayerMask wallLayer;

    [Header("Animation Settings")]
    public string movementAnimationParameter = "Speed";

    [Header("Audio Settings")]
    public AudioClip chargeSound;
    private AudioSource audioSource;
    private Transform player;
    private Animator animator;
    private BossAttack bossAttack;
    private bool isCharging = false;
    private int remainingCharges = 0;

    public bool IsCharging => isCharging; // Expose the isCharging field

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        bossAttack = GetComponent<BossAttack>();

        // Ensure AudioSource exists
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("[ChargeAttack] No AudioSource found. Adding one dynamically.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void EnableCharges()
    {
        remainingCharges = maxChargesAfterGasCan;
    }

    public bool HasChargesAvailable()
    {
        return remainingCharges > 0 && !isCharging;
    }

    public void PerformChargeIfAvailable()
    {
        if (HasChargesAvailable())
        {
            PerformAttack();
        }
    }

    protected override void PerformAttack()
    {
        if (isCharging || remainingCharges <= 0)
        {
            Debug.LogWarning("Charge attack cannot be performed right now.");
            return;
        }

        Debug.Log("Performing charge attack!");

        // Play the charge sound
        PlayChargeSound();

        Vector2 chargeDirection = (player.position - transform.position).normalized;

        if (animator != null)
        {
            animator.SetFloat("IdleX", Mathf.Round(chargeDirection.x));
            animator.SetFloat("IdleY", Mathf.Round(chargeDirection.y));
            animator.SetFloat(movementAnimationParameter, chargeSpeed);
        }

        StartCoroutine(ChargeCoroutine(chargeDirection));
    }

    private System.Collections.IEnumerator ChargeCoroutine(Vector2 direction)
    {
        isCharging = true;
        bossAttack.SetSpecialAttackActive(true);

        float distanceCharged = 0f;
        float fireTrailTimer = 0f;

        while (distanceCharged < chargeDistance)
        {
            if (IsWallInDirection(direction))
            {
                Debug.Log("Charge stopped due to wall collision.");
                break;
            }

            Vector3 movement = chargeSpeed * Time.deltaTime * direction;
            transform.position += movement;
            distanceCharged += movement.magnitude;

            fireTrailTimer += Time.deltaTime;
            if (fireTrailPrefab != null && fireTrailTimer >= fireTrailSpawnInterval)
            {
                SpawnFireTrail();
                fireTrailTimer = 0f;
            }

            yield return null;
        }

        yield return new WaitForSeconds(pauseBetweenCharges);

        isCharging = false;
        bossAttack.SetSpecialAttackActive(false);
        remainingCharges--;
        Debug.Log("Charge attack completed.");
    }

    private void PlayChargeSound()
    {
        if (chargeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(chargeSound);
            Debug.Log("[ChargeAttack] Playing charge sound.");
        }
        else
        {
            Debug.LogWarning("[ChargeAttack] Charge sound or AudioSource is missing.");
        }
    }

    private void SpawnFireTrail()
    {
        GameObject fireTrail = Instantiate(fireTrailPrefab, transform.position, Quaternion.identity);
        Destroy(fireTrail, fireTrailLifetime);

        float randomScale = Random.Range(fireTrailMinScale, fireTrailMaxScale);
        fireTrail.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        if (fireTrailRandomRotation)
        {
            float randomRotation = Random.Range(-30f, 30f);
            fireTrail.transform.Rotate(0f, 0f, randomRotation);
        }
    }

    private bool IsWallInDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, chargeSpeed * Time.deltaTime, wallLayer);
        return hit.collider != null;
    }
}
