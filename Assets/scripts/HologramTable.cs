using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore.Menu;
using UnityCore.Scene;

public class HologramTable : MonoBehaviour, IInteractable
{
    public float MaxRange { get {return maxRange;  } }
    public Transform interactionPopUpPosition;
    public GameObject interactionPopUp;
    private Transform popUpReference;
    private bool usingMap;
    private const float maxRange = 100f;
    private Transform mainCamera;
    private CameraController cameraController;
    
    //Menu integrations
    private SceneController sceneControler;
    private PageController pageController;
    
    private void Start() {
        mainCamera = Camera.main.transform;
        cameraController = mainCamera.GetComponent<CameraController>();

        sceneControler = SceneController.instance;
        pageController = PageController.instance;
    }
    public void OnStartHover()
    {
        popUpReference = Instantiate(interactionPopUp, interactionPopUpPosition.position, Quaternion.identity).transform;
    }
    public void OnInteract()
    {
        if(usingMap) TurnOffMap();
        else TurnOnMap();


        Debug.Log("The player interacted with the hologram table!");
    }
    private void TurnOffMap()
    {
        usingMap = false;
        cameraController.controller.canMove = true;
        mainCamera.SetParent(cameraController.characterCenter);
        mainCamera.localPosition = new Vector3(0f, 0f, -2.5f);
        mainCamera.localEulerAngles = Vector3.zero;

        TurnMouseOff();
    }
    private void TurnOnMap()
    {
        usingMap = true;
        cameraController.controller.canMove = false;
        mainCamera.SetParent(null);
        mainCamera.position = new Vector3(0, 1.2f, 5);
        mainCamera.eulerAngles = new Vector3(11f, 0, 0);

        TurnMouseOn();
    }
    public void OnEndHover()
    {
        Destroy(popUpReference.gameObject);
        popUpReference = null;
    }
    private void TurnMouseOn()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void TurnMouseOff()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void LoadIntoTestZone() {
        if(pageController == null || sceneControler == null) return;

        sceneControler.Load(SceneType.TutorialLevel, (_scene) => {
                                Debug.Log("Scene [" + _scene + "] loaded from the hologram map!" );
                            }, false, PageType.Loading);
    }
}
