using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState
{
    Attacking,
    Patrolling
}
public class EnemyMovement : MonoBehaviour
{
    [Header("--------------------------Patroling--------------------------")]
    public PatrolPoints patrolPointsRef;
    public int patrolPointIndex = 0;
    private NavMeshAgent agent;
    private EnemySight sight;
    private EnemyStats stats;

    void Awake () {
        sight = GetComponent<EnemySight>();
        stats = GetComponent<EnemyStats>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(sight.justSawPlayer) {
            if(sight.player) transform.LookAt(new Vector3(sight.player.position.x, transform.position.y, sight.player.position.z));
        }

        if(!stats.gettingKnockbacked) {
            if(sight.justSawPlayer && !sight.playerInSightRange)
            {
                ChasePlayer();
            } else {
                if(gameObject.activeSelf && agent.enabled) agent.SetDestination(transform.position);
            }
        }
    }

    public void AddIndex(int amount, int max) {
        if ((patrolPointIndex += amount) >= max) {
            patrolPointIndex -= max;
            return;
        }

        if (patrolPointIndex >= 0) {
            return;
        }

        patrolPointIndex = max + patrolPointIndex;
    }
    private void ChasePlayer()
    {
        if(!sight.player) return;

        Vector3 movePos = Vector3.Normalize(sight.player.position - transform.position);

        if(sight.playerInSightRange) {
            movePos = Vector3.MoveTowards(transform.position, movePos, sight.attackRange);
            if(gameObject.activeSelf && agent.enabled) agent.SetDestination(movePos);
        } else {
            if(gameObject.activeSelf && agent.enabled) agent.SetDestination(sight.player.position);
        }
    }
}