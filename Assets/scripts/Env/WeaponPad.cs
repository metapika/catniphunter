using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponPad : MonoBehaviour
{
    #region Fields
    public GameObject activeWeapon;
    private Melee activeWeaponInfo;
    public float weaponRotationSpeed = 5f;
    private Transform player;
    private PlayerCombat playerCombat;
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
        player = GameObject.Find("RoboSamurai").transform;
        playerCombat = player.GetComponent<PlayerCombat>();

        foreach (Transform weapon in transform) {
            if(weapon.CompareTag("Weapon") || weapon.CompareTag("Melee") || weapon.CompareTag("Ability")) {
                activeWeapon = weapon.gameObject;
                activeWeaponInfo = activeWeapon.GetComponent<Melee>();
            }
        }

        UpdateWeaponInfoDisplay();

        if(activeWeapon)
        {
            foreach (Transform weapon in playerCombat.handR)
            {
                if(weapon.GetComponent<Melee>().weaponDefinition.weaponType == activeWeaponInfo.weaponDefinition.weaponType)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void Update() {
        WeaponRotation();

        if(weaponInfoPrompt == null) return;
        
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
