using UnityEngine;

public class CameraSmoothCollision : MonoBehaviour
{
    public Transform referenceTransform;
    public float collisionOffset = 0.2f; //To prevent Camera from clipping through Objects
    public LayerMask detectionLayers;

    Vector3 referencePosition;
    Vector3 defaultPos;
    Vector3 directionNormalized;
    Transform parentTransform;
    float defaultDistance;

    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform.localPosition;
        parentTransform = transform.parent;
        referencePosition = referenceTransform.position;
        SettingUpProcedure();
    }

    // FixedUpdate for physics calculations
    void FixedUpdate()
    {
        Vector3 currentPos = defaultPos;
        RaycastHit hit;
        Vector3 dirTmp = parentTransform.TransformPoint(defaultPos) - referenceTransform.position;
        if (Physics.SphereCast(referenceTransform.position,  collisionOffset, dirTmp, out hit, defaultDistance, detectionLayers))
        {
            currentPos = (directionNormalized * (hit.distance - collisionOffset));
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * 15f);
    }
    public void HadleLockOnCollision(Vector3 cameraPos)
    {
        defaultPos = cameraPos;
        referencePosition = referenceTransform.position + Vector3.right;
        SettingUpProcedure();
    }
    public void SettingUpProcedure()
    {
        directionNormalized = defaultPos.normalized;
        defaultDistance = Vector3.Distance(defaultPos, Vector3.zero);
    }
    
}