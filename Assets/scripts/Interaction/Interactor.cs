using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private IInteractable currentInteractable;
    void Update()
    {
        CheckForInteraction();
    }

    private void CheckForInteraction()
    {
        if(currentInteractable == null) return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.OnInteract();
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        var interactable = other.GetComponent<IInteractable>();

        if(interactable == null) return;

        if(interactable == currentInteractable)
        {
            return;
        }

        RaycastHit hit;
        Vector3 dir = (other.transform.position - transform.position).normalized;

        if(Physics.Raycast(transform.position, dir, out hit, Vector3.Distance(transform.position, other.transform.position))) {
            if(hit.collider != other) return;
        }
        
        if(currentInteractable != null){
            currentInteractable.OnEndHover();
            currentInteractable = interactable;
            currentInteractable.OnStartHover();
            return;
        } else {
            currentInteractable = interactable;
            currentInteractable.OnStartHover();
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        var interactable = other.GetComponent<IInteractable>();

        if(interactable == null) return;

        if(interactable != currentInteractable) {
            return;
        } else {
            currentInteractable.OnEndHover();
            currentInteractable = null;
            return;
        }
    }
}
