using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    [SerializeField] HealthbarBehavior healthbarBehavior;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] public float health = 0;
    [SerializeField] public float maxHealth = 10;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthbarBehavior = GetComponentInChildren<HealthbarBehavior>();

        health = maxHealth;
        healthbarBehavior.Health(health, maxHealth);
    }
    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
            HitDamage(1);
        else if (Input.GetKeyDown(KeyCode.H))
            Heal();
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);
        if(context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }
    public void GiveDamage()
    {

    }
    public void HitDamage(float hitDamage)
    {
        health -= hitDamage;
        healthbarBehavior.Health(health, maxHealth);
        if (health <= 0)
            Dead();
    }

    public void Heal()
    {
        for(float healValue = health; healValue < maxHealth; healValue++)
        {
            health = healValue;
            healthbarBehavior.Health(healValue, maxHealth);
        }
    }
    public void Dead()
    {
        Destroy(gameObject);
    }
}
