using UnityEngine;

public class KnifeRotation : MonoBehaviour
{
    public Vector3 rotation = new Vector3(0, 0, 1);
    void Update()
    {
        transform.Rotate(rotation);
    }
}
