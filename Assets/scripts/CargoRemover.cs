using UnityEngine;

public class CargoRemover : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Cargo")) {
            Destroy(other.gameObject);
        }
    }
}
