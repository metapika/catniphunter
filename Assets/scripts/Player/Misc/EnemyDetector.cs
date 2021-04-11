using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public PlayerCombat player;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy"))
        {
            if(!player.targets.Contains(other.transform))
                player.targets.Add(other.transform);
        }
    }
        
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Enemy"))
        {
            if (player.targets.Contains(other.transform)) {
                player.targets.Remove(other.transform);
                player.AddIndex(0, player.targets.Count);
            }
        }
    }
}
