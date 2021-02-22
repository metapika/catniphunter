using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class SpiderSight : MonoBehaviour
{
    public LayerMask whatIsGround, whatIsPlayer;
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

        if(player != null) {
            RaycastHit hit;
            Vector3 direction = player.transform.position - transform.position;     
            
            if(Physics.Raycast(transform.position, direction, out hit, Vector3.Distance(transform.position, player.position) + 5f))
            {
                if(hit.transform.gameObject.CompareTag("Player")) 
                {
                    if(!justSawPlayer && playerInSightRange)
                    {
                        justSawPlayer = true;
                    }

                    playerNotBehindCover = true;
                }
                else {
                    playerNotBehindCover = false;
                }
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
