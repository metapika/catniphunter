using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore.Scene;
using UnityCore.Menu;

public class Die : MonoBehaviour
{
    private GameObject Player;
    private GameObject GameUI;
    private SkinnedMeshRenderer playerOverall;
    private SkinnedMeshRenderer playerDetails;
    public LayerMask whatIsPlayer;
    public LayerMask everything;
    public Material deathMaterial;
    private Material defMaterialOverall;
    private Material defMaterialDetails;
    private Camera cam;
    private CameraController camControl;
    private bool dead;
    private SceneController SceneController;
    private PageController PageController;
    private PlayerStats playerHealth;

    void Start()
    {
        SceneController = SceneController.instance;
        PageController = PageController.instance;
        GameUI = GameObject.Find("In-game Canvas");
        
        Player = GameObject.Find("RoboSamurai");
        playerHealth = Player.GetComponent<PlayerStats>();
        playerOverall = Player.transform.Find("GFX").transform.Find("overall").GetComponent<SkinnedMeshRenderer>();
        defMaterialOverall = playerOverall.material;
        playerDetails = Player.transform.Find("GFX").transform.Find("details").GetComponent<SkinnedMeshRenderer>();
        defMaterialDetails = playerDetails.material;

        cam = Player.transform.Find("Target").transform.Find("Main Camera").GetComponent<Camera>();
        camControl = cam.GetComponent<CameraController>();
    }

    void Update()
    {
        if(SceneController != null) {
            if(playerHealth.currentHealth <= 0) {
                if(!dead) {
                    CommitDie("death");
                }
            }
        }
    }

    public void CommitDie(string animationTrigger) {
        dead = true;
        LightsGoOut();
        DisableComponents();
        Player.GetComponent<Animator>().SetTrigger(animationTrigger);
        playerOverall.material = deathMaterial;
        playerDetails.material = deathMaterial;
        StartCoroutine(ShowEndScreen());
    }

    IEnumerator ShowEndScreen() {
        yield return new WaitForSeconds(1);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PageController.TurnPageOn(PageType.DeathScreen);
    }

    void LightsGoOut() {
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.cullingMask = whatIsPlayer;
    }

    void DisableComponents() {
        camControl.enabled = false;
        Player.GetComponent<PlayerPhysics>().enabled = false;
        Player.GetComponent<PlayerController>().enabled = false;
        Player.GetComponent<PlayerCombat>().enabled = false;
        Player.transform.Find("CurrentAbilities").gameObject.SetActive(false);
        GameUI.SetActive(false);
    }
}
