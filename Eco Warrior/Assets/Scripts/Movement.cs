using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private Transform _toolTransform;
    [SerializeField] private Transform _effectTransform;
    private float _rotationAngle = 0;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _animator;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.linearVelocity = _movement * _moveSpeed;
    }

    private readonly Dictionary<(float, float), (float, Vector2)> _movementDictionary = new()
    {
        {(0, 1), (90f, new Vector2(0f, 0.6f))},        // Up
        {(1, 1), (-45f, new Vector2(0.5f, 0.5f))},      // Up-Right
        {(1, 0), (0f, new Vector2(1f, -0.2f))},      // Right
        {(1, -1), (45f, new Vector2(0.5f, -0.5f))},     // Down-Right
        {(0, -1), (-90f, new Vector2(0f, -0.9f))},        // Down
        {(-1, -1), (-135f, new Vector2(-0.5f, -0.5f))},  // Down-Left
        {(-1, 0), (-180f, new Vector2(-1f, -0.2f))},   // Left
        {(-1, 1), (135f, new Vector2(-0.5f, 0.5f))}    // Up-Left
    };


    public void MoveCharacter(InputAction.CallbackContext context)
    {
        _animator.SetBool("isWalking", true);
        if (context.canceled)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetFloat("LastInputX", _movement.x);
            _animator.SetFloat("LastInputY", _movement.y);

        }
        _movement = context.ReadValue<Vector2>();
        _animator.SetFloat("InputX", _movement.x);
        _animator.SetFloat("InputY", _movement.y);

        if (_movement != Vector2.zero) ToolRotation();

      
    }

    private void ToolRotation()
    {
        Vector2 direction = _movement.normalized;
        float x = Mathf.Round(direction.x);
        float y = Mathf.Round(direction.y);
        float rotateX = 0;
        _rotationAngle = _movementDictionary[(x, y)].Item1;
        if (x == 1 && y == 0 || x == 1 && y == -1 || x == 1 && y == 1) rotateX = 180f;
        _toolTransform.rotation = Quaternion.Euler(rotateX, 0, _rotationAngle);
        _effectTransform.rotation = Quaternion.Euler(rotateX, 0, _rotationAngle);
        _toolTransform.localPosition = _movementDictionary[(x, y)].Item2;
    }

    

}
