using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum SpiderState
{
    Attacking,
    Patrolling
}
public class SpiderMovement : MonoBehaviour
{
    //Behavior
    public PatrolPoints patrolPointsRef;
    public int patrolPointIndex = 0;
    public float changePatrolPointDistance = 0.2f;
    public SpiderState state = SpiderState.Attacking;
    private NavMeshAgent agent;
    private SpiderSight sight;
    private SpiderStats stats;
    private Vector3 startingPosition;

    void Awake () {
        sight = GetComponent<SpiderSight>();
        stats = GetComponent<SpiderStats>();
        agent = GetComponent<NavMeshAgent>();

        startingPosition = transform.position;

        if(state == SpiderState.Patrolling)
            ChooseNextPatrolPoint();
    }

    private void Update()
    {
        if(state == SpiderState.Patrolling)
        {
            Debug.Log(Vector3.Distance(transform.position, patrolPointsRef.patrolPoints[patrolPointIndex].position));
            if(Vector3.Distance(transform.position, patrolPointsRef.patrolPoints[patrolPointIndex].position) <  changePatrolPointDistance) {
                ChooseNextPatrolPoint();
            }
            
            agent.SetDestination(patrolPointsRef.patrolPoints[patrolPointIndex].position);
        } else {
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
    }

    private void ChooseNextPatrolPoint()
    {
        AddIndex(Random.Range(0, patrolPointsRef.patrolPoints.Count + 1), patrolPointsRef.patrolPoints.Count);
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
        Vector3 movePos = Vector3.Normalize(sight.player.position - transform.position);

        if(sight.playerInSightRange) {
            movePos = Vector3.MoveTowards(transform.position, movePos, sight.attackRange);
            agent.SetDestination(movePos);
        } else {
            agent.SetDestination(sight.player.position);
        }
    }
}