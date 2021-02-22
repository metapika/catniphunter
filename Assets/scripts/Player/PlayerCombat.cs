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
    [SerializeField] private ParticleSystem swordParticles;
    public PlayerStats stats;
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
    private Animator anim;
    private ShieldAbility shield;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");

    private bool electrified;
    public float electrifiedTime = 2f;
    
    private void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        pphysics = GetComponent<PlayerPhysics>();

        if(transform.Find("CurrentAbilities").transform.Find("ShieldAbility") != null) {
            shield = transform.Find("CurrentAbilities").transform.Find("ShieldAbility").GetComponent<ShieldAbility>();
        }
        
        cutter.GetComponent<BoxCollider>().enabled = false;
        swordParticles.Stop();
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
        
        //Combo & charged attack
        if(Input.GetButton("Fire1")) {
            EquipSword();

            if(pphysics.IsGrounded()) {
                if(shield != null && !shield.blocking) {
                    Combo();
                } else {
                    Combo();
                }
            }
        }
        
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
        if(other.gameObject.CompareTag("Electric")) {
            if(!electrified) {
                stats.TakeDamage(1);
                StartCoroutine(Electrified());
            }
        }
    }

    private IEnumerator Electrified() {
        electrified = true;
        anim.SetBool("electrified", true);
        player.canMove = false;

        yield return new WaitForSeconds(electrifiedTime);

        electrified = false;
        anim.SetBool("electrified", false);
        player.canMove = true;
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
        swordParticles.Play();
        combo = true;
    }

    public void StopCombo() {
        swordParticles.Stop();
        combo = false;
    }
}
