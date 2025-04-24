using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private WeaponManager _weaponManager;
    private float _nextFireTime;

    // Lock state to disable all inputs (movement, shooting, etc.)
    public bool isLocked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _weaponManager = GetComponentInChildren<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocked) return; // Prevent any input while locked

        // Switch weapon functionality
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _weaponManager.SwitchWeapon();
        }
    }

    void FixedUpdate()
    {
        if (isLocked) return; // Prevent shooting when locked
        if (EventSystem.current.IsPointerOverGameObject() || ItemDragHandler.IsDragging) return;
        if (!Input.GetButton("Fire1") || !(Time.time >= _nextFireTime)) return;

        _weaponManager.Shoot(true);

        // Uses FireRate to calculate when next bullet should be fired
        _nextFireTime = Time.time + (1f / _weaponManager.CurrentWeapon.FireRate);
    }
}
