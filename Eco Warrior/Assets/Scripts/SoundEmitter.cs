using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public float soundRange = 10f;
    public void MakeSound(Vector2 originalPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soundRange);
        foreach (var hit in colliders)
        {
            EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.HearSound(originalPosition);
            }
        }
    }
}
