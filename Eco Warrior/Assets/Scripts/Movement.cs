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

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _animator;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _toolRotator = GetComponent<ToolRotator>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.linearVelocity = _movement * _moveSpeed;
    }

   


    public void MoveCharacter(InputAction.CallbackContext context)
    {
        _animator.SetBool("isWalking", true);
        //If the user no longer gives input
        if (context.canceled)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetFloat("LastInputX", _movement.x);
            _animator.SetFloat("LastInputY", _movement.y);

        }
        _movement = context.ReadValue<Vector2>();
        _animator.SetFloat("InputX", _movement.x);
        _animator.SetFloat("InputY", _movement.y);

        if (_movement != Vector2.zero) _toolRotator.RotateTool( false, _movement);

      
    }

   

}
