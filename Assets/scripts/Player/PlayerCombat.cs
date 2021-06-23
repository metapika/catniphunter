using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.AddressableAssets;

public class PlayerCombat : MonoBehaviour
{
    #region Fields
    public bool canDetectEnemies = true;
    public Melee currentWeapon;
    public int selectedWeapon = 0;
    public Transform weaponCase;
    public Transform handR;
    public Transform handL;
    public GameObject crossair;
    public Transform currentAbilities;
    //public static List<GameObject> weaponsToInstantiate = new List<GameObject>();
    public static List<GameObject> weaponInventory = new List<GameObject>();
    
    [Space]

    public bool canParry;
    public float parryTime = 0.35f;
    public float parrySpeed = 15f;
    public float parryTimerReset = 0.5f;
    public float fovChange = 65f;
    public float maxWallCheckDistance = 1f;
    public LayerMask obstacleMask;
    public Volume volume;
    private LensDistortion dodgeDistortion;
    public float offsetToDashedEnemy = 1.2f;
    public float minOffsetToDash = 0.2f;
    public ParticleSystem dashParticles;

    [Space]
    
    public List<Transform> targets;
    public Transform nearestTarget;
    public Transform lockOnTarget;
    public int lockIndex;
    public float lockOnSmoothness = 30f;
    public LayerMask maskForCoverCheck;
    public bool targetNotBehindCover;

    private CharacterController controller;
    private PlayerPhysics pphysics;
    private PlayerController movement;
    private Animator anim;
    private AbilityManager abilityList;
    private Camera mainCam;
    [HideInInspector] public CameraController camControl;
    [HideInInspector] public EnemyDetector enemyDetector;

    #endregion

