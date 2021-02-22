using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteGlitchEffectChange : MonoBehaviour
{
    private Image image;
    public Material targetMat;
    void Start()
    {
        image = GetComponent<Image>();
        image.material = targetMat;
    }
}
