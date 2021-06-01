using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public PlayerCombat player;
    public float maxDetectionHeight = 1f;
    public float minDetectionHeight = 2f;
    private void Start() {
        player.enemyDetector = this;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy"))
        {
            AddEnemy(other.transform);
        }
    }   
    public void AddEnemy(Transform other)
    {
        // if(transform.position.y - other.transform.position.y > maxDetectionHeight) return;

        // else if(transform.position.y - other.transform.position.y < -minDetectionHeight) return;

        if(!player.targets.Contains(other)) {
            int enemyID = 0;
            if(player.targets.Count > 0) enemyID = player.lockOnTarget.GetInstanceID();
            
            player.targets.Add(other);

            if(player.targets.Count > 0) {

                SortTargetsList();

                for (int i = 0; i < player.targets.Count; i++)
                {
                    if(player.targets[i].GetInstanceID() == enemyID) {

                        player.lockOnTarget = player.targets[i];
                        break;
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Enemy"))
        {
            RemoveEnemy(other.transform);
        }
    }
    public void RemoveEnemy(Transform other, bool enemyDead = false)
    {
        if(player.targets.Contains(other)) {
            int enemyID = 0;
            if(player.targets.Count > 0) enemyID = player.lockOnTarget.GetInstanceID();
            
            player.targets.Remove(other);

            if(player.targets.Count > 0) {
                if(enemyDead) {
                    player.AddIndex(0, player.targets.Count);
                    return;
                }

                SortTargetsList();

                for (int i = 0; i < player.targets.Count; i++)
                {
                    if(player.targets[i].GetInstanceID() == enemyID) {

                        player.lockOnTarget = player.targets[i];
                        player.lockIndex = i;
                        break;
                    }
                }
            } else {
                player.lockOnTarget = null;
                player.nearestTarget = null;
            }
        }
    }
    private void SortTargetsList()
    {
        player.targets.Sort(SortFunc);
    }
    private int SortFunc(Transform a, Transform b)
    {
        var relativePointA = transform.InverseTransformPoint(a.position);
        var relativePointB = transform.InverseTransformPoint(b.position);

        if(relativePointA.x < relativePointB.x)
        {
            return -1;
        } 
        else if(relativePointA.x > relativePointB.x)
        {
            return 1;
        }
        return 0;
    }
}
