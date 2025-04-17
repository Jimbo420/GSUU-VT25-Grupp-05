using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private WeaponManager _weaponManager;
    private float _nextFireTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _weaponManager = GetComponentInChildren<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _weaponManager.SwitchWeapon();
        }
    }

    void FixedUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject() || ItemDragHandler.IsDragging) return;
        if (!Input.GetButton("Fire1") || !(Time.time >= _nextFireTime)) return;

        _weaponManager.Shoot();
        
        //Uses FireRate to calculate when next bullet should be fired
        _nextFireTime = Time.time + (1f/_weaponManager.CurrentWeapon.FireRate);
    }
}
