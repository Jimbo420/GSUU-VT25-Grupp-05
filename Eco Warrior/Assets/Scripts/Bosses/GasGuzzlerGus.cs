using System.Collections;
using UnityEngine;

public class GasGuzzlerGus : MonoBehaviour
{
    // Boss Variables
    public Transform player;          // Reference to the player
    public float moveSpeed = 3f;      // Gus's movement speed
    public float attackRange = 2f;    // Radius of Gus's attack area
    public float attackCooldown = 2f; // Cooldown time between attacks
    [SerializeField] private float attackDamage = 3f;  // Damage dealt to the player during attack
    [SerializeField] private float knockbackForce = 5f; // Knockback force applied to the player

    [SerializeField] private float maxHealth = 100f;   // Gus's maximum health
    private float currentHealth;       // Gus's current health

    // Health System and Phases
    private bool hasStartedConversation = false;       // Tracks room entry conversation
    private bool[] phaseTriggered = new bool[3];       // Tracks health phases (75%, 50%, 25%)

    // Chat Bubble System
    public GameObject chatBubblePrefab;  // Prefab for the chat bubble
    public Transform chatBubbleParent;  // Parent transform for positioning the chat bubble
    private GameObject activeChatBubble; // Tracks the active chat bubble

    // Player Lock
    private bool isPlayerLocked = false;

    // Animator and Healthbar
    private Animator animator;
    public HealthbarBehavior healthBar;

    // Attack Timer
    private float lastAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.Health(currentHealth, maxHealth); // Initialize the health bar
    }

    void Update()
    {
        if (!hasStartedConversation && IsPlayerClose())
        {
            StartConversation("So you're the one who's been meddling in my factory! Time to pay!");
            hasStartedConversation = true;
        }

        CheckHealthPhases();

        if (!isPlayerLocked)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                if (Time.time > lastAttackTime + attackCooldown)
                {
                    StartAttack();
                }
            }

            Vector2 direction = (player.position - transform.position).normalized;
            animator.SetFloat("IdleX", Mathf.Round(direction.x));
            animator.SetFloat("IdleY", Mathf.Round(direction.y));
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        animator.SetFloat("IdleX", Mathf.Round(direction.x));
        animator.SetFloat("IdleY", Mathf.Round(direction.y));
        animator.SetFloat("Speed", moveSpeed);
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
                    Debug.Log($"Gus attacked the player and dealt {attackDamage} damage!");
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
        Debug.Log("Knockback interpolation complete!");
    }

    public void HitDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.Health(currentHealth, maxHealth);
        Debug.Log($"GasGuzzlerGus took {damage} damage! Current Health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("GasGuzzlerGus has been defeated!");
        Destroy(gameObject);
    }

    private bool IsPlayerClose()
    {
        return Vector2.Distance(transform.position, player.position) < attackRange * 2; // Adjust room entry range
    }

    private void StartConversation(string message)
    {
        if (activeChatBubble != null) // Prevent multiple bubbles
        {
            return;
        }

        isPlayerLocked = true; // Lock the player during conversation
        player.GetComponent<Movement>().isLocked = true; // Lock player movement
        player.GetComponent<PlayerController>().isLocked = true; // Lock shooting and other actions
        animator.SetFloat("Speed", 0); // Stop Gus's running animation
        animator.SetTrigger("Idle");  // Set idle animation

        activeChatBubble = Instantiate(chatBubblePrefab, chatBubbleParent);
        activeChatBubble.transform.localScale = Vector3.one;
        activeChatBubble.transform.localPosition = Vector3.zero; // Center chat bubble above Gus
        activeChatBubble.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;

        StartCoroutine(EndConversation());
    }

    private IEnumerator EndConversation()
    {
        yield return new WaitForSeconds(3f); // Conversation duration

        if (activeChatBubble != null)
        {
            Destroy(activeChatBubble); // Remove chat bubble
            activeChatBubble = null;  // Reset reference
        }

        isPlayerLocked = false; // Unlock player movement
        player.GetComponent<Movement>().isLocked = false; // Unlock player movement
        player.GetComponent<PlayerController>().isLocked = false; // Unlock shooting and other actions
        animator.ResetTrigger("Idle"); // Resume normal animations
    }

    private void CheckHealthPhases()
    {
        if (currentHealth <= maxHealth * 0.75f && !phaseTriggered[0])
        {
            StartConversation("You dare challenge me further? I'll make this your grave!");
            phaseTriggered[0] = true;
        }
        else if (currentHealth <= maxHealth * 0.5f && !phaseTriggered[1])
        {
            StartConversation("This battle is far from over! Witness my power!");
            phaseTriggered[1] = true;
        }
        else if (currentHealth <= maxHealth * 0.25f && !phaseTriggered[2])
        {
            StartConversation("You've pushed me to my limits, but I'll not fall easily!");
            phaseTriggered[2] = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
