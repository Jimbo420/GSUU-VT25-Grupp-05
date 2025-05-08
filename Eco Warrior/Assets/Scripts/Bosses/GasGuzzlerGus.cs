using UnityEngine;

public class GasGuzzlerGus : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Reference to the player transform.")]
    public Transform player;

    [Header("Gas Can Behavior")]
    [Tooltip("The distance at which Gus will pick up a gas can.")]
    public float gasCanPickupDistance = 1.5f;
    [Tooltip("The speed multiplier when Gus is moving toward a gas can.")]
    public float gasCanMoveSpeedMultiplier = 1.5f;

    private Transform currentGasCanTarget;
    private ChargeAttack chargeAttack; // Reference to the ChargeAttack script

    [Header("Component References")]
    private BossMovement movement;
    private BossAttack attack;
    private BossHealth health;
    private BossBoundaryHandler boundaryHandler;
    private BossEncounter encounter;

    private Animator animator;

    void Awake()
    {
        // Get references to the modular components
        movement = GetComponent<BossMovement>();
        attack = GetComponent<BossAttack>();
        health = GetComponent<BossHealth>();
        boundaryHandler = GetComponent<BossBoundaryHandler>();
        encounter = GetComponent<BossEncounter>();
        chargeAttack = GetComponent<ChargeAttack>();
        animator = GetComponent<Animator>();

        // Initialize components
        movement.Initialize(player, animator);
        attack.Initialize(animator, player);
        health.InitializeHealth();
    }

    void Update()
    {
        if (health.currentHealth <= 0)
            return; // Stop all logic if the boss is dead

        // Debug the gasCanMoveSpeedMultiplier value
        //Debug.Log($"[GasGuzzlerGus] Current gasCanMoveSpeedMultiplier: {gasCanMoveSpeedMultiplier}");

        // Check if the encounter should start
        if (!encounter.IsEncounterActive && Vector2.Distance(transform.position, player.position) < encounter.encounterStartDistance)
        {
            encounter.StartEncounter();
            return;
        }

        // Delegate logic to modular components
        if (encounter.IsEncounterActive)
        {
            HandleGasCanBehavior(); // Handle gas can-specific behavior
            HandleChargeAttack();   // Handle charge attack logic
            movement.HandleMovement();
            attack.HandleAttack();
            boundaryHandler.CheckBoundary();
        }
    }

    /// <summary>
    /// Handles gas can-specific behavior, such as moving toward and picking up gas cans.
    /// </summary>
    private void HandleGasCanBehavior()
    {
        // Skip gas can behavior if Gus is in the middle of a special attack
        if (chargeAttack != null && chargeAttack.IsCharging)
        {
            //Debug.Log("[GasGuzzlerGus] Skipping gas can behavior because Gus is performing a special attack.");
            return;
        }

        // Find the nearest gas can if no target is set
        if (currentGasCanTarget == null)
        {
            currentGasCanTarget = FindNearestGasCan();
        }

        if (currentGasCanTarget != null)
        {
            // Set the target to the gas can
            movement.SetTarget(currentGasCanTarget);

            // Apply the gas can speed multiplier directly
            movement.SetTemporaryMoveSpeed(movement.moveSpeed * gasCanMoveSpeedMultiplier);

            //Debug.Log($"[GasGuzzlerGus] Moving toward gas can. Target position: {currentGasCanTarget.position}, Speed: {movement.moveSpeed}");

            // Check if Gus is close enough to pick up the gas can
            if (Vector2.Distance(transform.position, currentGasCanTarget.position) <= gasCanPickupDistance)
            {
                PickUpGasCan();
            }
        }
        else
        {
            // Reset movement speed and target if no gas can is found
            movement.ResetMoveSpeed();
            movement.SetTarget(player);
        }
    }

    /// <summary>
    /// Finds the nearest gas can in the scene.
    /// </summary>
    /// <returns>The transform of the nearest gas can, or null if none are found.</returns>
    private Transform FindNearestGasCan()
    {
        GameObject[] gasCans = GameObject.FindGameObjectsWithTag("GasolineTank");
        Transform nearestGasCan = null;
        float shortestDistance = float.MaxValue;

        foreach (GameObject gasCan in gasCans)
        {
            float distance = Vector2.Distance(transform.position, gasCan.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestGasCan = gasCan.transform;
            }
        }

        return nearestGasCan;
    }

    /// <summary>
    /// Handles the logic for picking up a gas can.
    /// </summary>
    private void PickUpGasCan()
    {
        //Debug.Log("[GasGuzzlerGus] Gus picked up a gas can!");

        // Trigger the charge attack logic
        if (chargeAttack != null)
        {
            chargeAttack.EnableCharges(); // Enable charges in the ChargeAttack script
        }

        // Destroy the gas can
        Destroy(currentGasCanTarget.gameObject);

        // Clear the current gas can target
        currentGasCanTarget = null;

        // Reset movement speed
        movement.ResetMoveSpeed();
    }

    /// <summary>
    /// Handles the logic for triggering a charge attack.
    /// </summary>
    private void HandleChargeAttack()
    {
        if (chargeAttack != null && chargeAttack.HasChargesAvailable())
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Trigger the charge attack if the player is within a specific range
            if (distanceToPlayer > attack.attackRange && distanceToPlayer <= chargeAttack.chargeDistance)
            {
                //Debug.Log("[GasGuzzlerGus] Triggering charge attack toward player.");
                chargeAttack.PerformChargeIfAvailable();
            }
        }
    }
}