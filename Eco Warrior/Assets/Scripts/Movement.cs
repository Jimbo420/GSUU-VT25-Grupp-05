using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private Transform _firePointTransform;
    [SerializeField] private ToolRotator _toolRotator;
    [SerializeField] private Slider _healthbar;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private AudioSource footstepsSource;
    [SerializeField] private AudioClip footstepsClip;
    [SerializeField] private float stepInterval = 0.4f;

    private float stepTimer = 0f;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _animator;

    // Add a locking flag
    public bool isLocked = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _toolRotator = GetComponent<ToolRotator>();
    }

    void Update()
    {
        // Prevent movement if locked
        if (isLocked)
        {
            _rb.linearVelocity = Vector2.zero; // Stop the player's velocity
            return;
        }

        // Apply movement if not locked
        _rb.linearVelocity = _movement * _moveSpeed;

        bool isWalking = _movement != Vector2.zero; 

        if (isWalking)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                Debug.Log("Försöker spela fotstegsljud!");

                // Spela ljudet EN gång (PlayOneShot är bättre om du vill kunna spela överlappande ljud)
                footstepsSource.PlayOneShot(footstepsClip);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = stepInterval; // Gör så att nästa gång du börjar gå spelas ljudet direkt
        }

    }

    public void MoveCharacter(InputAction.CallbackContext context)
    {
        // Prevent player input if locked
        if (isLocked)
        {
            _movement = Vector2.zero; // Reset movement vector
            _animator.SetBool("isWalking", false); // Ensure no walking animation
            return;
        }

        _animator.SetBool("isWalking", true);

        // If the user no longer gives input
        if (context.canceled)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetFloat("LastInputX", _movement.x);
            _animator.SetFloat("LastInputY", _movement.y);
        }

        // Process movement input
        _movement = context.ReadValue<Vector2>();
        _animator.SetFloat("InputX", _movement.x);
        _animator.SetFloat("InputY", _movement.y);

        if (_movement != Vector2.zero)
            _toolRotator.RotateTool(false, _movement);
    }
}
