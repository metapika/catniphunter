using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour {
    [Header("Sword")]
    [SerializeField] private GameObject cutter;
    [SerializeField] private Transform swordCase;
    [SerializeField] private Transform swordHand;
    
    [Header("Combo")]
    public int combonum;
    public float reset;
    public float resetTime;
    public GameObject cam;

    [Header("Targets in radius")]
    public List<Transform> targets;
    public int targetIndex;
    
    private CharacterController controller;
    private PlayerPhysics pphysics;
    private PlayerController player;
    private bool attacking;
    private Animator anim;
    private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");
    List<string> animList = new List<string>(new string[] {"animation1", "animation2", "animation3"});
    
    private void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        pphysics = GetComponent<PlayerPhysics>();
        
        cutter.GetComponent<BoxCollider>().enabled = false;
    }

    void Update()
    {
        //Targets
        
        if (targets.Count > 0)
        {
            targets.RemoveAll(enemy => enemy == null);
            targetIndex = NearestTargetToCenter();
        }
        
        if(pphysics.IsGrounded()) {
            //Combo
            if(Input.GetButtonDown("Fire1") && combonum < 3) {
                if(targets.Count != 0)
                    if(transform.position.y - targets[targetIndex].position.y <= 1 && transform.position.y - targets[targetIndex].position.y >= -1)
                    //if(Vector3.Distance(transform.position, targets[targetIndex].position) < )
                        MoveTowardsTarget(targets[targetIndex]);
                
                anim.SetTrigger(animList[combonum]);
                combonum++;
                reset = 0f;
            }
            if(combonum > 0) {
                reset += Time.deltaTime;
                if(reset > resetTime) {
                    anim.SetTrigger("Reset");
                    combonum = 0;
                }
            }
            if(combonum == 3) {
                resetTime = 3f;
                combonum = 0;
            } else {
                resetTime = 1f;
            }
            
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Q1_slash") || anim.GetCurrentAnimatorStateInfo(0).IsName("Q2_360") || anim.GetCurrentAnimatorStateInfo(0).IsName("Q3_air")){
                attacking = true;
            } else {
                attacking = false;
            }

            //Camera Lock on
            if(attacking) {
                EquipSword();
                LockPlayer();
            } else {
                UnequipSword();
                UnLockPlayer();
            }
        }
    }
    
    public void MoveTowardsTarget(Transform target)
    {
        if (Vector3.Distance(transform.position, target.position) > 1 && Vector3.Distance(transform.position, target.position) < 10)
        {
            anim.SetFloat(hashSpeedPercentage, 1f, 0.1f, Time.deltaTime);
            transform.DOMove(TargetOffset(), .5f);
            transform.DOLookAt(targets[targetIndex].position, .2f);
        }
    }
    
    public Vector3 TargetOffset()
    {
        Vector3 position;
        position = targets[targetIndex].position;
        return Vector3.MoveTowards(position, transform.position, 1f);
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
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            targets.Add(other.transform);
        }
    }   
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (targets.Contains(other.transform))
                targets.Remove(other.transform);
        }
    }

    private void LockPlayer() {
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
        player.canMove = false;
    }
    private void UnLockPlayer() {
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
}
