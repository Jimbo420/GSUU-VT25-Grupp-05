using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolRotator : MonoBehaviour
{
    private WeaponManager _weaponManager;
    [SerializeField] private Transform _toolTransform;
    [SerializeField] private SpriteRenderer _toolSpriteRenderer;

    void Start()
    {
        _weaponManager = GetComponentInChildren<WeaponManager>();
    }

    public void RotateTool(bool isAiming, Vector2 movement = default)
    {
        Vector2 direction;

        if (isAiming)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (mouseWorldPos - _toolTransform.position);
        }
        else
        {
            if (movement == Vector2.zero) return;
            direction = movement;
        }


        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Rotate weapon based on angle
        _toolTransform.localEulerAngles = new Vector3(0f, 0f, angle);

        _toolSpriteRenderer.flipY = direction.x > 0;

        _toolSpriteRenderer.sprite = _weaponManager.CurrentWeapon.WeaponSprite;

        
        _weaponManager.UpdateWeaponOrientation(direction.x, direction.y);
    }
}
