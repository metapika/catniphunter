using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [HideInInspector] public Camera mainCam;
    public bool fullAxis;

    void Update()
    {
        transform.LookAt(mainCam.transform.position);

        if(!fullAxis) transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