    #region Unity Functions
    private void Awake() {
        controller = GetComponent<CharacterController>();

        movement = GetComponent<PlayerController>();
        pphysics = GetComponent<PlayerPhysics>();

        anim = GetComponent<Animator>();

        mainCam = Camera.main;

        camControl = mainCam.GetComponent<CameraController>();
        abilityList = currentAbilities.GetComponent<AbilityManager>();

        volume.profile.TryGet(out dodgeDistortion);

        if(PlayerCombat.weaponInventory.Count != 0)
        {
            //Object prefab = AssetDatabase.LoadAssetAtPath(aquiredWeapons[i], typeof(GameObject));
            Debug.Log(weaponInventory.Count);

            foreach (GameObject weapon in weaponInventory)
            {
                GameObject instantiatedWeapon = Instantiate(weapon, handR.position, handR.rotation, handR);

                PositionSelectedWeapon(instantiatedWeapon.GetComponent<Melee>());
            }

        }
        RefreshWeaponList();

        SelectWeapon();

        if(currentWeapon != null) {
            if(currentWeapon.weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
                currentWeapon.sword1.GetComponent<BoxCollider>().enabled = false;
                currentWeapon.sword2.GetComponent<BoxCollider>().enabled = false;
            } else {
                currentWeapon.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
    private void Update() {
        
        if(camControl.CameraToggleState()) TargetLockOnDetection();
        else TargetDetection();

        LockOnIndexing();

        if(Time.timeScale <= 0) return;

        if(Input.GetKeyDown(KeyCode.R) && targetNotBehindCover && targets.Count > 0 || Input.GetKeyDown(KeyCode.R) && targets.Count > 0 && camControl.CameraToggleState())
        {
            camControl.ToggleCameraLockOn();
        }
        
        //if(camControl.CameraToggleState()) {
            // if(Input.GetKeyDown(KeyCode.LeftShift))
            // {
            //     DodgeInput();
            // }
        //}

        if(currentWeapon && handR.childCount > 1) {
            if(currentWeapon.canAttack) {
                int previousSelectedWeapon = selectedWeapon;

                if(Input.GetKeyDown(KeyCode.Alpha1)) {
                    selectedWeapon = 0;
                } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
                    selectedWeapon = 1;
                }

                if(previousSelectedWeapon != selectedWeapon)
                {
                    SelectWeapon();
                }
            }
        }
    }
    public void DodgeInput()
    {
        if(parryTimerCoroutine == null) 
        {
            parryTimerCoroutine =  StartCoroutine(ButtonClicked(parryTimerReset));
        } else {
            StopCoroutine(parryTimerCoroutine);
            parryTimerCoroutine =  StartCoroutine(ButtonClicked(parryTimerReset));
        }
    }
    private void RefreshWeaponList()
    {
        PlayerCombat.weaponInventory.Clear();

        foreach (Transform weapon in handR)
        {
            weaponInventory.Add(weapon.GetComponent<Melee>().weaponDefinition.prefab);
        }
    }
    private void SelectWeapon()
    {
        int i = 0;

        foreach (Transform weapon in handR)
        {
            if(i == selectedWeapon) {
                EquipWeapon(weapon.gameObject, i);
            }
            else
                UnEquipWeapon(weapon.gameObject);
            i++;
        }
    }
    Coroutine parryTimerCoroutine;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("WeaponPad")) {
            WeaponPad pad = other.GetComponent<WeaponPad>();

            if(pad.activeWeapon != null) {
                if(pad.activeWeapon.CompareTag("Weapon") || pad.activeWeapon.CompareTag("Melee") ) {
                    PickUpWeapon(pad.activeWeapon);
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
    private void PickUpWeapon(GameObject weapon) {
        if(currentWeapon) UnEquipWeapon(currentWeapon.gameObject);
        EquipWeapon(weapon, handR.childCount);

        PositionSelectedWeapon(currentWeapon);

        currentWeapon.InitializeEquip();
        //currentWeapon.EquipSword();

        RefreshWeaponList();
    }
    private void PositionSelectedWeapon(Melee weapon)
    {
        if(weapon.weaponDefinition.weaponType == Weapon_SO.WeaponType.Katana) {
            weapon.transform.SetParent(handR);
            weapon.transform.position = handR.position;
            weapon.transform.rotation = handR.rotation;
        }
        else if(weapon.weaponDefinition.weaponType == Weapon_SO.WeaponType.Knife){
            weapon.transform.SetParent(handR);
            weapon.transform.localPosition = new Vector3(0.0102468356f,-0.0182343591f,-0.0692017823f);
            weapon.transform.localEulerAngles = new Vector3(38.4835701f,91.0783463f,249.838348f);
        }  
    }
    private void EquipWeapon(GameObject weapon, int index)
    {
        weapon.SetActive(true);
        currentWeapon = weapon.GetComponent<Melee>();
        selectedWeapon = index;
    }
    private void UnEquipWeapon(GameObject weapon)
    {
        weapon.gameObject.SetActive(false);
        
        //weapon.GetComponent<Melee>().UnequipSword();
    }
    
    #endregion

    #region Custom Functions
    private void LockOnIndexing()
    {
        if(camControl.CameraToggleState() && targets.Count > 0) {
        
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
    bool waiting;
    float cameraFov;
    public void Stop(float duration)
    {
        if(waiting) return;

        cameraFov = mainCam.fieldOfView;
        mainCam.fieldOfView = fovChange;
        Time.timeScale = 0.0f;
        StartCoroutine(Wait(duration));
    }
    private IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        mainCam.fieldOfView = cameraFov;
        Time.timeScale = 1.0f;
        waiting = false;
    }
    private IEnumerator ButtonClicked(float time)
    {
        canParry = true;

        yield return new WaitForSeconds(time);

        canParry = false;

    }
    public IEnumerator Dodge() {
        float startTime = Time.time;
        Vector3 direction = transform.right;
        
        dashParticles.Play();
        Stop(0.2f);
        // if (volume != null) {
        //     dodgeDistortion.intensity.value = distortionAmount;
        // }
        
        // Invincibility
        // transform.root.gameObject.GetComponent<PlayerStats>().enabled = false;

        if(!Physics.Raycast(transform.position, transform.right, maxWallCheckDistance, obstacleMask)) {
            direction = transform.right;
            anim.SetBool("dodgeRight", true);
        } else if(!Physics.Raycast(transform.position, -transform.right, maxWallCheckDistance, obstacleMask)) {
            direction = -transform.right;
            anim.SetBool("dodgeLeft", true);
        }

        while(Time.time < startTime + parryTime)
        {
            movement.controller.Move(direction * parrySpeed * Time.deltaTime);
            
            yield return null;
        }

        anim.SetBool("dodgeRight", false);
        anim.SetBool("dodgeLeft", false);

        // if (volume != null){
        //     dodgeDistortion.intensity.value = 0f;
        // }
        dashParticles.Stop();
    }

    private void TargetDetection() {
        if(!canDetectEnemies) return;

        movement.canRotate = true;
        
        if (targets.Count > 0)
        {
            nearestTarget = targets[NearestTargetToCenter()];
            lockOnTarget = targets[NearestTargetToCenter()];

            RaycastHit hit;
            Vector3 direction = Vector3.zero;

            direction = nearestTarget.position - transform.position;
            Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, nearestTarget.position), maskForCoverCheck);

            if(hit.transform != null)
            {
                if(hit.transform.gameObject.CompareTag("Enemy")) targetNotBehindCover = true;
                else targetNotBehindCover = false;
            }
        }
    }
    public void TargetLockOnDetection()
    {
        if(!canDetectEnemies) return;

        movement.canRotate = false;

        if (targets.Count > 0 && lockOnTarget != null) {  
            nearestTarget = targets[NearestTargetToCenter()];
                
            RaycastHit hit;
            Vector3 direction = Vector3.zero;

            direction = lockOnTarget.position - transform.position;
            Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, lockOnTarget.position), maskForCoverCheck);
            
            if(hit.transform != null)
            {
                if(hit.transform.gameObject.CompareTag("Enemy")) targetNotBehindCover = true;
                else targetNotBehindCover = false;
            }

            Vector3 lookDirection = lockOnTarget.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            Quaternion lookAt = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * lockOnSmoothness);

            transform.rotation = new Quaternion(transform.rotation.x, lookAt.y, transform.rotation.z, lookAt.w);
        } else {
            camControl.ToggleCameraLockOn();
        }
    }
    public void MoveTowardsTarget(Transform target = null)
    {
        if (Vector3.Distance(transform.position, target.position) > minOffsetToDash && Vector3.Distance(transform.position, target.position) < 10)
        {
            if(currentWeapon.weaponDefinition.weaponType != Weapon_SO.WeaponType.Knife) {
                if(camControl.CameraToggleState()) transform.DOMove(TargetOffset(lockOnTarget), .3f);
                else transform.DOMove(TargetOffset(nearestTarget), .3f);

                //if(transform.position.y - targets[index].position.y < 3 && transform.position.y - targets[index].position.y > -3)
                transform.DOLookAt(new Vector3(nearestTarget.position.x, transform.position.y, nearestTarget.position.z), .2f);
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
    public Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        Vector3 offset = new Vector3(0, .7f, 0);
        position = target.position + offset;
        return Vector3.MoveTowards(position, transform.position, offsetToDashedEnemy);
    }
    public void AddIndex(int amount, int max) {
        if ((lockIndex += amount) >= max) {
            lockIndex -= max;
            lockOnTarget = targets[lockIndex];
            return;
        }

        if (lockIndex >= 0) {
            lockOnTarget = targets[lockIndex];
            return;
        }

        lockIndex = max + lockIndex;
        lockOnTarget = targets[lockIndex];
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
        if(currentWeapon.weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
            currentWeapon.sword1.GetComponent<BoxCollider>().enabled = true;
            currentWeapon.sword2.GetComponent<BoxCollider>().enabled = true;
        } else {
            currentWeapon.GetComponent<BoxCollider>().enabled = true;
        }
        
    }
    public void DisableCutter()
    {
        if(currentWeapon.weaponDefinition.weaponType == Weapon_SO.WeaponType.DoubleSwords) {
            currentWeapon.sword1.GetComponent<BoxCollider>().enabled = false;
            currentWeapon.sword2.GetComponent<BoxCollider>().enabled = false;
        } else {
            currentWeapon.GetComponent<BoxCollider>().enabled = false;
        }
    }
    public void StartCombo() {
        if(targets.Count > 0)
        {
            if(targetNotBehindCover) {
                if(camControl.CameraToggleState()) MoveTowardsTarget(lockOnTarget);
                else MoveTowardsTarget(nearestTarget);
            }
        }

        currentWeapon.canAttack = false;
        movement.canMove = false;
    }
    public void StopCombo() {
        currentWeapon.canAttack = true;
        movement.canMove = true;
    }
    public void EnableTrail()
    {
        if(currentWeapon.weaponDefinition.weaponType == Weapon_SO.WeaponType.Katana)
        {
            currentWeapon.trail.Play();
        }
    }
    public void DisableTrail()
    {
        if(currentWeapon.weaponDefinition.weaponType == Weapon_SO.WeaponType.Katana) {
            currentWeapon.trail.Stop();
        }
    }
    #endregion
}
