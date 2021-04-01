using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderMovement : MonoBehaviour
{
    //Behavior
    private NavMeshAgent agent;
    private SpiderSight sight;
    private SpiderStats stats;
    private Vector3 startingPosition;

    void Awake () {
        sight = GetComponent<SpiderSight>();
        stats = GetComponent<SpiderStats>();
        agent = GetComponent<NavMeshAgent>();

        startingPosition = transform.position;
    }

    void Update()
    {
        if(sight.justSawPlayer) {
            transform.LookAt(new Vector3(sight.player.position.x, transform.position.y, sight.player.position.z));
        }

        if(!stats.gettingKnockbacked) {
            if(sight.justSawPlayer && !sight.playerInSightRange)
            {
                ChasePlayer();
            } else {
                agent.isStopped = true;
            }
        }
    }

    private void ChasePlayer()
    {
        Vector3 movePos = Vector3.Normalize(sight.player.position - transform.position);

        if(sight.playerInSightRange) {
            movePos = Vector3.MoveTowards(transform.position, movePos, sight.attackRange);
            agent.SetDestination(movePos);
        } else {
            agent.SetDestination(sight.player.position);
        }
    }
}
