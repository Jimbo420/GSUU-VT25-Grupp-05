using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _toolSpriteRenderer;
    [SerializeField] private Transform _firePoint;
    [SerializeField] public WeaponData CurrentWeapon;
    [SerializeField] private Transform _gunHolder;

    void Start()
    {
        SetWeapon(CurrentWeapon);
    }
    public void SetWeapon(WeaponData weapon)
    {
        CurrentWeapon = weapon;
        UpdateWeaponSprite();
        UpdateFirePointPosition();
    }

    void UpdateWeaponSprite()
    {
        _toolSpriteRenderer.sprite = CurrentWeapon.WeaponSprite;
    }

    void UpdateFirePointPosition()
    {
        
        _firePoint.localPosition = new Vector3(0, CurrentWeapon.FirePointOffset.y, 0);
    }

    public void UpdateWeaponOrientation(int xDirection, int yDirection)
    {
        if (xDirection is 1 or -1)
        {
            if (CurrentWeapon.WeaponType == WeaponData.TypeOfWeapon.Pistol)
                _firePoint.localPosition = new Vector3(0, -0.13f, 0); 
            
        }
        else if(yDirection is 1 or -1)
            _firePoint.localPosition = new Vector3(0, 0, 0);

        
        UpdateWeaponAndGunHolderPosition(xDirection, yDirection);
    }
    public void UpdateWeaponAndGunHolderPosition(int x, int y)
    {

        if (x != 0)
            _gunHolder.localPosition = new Vector3(x * CurrentWeapon.GunHolderOffset.x, CurrentWeapon.GunHolderOffset.y);
        
        else if (y != 0)
            _gunHolder.localPosition = new Vector3(0, y * 0.9f); 
        


    }
}