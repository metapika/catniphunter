using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public List<GameObject> allMods = new List<GameObject>();
    public List<GameObject> collectedMods = new List<GameObject>();

    private void Awake() {
        ReloadMods();
    }

    public void ReloadMods() {
        foreach (Transform mod in transform)
        {
            if(!collectedMods.Contains(mod.gameObject)) {
                collectedMods.Add(mod.gameObject);
            }
        }
    }
}
