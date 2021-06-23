using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private CheckpointManagement manager;
    private void Start() {
        manager = transform.root.GetComponent<CheckpointManagement>();
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player"))
        {
            for (int i = 0; i < manager.checkpointPositions.Count; i++)
            {
                if(transform.position == manager.checkpointPositions[i])
                {
                    CheckpointManagement.checkpointIndex = i;
                    //other.GetComponent<PlayerStats>().ApplyHealth(1000);
                }
            }
        }
    }
}
