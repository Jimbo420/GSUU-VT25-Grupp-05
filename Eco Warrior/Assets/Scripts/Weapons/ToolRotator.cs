using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolRotator : MonoBehaviour
{
    private Transform _toolTransform;
    private SpriteRenderer _toolSpriteRenderer;

    void Start()
    {
        _toolTransform = transform.Find("GunHolder");
        _toolSpriteRenderer = _toolTransform.Find("Gun").GetComponent<SpriteRenderer>();
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
            direction = movement == Vector2.zero ? Vector2.right : movement;
        }

        ApplyRotation(direction);
    }


    public void RotateToolTowards(Vector2 worldPosition)
    {
        Vector2 direction = (worldPosition - (Vector2)_toolTransform.position);
        ApplyRotation(direction);
    }


    private void ApplyRotation(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _toolTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        _toolSpriteRenderer.flipY = direction.x < 0;
        try
        {
            _toolSpriteRenderer.sprite = GetComponentInChildren<WeaponManager>().CurrentWeapon.WeaponSprite;
            GetComponentInChildren<WeaponVisuals>().UpdateWeaponOrientation(direction.x, direction.y);

        }
        catch (Exception) {}
    }
}
