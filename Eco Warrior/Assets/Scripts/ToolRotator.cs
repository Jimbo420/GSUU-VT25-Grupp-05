using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolRotator : MonoBehaviour
{
    private WeaponManager _weaponManager;
    [SerializeField] private Transform _toolTransform;
    [SerializeField] private SpriteRenderer _toolSpriteRenderer;

    private readonly Dictionary<(float, float), float> _movementDictionary = new()
    {   
        {(0, 1), 90f},        // Up
        {(1, 1), -45f},      // Up-Right
        {(1, 0), 180f},      // Right
        {(1, -1), 45f},     // Down-Right
        {(0, -1), -90f},        // Down
        {(-1, -1), -135f},  // Down-Left
        {(-1, 0), -180f},   // Left
        {(-1, 1), 135f}    // Up-Left
    };

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
            
            _toolSpriteRenderer.flipY = direction.x  > 0;
        
            // If aiming horizontally show side sprite; otherwise show the top 
            Sprite selectedSprite = (Mathf.Abs(direction.x) > 0.5f)
                ? _weaponManager.CurrentWeapon.SideSprite
                : _weaponManager.CurrentWeapon.WeaponSprite;
        
            _toolSpriteRenderer.sprite = selectedSprite;
            
            _weaponManager.UpdateWeaponOrientation(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
    }
}
