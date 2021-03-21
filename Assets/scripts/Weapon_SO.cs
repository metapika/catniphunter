using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName="Weapons/Weapon")]

public class Weapon_SO : ScriptableObject
{
    public enum WeaponType {
        Katana,
        Knife,
        DoubleSwords,
        Helberd
    }
    public WeaponType weaponType = WeaponType.Katana;
    public int damage = 20;
    public int attackSpeed = 15;
    public bool multipleAnimations = true;
    public string animationTrigger = "katana";
}
