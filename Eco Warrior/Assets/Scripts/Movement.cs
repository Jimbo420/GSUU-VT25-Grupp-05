using System.Collections.Generic;
using Interfaces;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Movement : MonoBehaviour, IDamageable
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private Transform _toolTransform;
    [SerializeField] private Transform _firePointTransform;
    [SerializeField] private Slider _healthbar;
    [SerializeField] private SpriteRenderer _toolSpriteRenderer;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] public float health = 0;
    [SerializeField] public float maxHealth = 25;
    private HealthbarBehavior _healthbarBehavior;
    private float _rotationAngle = 0;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _animator;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _healthbarBehavior = GetComponentInChildren<HealthbarBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.linearVelocity = _movement * _moveSpeed;
    }

    private readonly Dictionary<(float, float), float> _movementDictionary = new()
    {
        {(0, 1), 90f},        // Up
        {(1, 1), -45f},      // Up-Right
        {(1, 0), 0f},      // Right
        {(1, -1), 45f},     // Down-Right
        {(0, -1), -90f},        // Down
        {(-1, -1), -135f},  // Down-Left
        {(-1, 0), -180f},   // Left
        {(-1, 1), 135f}    // Up-Left
    };


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

        if (_movement != Vector2.zero) ToolRotation();

      
    }

    private void ToolRotation()
    {
        //Depending on where the player is looking, rotate the weapon
        Vector2 direction = _movement.normalized;
        float x = Mathf.Round(direction.x);
        float y = Mathf.Round(direction.y);
        float rotateX = 0;
        _rotationAngle = _movementDictionary[(x, y)];
        if ((int)x == 1) rotateX = 180f;
        _toolTransform.rotation = Quaternion.Euler(rotateX, 0, _rotationAngle);

        //Depending on Rotation, switch which sprite is used
        Sprite selectedSprite = _weaponManager.CurrentWeapon.WeaponSprite;
        if (x != 0)
        {
            selectedSprite = _weaponManager.CurrentWeapon.SideSprite;
        }
        else if (x != 0 && y != 0)
        {
            selectedSprite = _weaponManager.CurrentWeapon.SideSprite;
        }

        _toolSpriteRenderer.sprite = selectedSprite;

        _weaponManager.UpdateWeaponOrientation((int)x, (int)y);
    }


    public void HitDamage(float damage)
    {
        health -= damage;
        _healthbarBehavior.Health(health, maxHealth);
        if (health <= 0)
            Destroy(gameObject);
    }
}
