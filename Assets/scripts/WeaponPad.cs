using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponPad : MonoBehaviour
{
    #region Fields
    public GameObject activeWeapon;
    public float weaponRotationSpeed = 5f;
    public Transform player;
    public WeaponInfo_SO weaponInfoDefinition;
    public GameObject weaponInfoPrompt;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponDescription;
    public TextMeshProUGUI weaponReward;
    public float promptDisplayDistance = 4f;

    [HideInInspector] public Animator anim;

    #endregion

    #region Unity Functions
    
    private void Start() {
        anim = GetComponent<Animator>();

        foreach (Transform weapon in transform) {
            if(weapon.CompareTag("Weapon") || weapon.CompareTag("Melee") || weapon.CompareTag("Ability")) {
                activeWeapon = weapon.gameObject;
            }
        }

        UpdateWeaponInfoDisplay();
    }

    private void Update() {
        WeaponRotation();

        if(Vector3.Distance(transform.position, player.position) < promptDisplayDistance) 
        {
            weaponInfoPrompt.SetActive(true);
            weaponInfoPrompt.transform.LookAt(Camera.main.transform.position);
        }
        if(Vector3.Distance(transform.position, player.position) > promptDisplayDistance || activeWeapon == null) {
            weaponInfoPrompt.SetActive(false);
        }
    }

    #endregion

    #region Custom Functions
    private void WeaponRotation() {
        if(activeWeapon != null) {
            if(activeWeapon.CompareTag("Weapon")) {
                activeWeapon.transform.Rotate(0, 0, weaponRotationSpeed);
            } else if(activeWeapon.CompareTag("Ability")) {
                activeWeapon.transform.Rotate(0, weaponRotationSpeed, 0);
            }
        }
    }

    public void UpdateWeaponInfoDisplay() {
        if(weaponInfoPrompt != null) 
        {
            weaponName.text = weaponInfoDefinition.weaponName;
            weaponDescription.text = weaponInfoDefinition.description;
            weaponReward.text = weaponInfoDefinition.reward.ToString();
        }
    }

    #endregion
}
