using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderShoot : MonoBehaviour
{
    private SpiderSight sight;
    private Transform player;
    public GameObject projectile;
    public Transform gunPoint, gunPoint2;
    public Transform gun, gun2;
    public int magazineSize;
    public float timeBetweenAttacks, reloadTime;
    bool alreadyAttacked, reloading;
    int bulletsShot;
    void Start()
    {
        player = GameObject.Find("RoboSamurai").transform;
        sight = GetComponent<SpiderSight>();
    }

    void Update()
    {
        // if(!reloading && bulletsShot == magazineSize)
        //     StartCoroutine(Reload());
        
        if(sight.playerInAttackRange) AttackPlayer();
    }

    private void AttackPlayer()
    {
        Vector3 playerPos = player.position + new Vector3(0f, 0.7f, 0f);
        gun.LookAt(playerPos);
        gun2.LookAt(playerPos);
        gunPoint.LookAt(playerPos);
        gunPoint2.LookAt(playerPos);
    }

    public void Shoot()
    {

    }
}
