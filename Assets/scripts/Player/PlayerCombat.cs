using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCombat : MonoBehaviour
{
    #region Fields
    public bool canDetectEnemies = true;
    public Transform currentAbilities;
    public GameObject currentWeapon;
    public Transform weaponCase;
    public Transform weaponHand;
    public Transform weaponHand2;
    public Transform knifeThrowPoint;
    public GameObject crossair;
    public float offsetToDashedEnemy = 1.2f;
    public float minOffsetToDash = 0.2f;
    
    [Space]

    public float parryTime = 0.35f;
    public float parrySpeed = 15f;
    public ParticleSystem dodgeParticles;
    public float distortionAmount = 0.4f;
    public Volume volume;
    private LensDistortion dodgeDistortion;

    public List<Transform> targets;
    public int targetIndex;
    public int lockOnTargetIndex;
    public float lockOnSmoothness = 30f;
    public LayerMask maskForCoverCheck;
    private bool targetNotBehindCover;

    private CharacterController controller;
    private PlayerPhysics pphysics;
    private PlayerController movement;
    private Animator anim;
    private AbilityManager abilityList;
    private CameraController camControl;

    #endregion

    #region Unity Functions
    private void Awake() {
        controller = GetComponent<CharacterController>();

        movement = GetComponent<PlayerController>();
        pphysics = GetComponent<PlayerPhysics>();

        anim = GetComponent<Animator>();

        camControl = Camera.main.GetComponent<CameraController>();
        abilityList = currentAbilities.GetComponent<AbilityManager>();

        volume.profile.TryGet(out dodgeDistortion);

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
    private IEnumerator Dodge() {
        float startTime = Time.time;

        if (volume != null) {
            dodgeDistortion.intensity.value = distortionAmount;
        }

        // Invincibility
        // transform.root.gameObject.GetComponent<PlayerStats>().enabled = false;

        while(Time.time < startTime + parryTime)
        {
            movement.controller.Move(movement.moveDir * parrySpeed * Time.deltaTime);
            
            yield return null;
        }
        
        transform.root.gameObject.GetComponent<PlayerStats>().enabled = true;
        dodgeParticles.gameObject.SetActive(false);

        if (volume != null){
            dodgeDistortion.intensity.value = 0f;
        }
    }
    private void TargetDetection() {
        if(!canDetectEnemies) return;

        if(!camControl.CameraToggleState()) movement.canRotate = true;
        else if(targetNotBehindCover && camControl.CameraToggleState()) movement.canRotate = false;
        
        if (targets.Count > 0)
        {
            targetIndex = NearestTargetToCenter();
            
            // if(!camControl.CameraToggleState()) lockOnTargetIndex = NearestTargetToCenter();

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
                Vector3 lookDirection = targets[lockOnTargetIndex].transform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                Quaternion lookAt = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * lockOnSmoothness);

                transform.rotation = lookAt;
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
        Vector3 offset = new Vector3(0, .7f, 0);
        position = targets[index].position + offset;
        return Vector3.MoveTowards(position, transform.position, offsetToDashedEnemy);
    }
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
