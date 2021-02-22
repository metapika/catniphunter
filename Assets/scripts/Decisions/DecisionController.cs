using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class DecisionController : MonoBehaviour
{
    public AbilityManager abilityList;
    public Animator decisionBox;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;

    public void StartDecision(int optionID1, int optionID2) 
    {
        decisionBox.SetTrigger("start");
        
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisableComponents();

        option1Text.text = abilityList.allMods[optionID1].name;
        option2Text.text = abilityList.allMods[optionID2].name;

        option1Text.transform.parent.name = optionID1.ToString();
        option2Text.transform.parent.name = optionID2.ToString();
    }

    public void EnableComponents() {

        Camera.main.GetComponent<CameraController>().enabled = true;

        GetComponent<PlayerController>().enabled = true;
        GetComponent<PlayerCombat>().enabled = true;
        
        transform.Find("CurrentAbilities").gameObject.SetActive(true);
    }
    private void DisableComponents() {


        Camera.main.GetComponent<CameraController>().enabled = false;
        
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
            
        transform.Find("CurrentAbilities").gameObject.SetActive(false);
    }

    public void Choose() {
        int abilityID = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        
        EndDecision(abilityID);
    }

    public void EndDecision(int abilityID) {
        Instantiate(abilityList.allMods[abilityID], transform.Find("CurrentAbilities"));

        abilityList.ReloadMods();

        StartCoroutine(CollectingAnimation());


        decisionBox.SetTrigger("end");
    }

    public IEnumerator CollectingAnimation() {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GetComponent<Animator>().SetBool("collectedMod", true);

        yield return new WaitForSeconds(5.966667f / 3f);

        GetComponent<Animator>().SetBool("collectedMod", false);

        EnableComponents();
    }
}
