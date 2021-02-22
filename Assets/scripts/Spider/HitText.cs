using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitText : MonoBehaviour
{
    public float destroyTime = 3f;
    public Vector3 Offset = new Vector3(0,2,0);
    void Start()
    {
        Destroy(gameObject, destroyTime);

        transform.localPosition += Offset;
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
