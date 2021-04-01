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

    public bool canDetectEnemies = true;
    public List<Transform> targets;
    public int targetIndex;
    public int lockOnTargetIndex;
    public LayerMask maskForCoverCheck;
    private bool targetNotBehindCover;

    private CharacterController controller;
    private PlayerPhysics pphysics;
    private PlayerController movement;
    private PlayerStats stats;
    private Animator anim;
    private AbilityManager abilityList;
    private CameraController camControl;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");

    #endregion

    #region Unity Functions
    private void Awake() {
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerController>();
        pphysics = GetComponent<PlayerPhysics>();
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        camControl = Camera.main.GetComponent<CameraController>();
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
        TargetDetection();

        if(Input.GetKeyDown(KeyCode.R) && targetNotBehindCover && targets.Count > 0)
        {
            camControl.ToggleCameraLockOn();
        }
        if(camControl.CameraToggleState()) {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                AddIndex(-1, targets.Count);
            } 
            else if(Input.GetKeyDown(KeyCode.E)) 
            {
                AddIndex(1, targets.Count);
            }
        }
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
    public void AddIndex(int amount, int max) {
        if ((lockOnTargetIndex += amount) >= max) {
            lockOnTargetIndex -= max;
            return;
        }

        if (lockOnTargetIndex >= 0) {
            return;
        }

        lockOnTargetIndex = max + lockOnTargetIndex;
    }
    private void TargetDetection() {
        if(!canDetectEnemies) return;

        if(!camControl.CameraToggleState()) movement.canRotate = true;
        else if(targetNotBehindCover && camControl.CameraToggleState()) movement.canRotate = false;
        
        if (targets.Count > 0)
        {
            targetIndex = NearestTargetToCenter();
            
            if(!camControl.CameraToggleState()) lockOnTargetIndex = NearestTargetToCenter();

            RaycastHit hit;
            Vector3 direction = Vector3.zero;
                 
            if(camControl.CameraToggleState()) {
                direction = targets[lockOnTargetIndex].position - transform.position;
                Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, targets[lockOnTargetIndex].position), maskForCoverCheck);
            }
            else{
                direction = targets[targetIndex].position - transform.position;
                Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, targets[targetIndex].position), maskForCoverCheck);
            }

            if(hit.transform != null)
            {
                if(hit.transform.gameObject.CompareTag("Enemy")) targetNotBehindCover = true;
                else targetNotBehindCover = false;
            }

            if(targetNotBehindCover && camControl.CameraToggleState()) {
                int rotSpeed = 1;
                Vector3 dir = targets[lockOnTargetIndex].position - transform.position;
                dir.y = 0f;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), Time.time * rotSpeed);
            }
        } else if(camControl.CameraToggleState()) {
            camControl.ToggleCameraLockOn();
        }
    }
    public void MoveTowardsTarget(int index, Transform target = null)
    {
        if(target == null) 
        {
            target = targets[index];
        }

        if (Vector3.Distance(transform.position, target.position) > minOffsetToDash && Vector3.Distance(transform.position, target.position) < 10)
        {
            if(currentWeapon.GetComponent<Melee>().weaponDefinition.weaponType != Weapon_SO.WeaponType.Knife) {
                if(camControl.CameraToggleState()) transform.DOMove(TargetOffset(lockOnTargetIndex), .3f);
                else transform.DOMove(TargetOffset(targetIndex), .3f);

                //if(transform.position.y - targets[index].position.y < 3 && transform.position.y - targets[index].position.y > -3)
                transform.DOLookAt(new Vector3(targets[index].position.x, transform.position.y, targets[index].position.z), .2f);
            }
        }
    }
    private int NearestTargetToCenter()
    {
        //Need to merge the for loops
        float[] distances = new float[targets.Count];

        for (int i = 0; i < targets.Count; i++)
        {
            distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(targets[i].position), new Vector2(Screen.width / 2, Screen.height / 2));
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i]) {
                index = i;
            }
        }
        return index;
    }

    public Vector3 TargetOffset(int index)
    {
        Vector3 position;
        position = targets[index].position + offset;
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
            if(camControl.CameraToggleState()) MoveTowardsTarget(lockOnTargetIndex);
            else MoveTowardsTarget(targetIndex);
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
