using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    private Camera mainCam;
    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        transform.LookAt(mainCam.transform.position);

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
