using Assets.Scripts;
using UnityEngine;

public class SoundEmitter : MonoBehaviour, IPlaySound
{
    public Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void MakeSound(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in colliders)
        {
            EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.HearSound(player.position);

            }
        }
    }

    public void Play(AudioSource source, bool loop)
    {
        if (loop)
        {
            if (source.isPlaying) return;
            source.Play();
            MakeSound(5f);
        }
        else
        {
            source.PlayOneShot(source.clip);
            if (transform.gameObject.tag.Contains("Player"))
            { 
                MakeSound(7f);
            }
        }
    }
}
