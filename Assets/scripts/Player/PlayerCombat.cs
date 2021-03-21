using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    #region Fields
    public Transform currentAbilities;
    public GameObject currentWeapon;
    public Transform weaponCase;
    public Transform weaponHand;
    public Transform weaponHand2;
    public Transform cameraTarget;
    public Transform knifeThrowPoint;
    public GameObject crossair;
    public float offsetToDashedEnemy = 1.2f;
    public float minOffsetToDash = 0.2f;
    public Vector3 offset = new Vector3(0, .8f, 0);

    public List<Transform> targets;
    public int targetIndex;
    public LayerMask maskForCoverCheck;
    private bool targetNotBehindCover;

    private CharacterController controller;
    private PlayerPhysics pphysics;
    private PlayerController movement;
    private PlayerStats stats;
    private Animator anim;
    private AbilityManager abilityList;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");

    #endregion

    #region Unity Functions
    private void Awake() {
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerController>();
        pphysics = GetComponent<PlayerPhysics>();
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        abilityList = currentAbilities.GetComponent<AbilityManager>();

        currentWeapon = GetObtainedWeapons();

        if(currentWeapon != null) {
            if(currentWeapon.GetComponent<Melee>().weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
                currentWeapon.GetComponent<Melee>().sword1.GetComponent<BoxCollider>().enabled = false;
                currentWeapon.GetComponent<Melee>().sword2.GetComponent<BoxCollider>().enabled = false;
            } else {
                currentWeapon.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
    private void Update() {
        DetectTargets();
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("WeaponPad")) {
            WeaponPad pad = other.GetComponent<WeaponPad>();

            if(pad.activeWeapon != null) {
                if(pad.activeWeapon.CompareTag("Weapon") || pad.activeWeapon.CompareTag("Melee") ) {
                    EquipWeapon(pad.activeWeapon);
                    pad.anim.enabled = false;
                    pad.activeWeapon = null;
                } else if(pad.activeWeapon.CompareTag("Ability")) {
                    EquipMod(pad.activeWeapon.GetComponent<AbilityID>().ID);
                    Destroy(pad.activeWeapon);
                    pad.activeWeapon = null;
                }
            }
        }
    }
    #endregion

    #region Custom Functions
    private void DetectTargets() {
        if (targets.Count > 0)
        {
            movement.canRotate = false;
            targetIndex = NearestTargetToCenter();

            RaycastHit hit;
            Vector3 direction = targets[targetIndex].position - transform.position;     
            Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, targets[targetIndex].position), maskForCoverCheck);

            if(hit.transform != null)
            {
                if(hit.transform.gameObject.CompareTag("Enemy"))
                    targetNotBehindCover = true;
                else
                    targetNotBehindCover = false;
            }
            
            if(pphysics.IsGrounded() && targetNotBehindCover) {
                if(transform.position.y - targets[targetIndex].position.y < 3 && transform.position.y - targets[targetIndex].position.y > -3) 
                {
                    transform.LookAt(new Vector3(targets[targetIndex].position.x, transform.position.y, targets[targetIndex].position.z));
                }
            }
        } else {
            movement.canRotate = true;
        }
    }
    public void MoveTowardsTarget(Transform target = null)
    {
        if(target == null) 
        {
            target = targets[targetIndex];
        }

        if (Vector3.Distance(transform.position, target.position) > minOffsetToDash && Vector3.Distance(transform.position, target.position) < 10)
        {
            if(currentWeapon.GetComponent<Melee>().weaponDefinition.weaponType != Weapon_SO.WeaponType.Knife) {
                transform.DOMove(TargetOffset(), .3f);
                if(pphysics.IsGrounded()) 
                {
                    if(transform.position.y - targets[targetIndex].position.y < 3 && transform.position.y - targets[targetIndex].position.y > -3)
                    {
                        transform.DOLookAt(new Vector3(targets[targetIndex].position.x, transform.position.y, targets[targetIndex].position.z), .2f);
                    }
                }
            }
        }
    }
    private int NearestTargetToCenter()
    {
        float[] distances = new float[targets.Count];

        for (int i = 0; i < targets.Count; i++)
        {
            distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(targets[i].position), new Vector2(Screen.width / 2, Screen.height / 2));
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }
        return index;
    }

    public Vector3 TargetOffset()
    {
        Vector3 position;
        position = targets[targetIndex].position + offset;
        return Vector3.MoveTowards(position, transform.position, offsetToDashedEnemy);
    }
    private void EquipWeapon(GameObject weapon) {
        currentWeapon = weapon;
        
        currentWeapon.transform.SetParent(weaponCase);
        currentWeapon.transform.position = weaponCase.position;
        currentWeapon.transform.rotation = weaponCase.rotation;

        if(weapon.GetComponent<Melee>() != null) {
            weapon.GetComponent<Melee>().InitializeEquip();
        }
    }
    private GameObject GetObtainedWeapons() {
        GameObject selectedWeapon = null;

        foreach (Transform possibleWeapon in weaponCase) {
            if(possibleWeapon.CompareTag("Weapon") || possibleWeapon.CompareTag("Melee")) {
                selectedWeapon = possibleWeapon.gameObject;
                return selectedWeapon;
            }
        }

        if(selectedWeapon == null) {
            foreach (Transform possibleWeapon in weaponHand) {
                if(possibleWeapon.CompareTag("Weapon") || possibleWeapon.CompareTag("Melee")) {
                    if(selectedWeapon == null) {
                        selectedWeapon = possibleWeapon.gameObject;
                        return selectedWeapon;
                    }
                }
            } 
        }

        return selectedWeapon;
    }
    private void EquipMod(int ID) {
        var mod = abilityList.allMods[ID];
        var abilityID = mod.GetComponent<AbilityID>();
        var newID = abilityID.ID;
        
        if (!abilityList.collectedMods.Any(a => a.GetComponent<AbilityID>().ID == newID)) {
            Instantiate(abilityList.allMods[ID], currentAbilities.position, Quaternion.identity, currentAbilities);
        }

        abilityList.ReloadMods();
    }
    public void EnableCutter()
    {
        if(currentWeapon.GetComponent<Melee>().weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
            currentWeapon.GetComponent<Melee>().sword1.GetComponent<BoxCollider>().enabled = true;
            currentWeapon.GetComponent<Melee>().sword2.GetComponent<BoxCollider>().enabled = true;
        } else {
            currentWeapon.GetComponent<BoxCollider>().enabled = true;
        }
        
    }
    public void DisableCutter()
    {
        if(currentWeapon.GetComponent<Melee>().weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
            currentWeapon.GetComponent<Melee>().sword1.GetComponent<BoxCollider>().enabled = false;
            currentWeapon.GetComponent<Melee>().sword2.GetComponent<BoxCollider>().enabled = false;
        } else {
            currentWeapon.GetComponent<BoxCollider>().enabled = false;
        }
    }
    public void StartCombo() {
        if(currentWeapon.GetComponent<Melee>().weaponDefinition.weaponType == Weapon_SO.WeaponType.Katana) {
            currentWeapon.GetComponent<Melee>().trail.Play();
        }
        if(targets.Count > 0)
        {
            MoveTowardsTarget();
        }

        currentWeapon.GetComponent<Melee>().canAttack = false;
        movement.canMove = false;
        anim.SetBool("inCombat", true);
    }
    public void StopCombo() {
        if(currentWeapon.GetComponent<Melee>().weaponDefinition.weaponType == Weapon_SO.WeaponType.Katana) {
            currentWeapon.GetComponent<Melee>().trail.Stop();
        }
        currentWeapon.GetComponent<Melee>().canAttack = true;
        movement.canMove = true;
    }
    public void NoLongerInCombat() {
        anim.SetBool("inCombat", false);

        currentWeapon.GetComponent<Melee>().UnequipSword();
    }

    #endregion
}
