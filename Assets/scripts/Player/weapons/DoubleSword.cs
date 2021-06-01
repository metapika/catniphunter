
using UnityEngine;

public class DoubleSword : MonoBehaviour
{
    [HideInInspector] public Weapon_SO weaponDefinition;
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Enemy")) {
            if(other.name != "EnemyTest") {
                other.GetComponent<EnemyStats>().TakeDamage(weaponDefinition.damage);
            }
        }
    }
}
