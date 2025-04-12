using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private Transform _toolTransform;
    [SerializeField] private Transform _firePointTransform;
    [SerializeField] private Slider _healthbar;
    [SerializeField] private SpriteRenderer _toolSpriteRenderer;
    [SerializeField] private WeaponManager _weaponManager;

    private float _rotationAngle = 0;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _animator;
    //private int i = 160;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.linearVelocity = _movement * _moveSpeed;
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if(_healthbar.fillRect is null) Debug.Log("Healthbar rect is null");
        //    _healthbar.fillRect.offsetMax =
        //        new Vector2(_healthbar.fillRect.offsetMax.x - 10f, _healthbar.fillRect.offsetMax.y);
        //    i -= 10;
        //    if(i == 80) _healthbar.fillRect.GetComponent<Image>().color = Color.yellow;
        //    else if (i == 40) _healthbar.fillRect.GetComponent<Image>().color = Color.red;
        //    if(i == 0) Destroy(gameObject);
        //}
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
        _rotationAngle = _movementDictionary[(x, y)];
        if ((int)x == 1) rotateX = 180f;
        _toolTransform.rotation = Quaternion.Euler(rotateX, 0, _rotationAngle);


        if (x is 1 or -1)
            _toolSpriteRenderer.sprite = _weaponManager.CurrentWeapon.SideSprite;

        if (y is 1 or -1)
            _toolSpriteRenderer.sprite = _weaponManager.CurrentWeapon.WeaponSprite;
        
        _weaponManager.UpdateWeaponOrientation((int)x, (int)y);
    }

    

}
