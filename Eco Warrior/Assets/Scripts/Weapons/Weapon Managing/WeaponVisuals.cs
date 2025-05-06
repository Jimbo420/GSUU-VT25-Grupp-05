using UnityEngine;

public class WeaponVisuals : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _gunHolder;
    private SpriteRenderer _toolSpriteRenderer;
    private WeaponManager _weaponManager;
    private ToolRotator _toolRotator;
    public Transform GetFirePoint() => _firePoint;

    void Awake()
    {
        Test();
    }

    private void Test()
    {
        _weaponManager = GetComponent<WeaponManager>();
        _toolSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _toolRotator = GetComponentInParent<ToolRotator>();
    }
    public ToolRotator GetToolRotator() => _toolRotator;
    /// <summary>
    /// When switching weapon, the sprite needs to be updated
    /// </summary>
    public void UpdateWeaponSprite()
    {
        Test();
        _toolSpriteRenderer.sprite = _weaponManager.CurrentWeapon.WeaponSprite;
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
        if (_weaponManager.CurrentWeapon.WeaponType == WeaponData.TypeOfWeapon.Pistol)
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
            targetOffset.x = x * _weaponManager.CurrentWeapon.GunHolderOffset.x;


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
