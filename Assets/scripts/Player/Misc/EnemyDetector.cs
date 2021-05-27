using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public PlayerCombat player;
    public float maxDetectionHeight = 1f;
    public float minDetectionHeight = 2f;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy"))
        {
            AddEnemy(other);
        }
    }   
    public void AddEnemy(Collider other)
    {
        if(!player.targets.Contains(other.transform)) {
            player.targets.Add(other.transform);

            string currentTargetName = player.targets[player.lockOnTargetIndex].name;

            SortTargetsList();

            for(int i = 0; i < player.targets.Count; i++){
                if(player.targets[i].name == currentTargetName) {
                    player.lockOnTargetIndex = i;
                    break;
                }
            }
            if(player.lockOnTargetIndex < 0 || player.lockOnTargetIndex > player.targets.Count) {
                player.AddIndex(0, player.targets.Count);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Enemy"))
        {
            RemoveEnemy(other);
        }
    }
    public void RemoveEnemy(Collider other)
    {
        if(player.targets.Contains(other.transform)) {

            string currentTargetName = player.targets[player.lockOnTargetIndex].name;

            SortTargetsList();

            for(int i = 0; i < player.targets.Count; i++){
                if(player.targets[i].name == currentTargetName) {
                    player.lockOnTargetIndex = i;
                    break;
                }
            }

            player.targets.Remove(other.transform);
            
            if(player.lockOnTargetIndex < 0 || player.lockOnTargetIndex > player.targets.Count) {
                player.AddIndex(0, player.targets.Count);
            }
        }
    }
    // if(transform.position.y - other.transform.position.y > maxDetectionHeight) return;

    // else if(transform.position.y - other.transform.position.y < -minDetectionHeight) return;
    private void SortTargetsList()
    {
        player.targets.Sort(SortFunc);
    }
    private int SortFunc(Transform a, Transform b)
    {
        if(a.position.x < b.position.x)
        {
            return -1;
        } 
        else if(a.position.x > b.position.x)
        {
            return 1;
        }
        return 0;
    }
}
