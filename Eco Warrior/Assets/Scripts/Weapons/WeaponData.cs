using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    [SerializeField] public TypeOfWeapon WeaponType;
    [SerializeField] public Sprite WeaponSprite;
    public float FireRate; //How often shots are fired
    public float Accuracy; //Spread of the weapon
    public float Damage;
    public float BulletSpeed; //How fast they fire (might standardize)

    //Adjusts the position of where bullets are fired from and how gun is held
    public Vector2 FirePointOffset;
    public Vector2 GunHolderOffset;
    public enum TypeOfWeapon
    {
        Pistol,
        MachineGun,
        SMG
    }
}
