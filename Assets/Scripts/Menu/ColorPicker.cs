using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    private float r = 1f;
    private float g = 1f;
    private float b = 1f;

    public Slider r_slider;
    public Slider g_slider;
    public Slider b_slider;

    public bool leadingColor;
    public bool detailColor;

    private Color defaultColor;
    private Color currentColor;

    private void Start() {
        if(leadingColor)
            defaultColor = new Color(255f / 255f, 46f / 255f, 189f / 255f);
        else if(detailColor)
            defaultColor = new Color(89f / 255f, 0f, 152f / 255f);
        
        r_slider.value = defaultColor.r;
        g_slider.value = defaultColor.g;
        b_slider.value = defaultColor.b;
        
        SetColor();
    }

    public void SetR(float _r) {
        r = _r;
        SetColor();
    }

    public void SetG(float _g) {
        g = _g;
        SetColor();
    }

    public void SetB(float _b) {
        b = _b;
        SetColor();
    }

    public void SetColor() {
        currentColor = new Color(r,g,b);
    }

    public Color GetColor() {
        return currentColor;
    }
}
