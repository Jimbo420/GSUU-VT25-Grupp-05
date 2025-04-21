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
        {(1, 0), 0f},      // Right
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
    
    public void RotateTool(Vector2 movement)
    {
        if (movement == Vector2.zero) return;

        Vector2 direction = movement.normalized;
        float x = Mathf.Round(direction.x);
        float y = Mathf.Round(direction.y);
        float rotateX = 0;
        float rotationAngle = _movementDictionary[(x, y)];

        if ((int)x == 1) rotateX = 180f;

        _toolTransform.rotation = Quaternion.Euler(rotateX, 0, rotationAngle);

        Sprite selectedSprite;
        selectedSprite = x != 0 ? _weaponManager.CurrentWeapon.SideSprite : _weaponManager.CurrentWeapon.WeaponSprite;
        

        _toolSpriteRenderer.sprite = selectedSprite;
        _weaponManager.UpdateWeaponOrientation((int)x, (int)y);
    }
}
