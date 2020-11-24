using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{   
    private Animator anim = null;
    
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private int attackDamage = 25;

    private void Start() {
        anim = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        //Play attack animation
        anim.SetTrigger("Attack");

        //Detect enemies
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        //Damage them
        foreach(Collider enemy in hitEnemies)
        {
            Debug.Log("Hit enemy! ");
            //enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
