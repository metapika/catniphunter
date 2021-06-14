using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    #region Fields
    public Transform targetIndicatorPrefab;
    private SpriteBillboard targetIndicatorRef;
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
    private CameraSmoothCollision cameraCollision;

    private float mouseX, mouseY;

    #endregion

    #region Unity Functions
    private void Start() {
        cameraCollision = GetComponent<CameraSmoothCollision>();
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
                        Vector3 cameraReset = new Vector3(1, transform.localPosition.y, transform.localPosition.z);
                        
                        cameraCollision.HadleLockOnCollision(new Vector3(1f, 0f, -2.5f));
                        transform.localPosition = cameraReset;

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
        Vector3 cameraReset = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        lockOnTarget = false;

        cameraCollision.HadleLockOnCollision(new Vector3(0f, 0f, -2.5f));
        transform.localPosition = cameraReset;
        
        if(targetIndicatorRef != null) Destroy(targetIndicatorRef.gameObject);

        StartCoroutine(Lerp(vignetteTime, cameraVignette.intensity.value, cameraVignette.intensity.value, defaultVignette));
    }
    private void CameraLookAt(Transform target){
        if(!lockOnTarget) {
            //Normal camera
            transform.LookAt(characterCenter);
            return;
        }

        Vector3 direction = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion lookAt = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * playerCombat.lockOnSmoothness);

        characterCenter.rotation = lookAt;

        if(targetIndicatorRef == null) targetIndicatorRef = Instantiate(targetIndicatorPrefab, target.position + new Vector3(0, 1.5f, 0), Quaternion.identity).GetComponent<SpriteBillboard>();
        targetIndicatorRef.mainCam = GetComponent<Camera>();
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
