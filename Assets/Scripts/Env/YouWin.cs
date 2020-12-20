using UnityEngine;

public class YouWin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            Destroy(other.gameObject);
        }
    }
}
