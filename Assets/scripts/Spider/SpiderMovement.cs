using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderMovement : MonoBehaviour
{
    //Behavior
    public NavMeshAgent agent;
    private Transform player;
    private SpiderSight sight;
    private Vector3 startingPosition;

    void Awake () {
        player = GameObject.Find("RoboSamurai").transform;
        sight = GetComponent<SpiderSight>();

        startingPosition = transform.position;
    }

    void Update()
    {
        // if(sight.playerInSightRange) {
        //     LookAtPlayer();
        // }
        
        if(sight.playerInSightRange) {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }


        if(sight.playerInSightRange && !sight.playerInAttackRange)
        {
            ChasePlayer();

        } else {
            agent.SetDestination(transform.position);
        }
    }

    private void ChasePlayer()
    {
        Vector3 movePos = player.position;
        movePos = Vector3.MoveTowards(movePos, transform.position, sight.attackRange);
        
        agent.SetDestination(movePos);
    }

    private void LookAtPlayer()
    {
        Vector3 movePos = player.position;
        movePos = Vector3.MoveTowards(movePos, transform.position, sight.attackRange);
        
        transform.LookAt(new Vector3(movePos.x, transform.position.y, movePos.z));     

        // if(Vector3.Distance(player.position, transform.position) > sight.attackRange + 2) {
        //     transform.LookAt(new Vector3(movePos.x, transform.position.y, movePos.z));  
        // } else { 
        //     transform.LookAt(player.position);
        // }
    }
}
