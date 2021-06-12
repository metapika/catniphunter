using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class FadeFromBlack : MonoBehaviour
{
    public bool fadeAtStart;
    public float fadeDuration = .5f;
    public float sceneStartDelay = 1f;
    public Color fadeColor;
    private Image board;
    private PageController pageController;
    void Start()
    {
        board = GetComponent<Image>();
        pageController = PageController.instance;

        if(fadeAtStart && pageController) StartCoroutine(SceneDelayFade());
    }
    private IEnumerator SceneDelayFade()
    {
        board.color = fadeColor;

        while (pageController.PageIsOn(PageType.Loading))
        {
            yield return null;
        }

        //yield return new WaitForSeconds(sceneStartDelay);

        FadeFromColor(fadeColor);
    }
    public void FadeFromColor(Color targetColor)
    {
        if(board == null) return;

        StartCoroutine(LerpFadeOut(fadeDuration, 1, 0, targetColor));
    }
    private IEnumerator LerpFadeOut(float time, float startValue, float targetValue, Color color)
    {
        float start = Time.time;
        float alpha = 0;

        board.color = color;

        while (Time.time < start + time)
        {
            float completion = (Time.time - start) / time;
            board.color = new Color(color.r, color.g, color.b, Mathf.Lerp(startValue, targetValue, completion));
            yield return null;
        }
        alpha = targetValue;
        board.color = new Color(color.r, color.g, color.b, alpha);
    }

}
