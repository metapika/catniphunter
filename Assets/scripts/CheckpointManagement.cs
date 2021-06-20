using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManagement : MonoBehaviour
{
    public static int checkpointIndex = 0;
    public FadeFromBlack fadeFromBlack;
    public List<Vector3> checkpointPositions;
    [SerializeField] private Transform player;
    private void Start() {
        checkpointPositions.Add(player.position);

        foreach (Transform checkpoint in transform)
        {
            checkpointPositions.Add(checkpoint.position);
        }

        player.position = checkpointPositions[checkpointIndex];    
    }
    public void SoftRespawn()
    {
        fadeFromBlack.FadeFromColor(Color.white);
        player.position = checkpointPositions[checkpointIndex];
    }
}
