using System.Collections.Generic;
using Assets.Scripts;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    private ToolRotator _toolRotator;
    //[SerializeField] private Slider _healthbar;
    [SerializeField] private float moveSpeed = 2f;
    private AudioSource _footstepsSource;
    //[SerializeField] private GameObject _footstepsPrefab;
    // [SerializeField] private AudioClip _footstepsClip;

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _animator;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _toolRotator = GetComponent<ToolRotator>();
        _footstepsSource = GameObject.Find("Step Audio").GetComponent<AudioSource>();
        if (_toolRotator is not null)
            _toolRotator.RotateTool(false, _movement);
        
    }

    // Update is called once per frame
    void Update()
    {
        _rb.linearVelocity = _movement * _moveSpeed;
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            _moveSpeed = 6f;
            GetComponentInParent<SoundEmitter>().Play(_footstepsSource, true);
        }
        else
        {
            _moveSpeed = 4f;
        }

    }

    public void MoveCharacter(InputAction.CallbackContext context)
    {
        //TODO Fix
       
        
        _animator.SetBool("isWalking", true);
        
        //If the user no longer gives input 
        if (context.canceled)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetFloat("LastInputX", _movement.x);
            _animator.SetFloat("LastInputY", _movement.y);
            Stop();
        }
        _movement = context.ReadValue<Vector2>();
        _animator.SetFloat("InputX", _movement.x);
        _animator.SetFloat("InputY", _movement.y);

        if (_movement != Vector2.zero && _toolRotator is not null) _toolRotator.RotateTool( false, _movement);

      
    }
    public void Stop()
    {
       _footstepsSource.Stop();
    }
}
