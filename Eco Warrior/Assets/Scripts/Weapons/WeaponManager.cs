using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    private SpriteRenderer _toolSpriteRenderer;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private WeaponData[] _availableWeapons;
    [SerializeField] private Transform _gunHolder;
    [SerializeField] GameObject bulletPrefab;
    public WeaponData CurrentWeapon { get; private set; }
    private ToolRotator _toolRotator;
    private int _currentWeaponIndex = 0;

    void Start()
    {
        ReloadAmmunition();
        _toolRotator = GetComponentInParent<ToolRotator>();
        _toolSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CurrentWeapon = _availableWeapons[0];
        UpdateWeaponSprite();
    }


    void ReloadAmmunition()
    {
        foreach (var weapon in _availableWeapons)
            weapon.CurrentAmmunition = weapon.MaxAmmunition;
    }
    public void Shoot(bool isAiming = false)
    {

        if (!CurrentWeapon.HasUnlimitedAmmo && CurrentWeapon.CurrentAmmunition == 0) return;
        
        //If the player is shooting with mouse, first rotate the weapon
        if(isAiming) _toolRotator.RotateTool(isAiming);

        //Calculate the direction of bullet (to create the effect of inaccuracy or spread)
        float angleOffset = Random.Range(-CurrentWeapon.Accuracy, CurrentWeapon.Accuracy);
        Quaternion bulletRotation = _firePoint.rotation * Quaternion.Euler(0, 0, angleOffset - 90f);
        
        //Create the bullet game object and assign needed values
        GameObject bullet = Instantiate(bulletPrefab, _firePoint.position, bulletRotation);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.SetDamage(CurrentWeapon.Damage);
        bulletComponent.SetShooter(transform.parent.gameObject);
        
        //Give it a rigid body and add the force to make it move forward
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bullet.transform.up * CurrentWeapon.BulletSpeed, ForceMode2D.Impulse);
        CurrentWeapon.CurrentAmmunition--;
    }

    /// <summary>
    /// Switches weapon where it rotates between weapons in an array
    /// </summary>
    public void SwitchWeapon()
    {
        if (_currentWeaponIndex < _availableWeapons.Length - 1) 
            _currentWeaponIndex++;
        else
            _currentWeaponIndex = 0;
        CurrentWeapon = _availableWeapons[_currentWeaponIndex];
        UpdateWeaponSprite();
        UpdateWeaponAndGunHolderPosition();
    }
    /// <summary>
    /// When switching weapon, the sprite needs to be updated
    /// </summary>
    void UpdateWeaponSprite()
    {
        _toolSpriteRenderer.sprite = CurrentWeapon.WeaponSprite;
    }

    /// <summary>
    /// Updates the Firepoint where the pistol has a custom firepoint position
    /// compared to the other weapons
    /// </summary>
    /// <param name="xDirection"></param>
    /// <param name="yDirection"></param>
    public void UpdateWeaponOrientation(float xDirection, float yDirection)
    {
        //Pistol should be moved down slightly when selected
        if (CurrentWeapon.WeaponType == WeaponData.TypeOfWeapon.Pistol)
            _firePoint.localPosition = (int)xDirection == -1 ? new Vector3(0.9f, -0.13f, 0) : new Vector3(0.9f, 0.13f, 0);
        else
            _firePoint.localPosition = new Vector3(0.9f, 0, 0);

        UpdateWeaponAndGunHolderPosition(xDirection, yDirection);
    }

    /// <summary>
    /// Adjusts the position of the gun holder based on movement and direction of shooting
    /// The Y (up and down) of the weapon changes dynamically alongside the X to get a smooth transition
    /// going from horizontal to vertical
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void UpdateWeaponAndGunHolderPosition(float x = 0, float y = 0)
    {
        Vector3 targetOffset = Vector3.zero;

        // If the user is moving horizontally
        if (Mathf.RoundToInt(x) != 0)
            targetOffset.x = x * CurrentWeapon.GunHolderOffset.x;
        

        // Calculate the amount of horizontal movement (1 for horizontal, 0 for vertical movement)
        float xInfluence = Mathf.Clamp01(Mathf.Abs(x));

        // Apply a change depending on the horizontal movement
        float adjustedXInfluence = Mathf.Pow(xInfluence, 2);

        //Change the Y position based on the X
        float yOffset = Mathf.Lerp(0f, -0.5f, adjustedXInfluence); 

        //Custom adjustment if facing up
        if (y > 0.5f)
        {
            yOffset = -0.4f;
        }

        targetOffset.y = yOffset;

        _gunHolder.localPosition = targetOffset;
    }
}