using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public List<Transform> enemies;
    public Transform gate;
    private Vector3 gateEndPos;

    private void Start() {
        gateEndPos = new Vector3(3.04857349f, -4.62388992f, 81.5105286f);
    }

    // Update is called once per frame
    void Update()
    {
        if(enemies.Count <= 0)
            OpenGate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemies.Add(other.transform);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemies.Contains(other.transform))
                enemies.Remove(other.transform);
        }
    }

    private void OpenGate() {
        gate.position = Vector3.Slerp(gate.position, gateEndPos, 0.2f);
    }
}
