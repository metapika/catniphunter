using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour {
    [Header("Sword")]
    [SerializeField] private GameObject cutter;
    [SerializeField] private Transform swordCase;
    [SerializeField] private Transform swordHand;
    public CharacterStats stats;
    private bool combo;

    [Header("Targets in radius")]
    public List<Transform> targets;
    public int targetIndex;
    bool targetNotBehindCover = false;

    [Header("Charged attack")]
    public float chargeTimer = 0f;
    public float chargeDestination = 2f;
    public Slider chargeSlider;
    public Image sliderFill;
    
    private CharacterController controller;
    private PlayerPhysics pphysics;
    public PlayerController player;
    public GameObject cam;
    private PlayerAbilities abilites;
    private Animator anim;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");
    
    private void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        pphysics = GetComponent<PlayerPhysics>();
        abilites = GetComponent<PlayerAbilities>();
        
        cutter.GetComponent<BoxCollider>().enabled = false;
    }

    void Update()
    {
        //Targets
        if (targets.Count > 0)
        {
            targetIndex = NearestTargetToCenter();
            
            RaycastHit hit;
            Vector3 direction = targets[targetIndex].position - transform.position;     
            Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, targets[targetIndex].position));

            if(hit.transform != null)
            {
                if(hit.transform.gameObject.CompareTag("Enemy"))
                    targetNotBehindCover = true;
                else
                    targetNotBehindCover = false;
            }
            
            if(pphysics.IsGrounded())
                if(transform.position.y - targets[targetIndex].position.y < 3 && transform.position.y - targets[targetIndex].position.y > -3)
                    if(targetNotBehindCover)
                        transform.LookAt(new Vector3(targets[targetIndex].position.x, transform.position.y, targets[targetIndex].position.z));
        }
        
        chargeSlider.value = chargeTimer;
        
        //Combo & charged attack
        if(Input.GetButton("Fire1")) {
            EquipSword();
            if(pphysics.IsGrounded())
                chargeTimer += Time.deltaTime;
        }
        
        if(Input.GetButtonUp("Fire1")) {
            if(targets.Count != 0 && pphysics.IsGrounded())
                    if(Vector3.Distance(transform.position, targets[targetIndex].position) > 4)
                        if(targetNotBehindCover)
                        MoveTowardsTarget(targets[targetIndex]);
            
            if(chargeTimer < chargeDestination) {
                chargeTimer = 0;
                if(pphysics.IsGrounded() && !abilites.blocking)
                    Combo();
            } else {
                chargeTimer = 0;
                if(pphysics.IsGrounded() && !abilites.blocking)
                    anim.SetTrigger("chargedAttack");
            }

            Color chargingColor = new Color(255f / 255f, 118f / 255f, 246f / 255f);
            Color chargedColor = new Color(254f / 255f, 45f / 255f, 189f / 255f);
            
            if(chargeTimer <= chargeDestination / 2)
                sliderFill.color = chargingColor;
            if(chargeTimer >= chargeDestination)
                sliderFill.color = chargedColor;
        }

        if(Input.GetKeyDown(KeyCode.Q))
            ThrowShurikens();
        
        if(Input.GetKeyDown(KeyCode.I)) {
            EquipSword();
        } else if (Input.GetKeyDown(KeyCode.O)) {
            UnequipSword();
        }
    }

    private void Combo() 
    {
        int attacknum = Random.Range(0, 2);
        List<string> animList = new List<string>(new string[] {"slash1", "slash2"});
        
        if(!combo) {
            anim.SetTrigger(animList[attacknum]);
        }
    }

    private void ThrowShurikens()
    {
        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        transform.LookAt(transform.position + forward);
        anim.SetTrigger("ability1");
    }
    
    public void MoveTowardsTarget(Transform target)
    {
        if (Vector3.Distance(transform.position, target.position) > 1 && Vector3.Distance(transform.position, target.position) < 10)
        {
            anim.SetFloat(hashSpeedPercentage, 1f, 0.1f, Time.deltaTime);
            transform.DOMove(TargetOffset(), .5f);
            if(pphysics.IsGrounded())
            if(transform.position.y - targets[targetIndex].position.y < 3 && transform.position.y - targets[targetIndex].position.y > -3)
                transform.DOLookAt(new Vector3(targets[targetIndex].position.x, transform.position.y, targets[targetIndex].position.z), .2f);
        }
    }
    
    public Vector3 TargetOffset()
    {
        Vector3 position;
        position = targets[targetIndex].position;
        return Vector3.MoveTowards(position, transform.position, 1.2f);
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
    
    public void SelectTarget(int index)
    {
        targetIndex = index;
        if(pphysics.IsGrounded())
            if(transform.position.y - targets[targetIndex].position.y < 3 && transform.position.y - targets[targetIndex].position.y > -3)
                transform.DOLookAt(new Vector3(targets[targetIndex].position.x, transform.position.y, targets[targetIndex].position.z), .3f).SetUpdate(true);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fist")) {
            stats.TakeDamage(other.transform.root.GetComponent<CharacterStats>().characterDefinition.baseDamage);
            anim.SetTrigger("hit");
        }
        
        if(other.gameObject.CompareTag("Bullet")) {
            if(other.gameObject.GetComponent<Missile>()) {
                stats.TakeDamage(other.GetComponent<Missile>().bulletDamage);
                other.gameObject.GetComponent<Missile>().DestroyMissile();
            }
            else {
                stats.TakeDamage(other.GetComponent<BulletBase>().bulletDamage);
                Destroy(other.gameObject);
            }
            anim.SetTrigger("hit");
        }
    }  

    public void EnableCutter()
    {
        cutter.GetComponent<BoxCollider>().enabled = true;
    }
    
    public void DisableCutter()
    {
        cutter.GetComponent<BoxCollider>().enabled = false;
    }

    public void UnequipSword() {
        cutter.transform.SetParent(swordCase);
        cutter.transform.position = swordCase.position;
        cutter.transform.rotation = swordCase.rotation;
    }
    
    public void EquipSword() {
        cutter.transform.SetParent(swordHand);
        cutter.transform.position = swordHand.position;
        cutter.transform.rotation = swordHand.rotation;
    }

    public void StartCombo() {
        combo = true;
    }

    public void StopCombo() {
        combo = false;
    }
}
