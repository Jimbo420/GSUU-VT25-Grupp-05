using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _toolSpriteRenderer;
    [SerializeField] private Transform _firePoint;
    public WeaponData CurrentWeapon = null;
    [SerializeField] private WeaponData[] _availableWeapons;
    [SerializeField] private Transform _gunHolder;
    private int _currentWeaponIndex = 0;

    void Start()
    {
        SetWeapon(_availableWeapons[2]);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentWeaponIndex = (_currentWeaponIndex + 1) % _availableWeapons.Length;
            SetWeapon(_availableWeapons[_currentWeaponIndex]);
        }
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

        if (x != 0 & y > 0)
        {
            _gunHolder.localPosition = new Vector3(x * 0.65f, 0.4f);
        }
        else if (x != 0 && y < 0)
        {
            _gunHolder.localPosition = new Vector3(x * 0.65f, -0.4f);

        }



    }
}