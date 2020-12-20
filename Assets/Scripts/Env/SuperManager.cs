using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperManager : MonoBehaviour
{
    public GameObject controlPanel;
    public GameObject deathPanel;
    public bool lockCursor;

    private void Start() {
        deathPanel.SetActive(false);
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.I)) {
            if(controlPanel.activeSelf == true) {
                controlPanel.SetActive(false);
            } else {
                controlPanel.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if(GameObject.Find("RoboSamurai") == null) {
            lockCursor = false;
            deathPanel.SetActive(true);
        }

        //Lock cursor
        if(lockCursor == true)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            Destroy(other.gameObject);
        }
    }

    public void Restart() {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
