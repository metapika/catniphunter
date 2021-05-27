using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName="Weapons/Weapon Info")]
public class WeaponInfo_SO : ScriptableObject
{
    public string weaponName = "CyberKatana";
    
    [TextArea(3, 10)]
    public string description = "A katana made of the best parts you could find in your garage in hope of having something you can rely on";
    public int reward;

}
