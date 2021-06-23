using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [Header("--------------------------Universal--------------------------")]
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public float attackRange = 15;
    public bool playerInSightRange;
    public bool playerInAttackRange;
    public bool justSawPlayer;
    public GameObject playerSpottedIndicator;
    public GameObject playerNotInSightIndicator;
    public LayerMask playerMask;
    public LayerMask envoriementMask;
    public LayerMask enemyMask;
    public Transform player;
    private void Start() {
        StartCoroutine("FindTargetsWithDelay", .2f);

        playerSpottedIndicator.SetActive(false);
        playerNotInSightIndicator.SetActive(false);
    }
    void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if(targetsInViewRadius.Length > 0) {
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);

                    if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, envoriementMask)) {
                        justSawPlayer = true;
                        player = target;
                        AlertOthers();
                        playerInSightRange = true;
                        if(dstToTarget <= attackRange)
                            playerInAttackRange = true;
                        ShowPlayerSpottedIndicator();
                    } else {
                        PlayerDissapeared();
                        if(playerSpottedIndicator && playerNotInSightIndicator)
                        {
                            playerSpottedIndicator.SetActive(false);
                            playerNotInSightIndicator.SetActive(true);
                        }
                    }
                }
            }
        } else {
            PlayerDissapeared();
        }
    }
    private void PlayerDissapeared()
    {
        playerInSightRange = false;
        playerInAttackRange = false;
    }
    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true) {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    public void AlertOthers()
    {
        Collider[] foundEnemies = Physics.OverlapSphere(transform.position, 35, enemyMask);
        foreach (Collider enemy in foundEnemies)
        {
            EnemySight othersSight = enemy.GetComponent<EnemySight>();

            if(othersSight) {
                othersSight.player = player;
                othersSight.justSawPlayer = true;
                othersSight.ShowPlayerSpottedIndicator();
            }
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    public void ShowPlayerSpottedIndicator()
    {
        if(playerNotInSightIndicator && playerSpottedIndicator)
        {
            playerNotInSightIndicator.SetActive(false);
            playerSpottedIndicator.SetActive(true);
        }
    }
}
