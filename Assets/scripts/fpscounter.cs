using UnityEngine;
using TMPro;
public class fpscounter : MonoBehaviour
{
    private TextMeshProUGUI text;
    float frameCount = 0;
    float nextUpdate = 0;
    float fps = 0;
    float updateRate = 4;  // 4 updates per sec.
    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
        nextUpdate = Time.time;
    }
    private void Update() {
        frameCount++;
        if (Time.time > nextUpdate)
        {
            nextUpdate += 1 / updateRate;
            fps = frameCount * updateRate;
            frameCount = 0;
        }
        text.text = fps.ToString();
    }
}
