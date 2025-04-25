using System.Collections;
using UnityEngine;

public class GasGuzzlerGus : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float gasTankMoveSpeed = 6f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    [SerializeField] private float attackDamage = 3f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool hasStartedConversation = false;
    private bool[] phaseTriggered = new bool[3];
    public GameObject chatBubblePrefab;
    public Transform chatBubbleParent;
    private GameObject activeChatBubble;
    private bool isPlayerLocked = false;
    private Animator animator;
    public HealthbarBehavior healthBar;
    private float lastAttackTime;
    public GameObject gasolineTankPrefab;
    public Transform[] spawnPoints;
    private bool isSpecialAttackActive = false;
    public float specialAttackSpeed = 12f;
    public float encounterStartDistance = 5f;
    private bool isEncounterActive = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.Health(currentHealth, maxHealth);
        InvokeRepeating(nameof(SpawnGasolineTank), 10f, 20f);
    }

    void Update()
    {
        if (!isEncounterActive && Vector2.Distance(transform.position, player.position) < encounterStartDistance)
        {
            StartEncounter();
            return;
        }

        if (!isEncounterActive || isSpecialAttackActive || isPlayerLocked)
        {
            animator.SetFloat("Speed", 0);
            return;
        }

        GameObject closestTank = FindClosestGasolineTank();
        if (closestTank != null)
        {
            MoveTowards(closestTank.transform.position, gasTankMoveSpeed);
            if (Vector2.Distance(transform.position, closestTank.transform.position) < 1f)
                ConsumeGasolineTank(closestTank);
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
                ChasePlayer();
            else if (Time.time > lastAttackTime + attackCooldown)
                StartAttack();

            Vector2 direction = (player.position - transform.position).normalized;
            animator.SetFloat("IdleX", Mathf.Round(direction.x));
            animator.SetFloat("IdleY", Mathf.Round(direction.y));
        }
    }

    private void StartEncounter()
    {
        isEncounterActive = true;
        StartConversation("So you're the one who's been meddling in my factory! You will pay dearly for this!");
    }

    void ChasePlayer()
    {
        MoveTowards(player.position, moveSpeed);
    }

    void StartAttack()
    {
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }

    public void PerformAttack()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                HealthbarBehavior playerHealth = hit.GetComponentInChildren<HealthbarBehavior>();
                if (playerHealth != null)
                {
                    playerHealth.HitDamage(attackDamage, hit.gameObject);
                }
                ApplyKnockback(hit);
            }
        }
    }

    void ApplyKnockback(Collider2D playerCollider)
    {
        Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockbackDirection = (playerCollider.transform.position - transform.position).normalized;
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = Vector2.up;
            }
            StartCoroutine(InterpolateKnockback(playerRb, knockbackDirection));
        }
    }

    IEnumerator InterpolateKnockback(Rigidbody2D playerRb, Vector2 direction)
    {
        float knockbackDuration = 0.2f;
        float elapsedTime = 0f;
        Vector2 startPosition = playerRb.position;
        Vector2 targetPosition = startPosition + direction * knockbackForce;
        playerRb.linearVelocity = Vector2.zero;

        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / knockbackDuration;
            playerRb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            yield return null;
        }

        playerRb.MovePosition(targetPosition);
    }

    public void HitDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.Health(currentHealth, maxHealth);
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private bool IsPlayerClose()
    {
        return Vector2.Distance(transform.position, player.position) < attackRange * 2;
    }

    private void StartConversation(string message)
    {
        if (activeChatBubble != null)
            return;

        isPlayerLocked = true;
        player.GetComponent<Movement>().isLocked = true;
        player.GetComponent<PlayerController>().isLocked = true;
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Idle");

        activeChatBubble = Instantiate(chatBubblePrefab, chatBubbleParent);
        activeChatBubble.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;
        StartCoroutine(EndConversation());
    }

    private IEnumerator EndConversation()
    {
        yield return new WaitForSeconds(3f);
        if (activeChatBubble != null)
        {
            Destroy(activeChatBubble);
            activeChatBubble = null;
        }
        isPlayerLocked = false;
        player.GetComponent<Movement>().isLocked = false;
        player.GetComponent<PlayerController>().isLocked = false;
        animator.ResetTrigger("Idle");
    }

    private void CheckHealthPhases()
    {
        if (currentHealth <= maxHealth * 0.75f && !phaseTriggered[0])
        {
            TriggerPhase(0, "You dare challenge me further? I'll make this your grave!");
        }
        else if (currentHealth <= maxHealth * 0.5f && !phaseTriggered[1])
        {
            TriggerPhase(1, "This battle is far from over! Witness my power!");
        }
        else if (currentHealth <= maxHealth * 0.25f && !phaseTriggered[2])
        {
            TriggerPhase(2, "You've pushed me to my limits, but I'll not fall easily!");
        }
    }

    private void TriggerPhase(int phaseIndex, string message)
    {
        StartConversation(message);
        phaseTriggered[phaseIndex] = true;
    }

    private void SpawnGasolineTank()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(gasolineTankPrefab, spawnPoint.position, Quaternion.identity);
    }

    private GameObject FindClosestGasolineTank()
    {
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("GasolineTank");
        GameObject closestTank = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject tank in tanks)
        {
            float distance = Vector2.Distance(transform.position, tank.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTank = tank;
            }
        }

        return closestTank;
    }

    private void ConsumeGasolineTank(GameObject tank)
    {
        Destroy(tank);
        StartSpecialAttack();
    }

    private void StartSpecialAttack()
    {
        StartCoroutine(SpecialAttackPattern());
    }

    private IEnumerator SpecialAttackPattern()
    {
        isSpecialAttackActive = true;

        for (int i = 0; i < 3; i++)
        {
            Vector2 chargeDirection = (player.position - transform.position).normalized;

            animator.SetFloat("IdleX", Mathf.Round(chargeDirection.x));
            animator.SetFloat("IdleY", Mathf.Round(chargeDirection.y));
            animator.SetFloat("Speed", specialAttackSpeed * 2f);

            float chargeDistance = 15f;
            float chargeSpeed = specialAttackSpeed * 3f;
            float elapsedTime = 0f;

            while (elapsedTime < chargeDistance / chargeSpeed)
            {
                transform.position += (Vector3)(chargeDirection * chargeSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, 1f);
            if (hitPlayer != null && hitPlayer.CompareTag("Player"))
            {
                Rigidbody2D playerRb = hitPlayer.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
                hitPlayer.GetComponent<HealthbarBehavior>().HitDamage(attackDamage, gameObject);
            }

            yield return new WaitForSeconds(1f);
        }

        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Idle");
        isSpecialAttackActive = false;
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        animator.SetFloat("IdleX", Mathf.Round(direction.x));
        animator.SetFloat("IdleY", Mathf.Round(direction.y));
        animator.SetFloat("Speed", speed);
    }
}
