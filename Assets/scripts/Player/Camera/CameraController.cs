using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Fields
    public Transform targetIndicator;
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private bool clampView = true;
    [SerializeField] private bool lockOnTarget = true;

    [Space]

    public float maxLockOnDistance = 12;
    [SerializeField] private float sensitivityX = 10;
    [SerializeField] private float sensitivityY = 10;
    [SerializeField] private Transform characterCenter;

    private PlayerCombat playerCombat;
    private float mouseX, mouseY;

    #endregion

    #region Unity Functions
    private void Awake() {
        playerCombat = transform.root.GetComponent<PlayerCombat>();

        if(!lockCursor) return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate() {
        CameraControl();
        if(playerCombat.targets.Count > 0) {
            if(CameraToggleState()) {CameraLookAt(playerCombat.targets[playerCombat.lockOnTargetIndex]);}
            else {CameraLookAt(playerCombat.targets[playerCombat.targetIndex]);}
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
        if(lockOnTarget) { lockOnTarget = false; transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z); transform.localEulerAngles = Vector3.zero; }
        else { HandleCameraLockOn(); }
    }
    private void HandleCameraLockOn()
    {
        float shortestDistance = Mathf.Infinity;

        if(playerCombat.targetIndex <= playerCombat.targets.Count) {
            if(playerCombat.targets[playerCombat.targetIndex] != null)
            {
                Vector3 lockTargetDirection = transform.root.position - playerCombat.targets[playerCombat.targetIndex].position;
                float distanceFromTarget = Vector3.Distance(transform.root.position, playerCombat.targets[playerCombat.targetIndex].position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, Camera.main.transform.forward);

                if(transform.root != playerCombat.targets[playerCombat.targetIndex].root 
                    && viewableAngle > -50 &&  viewableAngle > 50 
                    && distanceFromTarget <= maxLockOnDistance)
                {
                    transform.localPosition = new Vector3(1, transform.localPosition.y, transform.localPosition.z);
                    lockOnTarget = true;
                }

                if(distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                }
            }
        } else {
            lockOnTarget = false;
            transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
            transform.localEulerAngles = Vector3.zero;
        }
    }
    public bool CameraToggleState()
    {
        return lockOnTarget;
    }
    private void CameraLookAt(Transform target){
        if(!lockOnTarget || !(playerCombat.lockOnTargetIndex > 0) && !(playerCombat.lockOnTargetIndex < playerCombat.targets.Count)) {
            transform.LookAt(characterCenter);
            return;
        }

        int speed = 1;

        Vector3 dir = target.position - transform.position;
        dir.Normalize();
        Quaternion camRotation = Quaternion.LookRotation(dir);

        // CAMERA
        transform.rotation = Quaternion.Slerp(transform.rotation, camRotation, speed * Time.deltaTime);   

        dir = target.position - characterCenter.position;
        dir.Normalize();
        Quaternion centerRotation = Quaternion.LookRotation(dir);

        // HANDLE
        characterCenter.rotation = Quaternion.Slerp(characterCenter.transform.rotation, centerRotation, speed * Time.deltaTime);   
        
        targetIndicator.position = target.position + new Vector3(0, 2, 0);
    }
}
