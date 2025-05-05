using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewBossPhase", menuName = "Boss/BossPhase")]
public class BossPhase : ScriptableObject
{
    [Header("Phase Settings")]
    public float healthThreshold; // Health percentage to trigger this phase
    public string phaseMessage; // Message to display when this phase starts

    [Header("Behavior Modifiers")]
    [Tooltip("Custom key-value pairs for modifying boss properties (e.g., 'MoveSpeed', 'AttackDamage').")]
    public Modifier[] modifiers; // Array of key-value pairs for generic property modification

    [Header("Phase Events")]
    [Tooltip("Custom UnityEvents to trigger during this phase.")]
    public UnityEvent onPhaseStart; // Events to trigger when the phase starts
    public UnityEvent onPhaseEnd; // Events to trigger when the phase ends

    [Header("Spawning Settings")]
    [Tooltip("Prefab to spawn during this phase.")]
    public GameObject spawnPrefab; // The prefab to spawn
    [Tooltip("Number of objects to spawn.")]
    public int spawnQuantity = 1; // Number of objects to spawn
    [Tooltip("Whether to use all specified spawn points or random ones.")]
    public bool useAllSpawnPoints = false; // Whether to use all specified spawn points or random ones

    [Tooltip("Spawn points to use for this phase.")]
    public SpawnPointType spawnPointType = SpawnPointType.Default; // Which spawn points to use
    [Tooltip("Custom spawn points (only used if 'Custom' is selected).")]
    public Transform[] customSpawnPoints; // Custom spawn points for this phase

    public enum SpawnPointType
    {
        Default, // Use default spawn points
        Special, // Use special spawn points
        Custom   // Use custom spawn points
    }
}

[System.Serializable]
public class Modifier
{
    public string key; // The name of the property to modify (e.g., "MoveSpeed")
    public float value; // The value to apply (e.g., multiplier or absolute value)
}
