using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private float range;
    
    private IInteractable currentTarget;
    private Camera mainCamera;

    private void Awake() 
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        RaycastForInteractable();

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(currentTarget != null)
            {
                currentTarget.OnInteract();
            }
        }
    }
    private void RaycastForInteractable()
    {
        // if(Physics.Raycast(ray, out whatIHit, range))
        // RaycastHit whatIHit;

        // Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        if(hitColliders.Length > 0)
        {
            foreach(Collider collider in hitColliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();

                if(interactable != null)
                {
                    if(Vector3.Distance(transform.position, collider.transform.position) <= interactable.MaxRange)
                    {
                        if(interactable == currentTarget)
                        {
                            return;
                        }
                        else if(currentTarget != null){
                            currentTarget.OnEndHover();
                            currentTarget = interactable;
                            currentTarget.OnStartHover();
                            return;
                        } else {
                            currentTarget = interactable;
                            currentTarget.OnStartHover();
                        }
                    }
                    else {
                        if(currentTarget != null)
                        {
                            currentTarget.OnEndHover();
                            currentTarget = null;
                            return;
                        }
                    }
                } else {
                    if(currentTarget != null)
                    {
                        currentTarget.OnEndHover();
                        currentTarget = null;
                        return;
                    }
                }
            }
        } else {
            if(currentTarget != null)
            {
                currentTarget.OnEndHover();
                currentTarget = null;
                return;
            }
        }
    }
}
