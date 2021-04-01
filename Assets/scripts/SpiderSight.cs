using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSight : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public float attackRange = 15;
    public bool playerInSightRange;
    public bool playerInAttackRange;
    public bool justSawPlayer;
    public LayerMask playerMask;
    public LayerMask envoriementMask;
    public Transform player;
    private void Start() {
        StartCoroutine("FindTargetsWithDelay", .2f);
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
                        playerInSightRange = true;
                        if(dstToTarget <= attackRange)
                            playerInAttackRange = true;
                    } else {
                        PlayerDissapeared();
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
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
