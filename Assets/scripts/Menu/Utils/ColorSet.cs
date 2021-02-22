using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSet : MonoBehaviour
{
    public Material mat;
    public ColorPicker picker;

    private void Update() {
        mat.SetColor("_BaseColor", picker.GetColor());
    }
}
