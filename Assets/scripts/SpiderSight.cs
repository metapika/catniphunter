using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpiderSight : MonoBehaviour
{
    public LayerMask whatIsPlayer;
    public LayerMask coverDetectionMask;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, playerNotBehindCover, justSawPlayer;
    private Transform player;

    private void Start() {
        player = GameObject.Find("RoboSamurai").transform;
    }
    private void Update() {
        DetectPlayer();
    }

    private void DetectPlayer() {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        Vector3 direction = player.transform.position - transform.position;     
        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, player.position) + 5f, coverDetectionMask))
        {
            if(hit.transform.gameObject.CompareTag("Player") || hit.transform.gameObject.CompareTag("Shield")) 
            {
                playerNotBehindCover = true;
                justSawPlayer = true;
            }
            else
            {
                playerNotBehindCover = false;
            }
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
