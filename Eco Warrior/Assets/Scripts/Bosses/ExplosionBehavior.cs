using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    private void Start()
    {
        // Destroy the explosion after the animation length
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length; // Get the animation's duration
            Destroy(gameObject, animationLength); // Destroy the explosion after it finishes playing
        }
        else
        {
            // If no animator is attached, set a default lifetime
            Destroy(gameObject, 2f); // Default lifetime of 2 seconds
        }
    }
}
