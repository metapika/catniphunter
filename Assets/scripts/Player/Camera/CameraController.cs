using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    #region Fields
    public Transform targetIndicator;
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private bool clampView = true;
    [SerializeField] private bool lockOnTarget = true;

    [Space]

    [SerializeField] private float sensitivityX = 10;
    [SerializeField] private float sensitivityY = 10;
    public Transform characterCenter;
    [SerializeField] private float lockOnVignette = 0.4f;
    [SerializeField] private float vignetteTime = 1f;
    [SerializeField] private LayerMask envoriementMask;
    public Volume postProcessVolume;
    private float defaultVignette;
    private Vignette cameraVignette;

    private PlayerCombat playerCombat;
    [HideInInspector] public PlayerController controller;

    private float mouseX, mouseY;

    #endregion

    #region Unity Functions
    private void Start() {
        playerCombat = transform.root.GetComponent<PlayerCombat>();
        controller = transform.root.GetComponent<PlayerController>();

        postProcessVolume.profile.TryGet(out cameraVignette);

        if(!lockCursor) return;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate() {
        if(Time.timeScale > 0) {
            CameraControl();
            if(playerCombat) if(playerCombat.targets.Count > 0) {
                if(CameraToggleState()) {CameraLookAt(playerCombat.lockOnTarget);}
                else {CameraLookAt(characterCenter);}
            }
        }
    }

    #endregion

    private void CameraControl() {
        mouseX += Input.GetAxis("Mouse X") * sensitivityX / 10;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivityY / 10;
        
        if(clampView) {
            mouseY = Mathf.Clamp(mouseY, -25, 60);
        }
        if(!lockOnTarget) {
            characterCenter.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
    }
    public void ToggleCameraLockOn()
    {
        if(lockOnTarget) { LockOnOff(); }
        else { HandleCameraLockOn(); }
    }
    private void HandleCameraLockOn()
    {
        if(playerCombat.targets.Count > 0) {
                playerCombat.lockOnTarget = playerCombat.nearestTarget;
                Transform target = playerCombat.lockOnTarget;
                Vector3 dirToTarget = (target.position - transform.root.position).normalized;
                if(Vector3.Angle(transform.root.forward, dirToTarget) < 360 / 2) { //Add a viewing angle
                    float dstToTarget = Vector3.Distance(transform.root.position, target.position);

                    if(!Physics.Raycast(transform.root.position, dirToTarget, dstToTarget, envoriementMask)) 
                    {
                        // characterCenter.eulerAngles = Vector3.zero;
                        // transform.eulerAngles = Vector3.zero;
                        transform.localPosition = new Vector3(1, transform.localPosition.y, transform.localPosition.z);
                        StartCoroutine(Lerp(vignetteTime, cameraVignette.intensity.value, cameraVignette.intensity.value, lockOnVignette));
                        lockOnTarget = true;

                    } else {
                        LockOnOff();
                    }
                }
        } else {
            LockOnOff();
        }
    }

    private void LockOnOff()
    {
        lockOnTarget = false;
        transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        // transform.localEulerAngles = Vector3.zero;
        
        StartCoroutine(Lerp(vignetteTime, cameraVignette.intensity.value, cameraVignette.intensity.value, defaultVignette));
    }
    private void CameraLookAt(Transform target){
        if(!lockOnTarget) {
            //Normal camera
            transform.LookAt(characterCenter);
            return;
        }

        Vector3 direction = playerCombat.lockOnTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion lookAt = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * playerCombat.lockOnSmoothness);

        characterCenter.rotation = lookAt;


        targetIndicator.position = target.position + new Vector3(0, 1.5f, 0);
    }
    private IEnumerator Lerp(float time, float value, float startValue, float targetValue)
    {
        float start = Time.time;

        while (Time.time < start + time)
        {
            float completion = (Time.time - start) / time;
            cameraVignette.intensity.value = Mathf.Lerp(startValue, targetValue, completion);
            yield return null;
        }

        cameraVignette.intensity.value = targetValue;
    }

    public bool CameraToggleState()
    {
        return lockOnTarget;
    }
}
