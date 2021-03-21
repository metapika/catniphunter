using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    public int damage = 10;
    public float timeBeforeReseting = 4f;
    public float restTime = 3f;
    public bool triggerEnabled;
    public ParticleSystem particles;
    public List<PlayerStats> targets;
    private void Start() {
        StartCoroutine(Cycle());

        InvokeRepeating("Dps", 1, 0.3f);
    }
    private void Update() {
        if(triggerEnabled) {
            GetComponent<BoxCollider>().enabled = true;
            particles.Play();
        } else {
            GetComponent<BoxCollider>().enabled = false;
            particles.Stop();
        }
    }
    private IEnumerator Cycle() {
        triggerEnabled = true;

        yield return new WaitForSeconds(timeBeforeReseting);

        triggerEnabled = false;

        yield return new WaitForSeconds(restTime);

        StartCoroutine(RestartCycle());
    }
    private IEnumerator RestartCycle() {
        StartCoroutine(Cycle());

        yield return null;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<PlayerStats>())
        {
            if(!targets.Contains(other.gameObject.GetComponent<PlayerStats>()))
                targets.Add(other.gameObject.GetComponent<PlayerStats>());
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<PlayerStats>())
        {
            if (targets.Contains(other.gameObject.GetComponent<PlayerStats>()))
                targets.Remove(other.gameObject.GetComponent<PlayerStats>());
        }
    }
    void Dps() {
        if(targets.Count != 0) {
            foreach(PlayerStats health in targets) {
                if(health.gameObject.CompareTag("Player")) {
                    health.TakeDamage(damage);
                    health.gameObject.GetComponent<Animator>().SetTrigger("hit");
                }    
            }
        }
    }
}
