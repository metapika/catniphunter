using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public float cargoSpeed = 10;
    void Update()
    {
        transform.Translate(-cargoSpeed * Time.deltaTime, 0f, 0f);
    }
}
