using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public float soundRange = 10f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    public void MakeSound()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soundRange);
        foreach (var hit in colliders)
        {
            EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.HearSound(player.position);
            }
        }
    }
}
