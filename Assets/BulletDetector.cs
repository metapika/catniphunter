using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    public PlayerCombat player;
    public bool bulletInsideTrigger;
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Bullet"))
        {
            bulletInsideTrigger = true;
        } else {
            bulletInsideTrigger = false;
        }
    }
}
