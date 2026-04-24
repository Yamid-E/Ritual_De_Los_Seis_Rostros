using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform target; // Player
    public float smoothSpeed = 8f;
    public float wallOffset = 0.3f;
    public LayerMask collisionLayer;

    private Vector3 originalLocalPos;
    private Vector3 currentLocalPos;

    void Start()
    {
        originalLocalPos = transform.localPosition;
        currentLocalPos = originalLocalPos;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Transform pivot = transform.parent;
        if (pivot == null) return;

        Vector3 desiredWorldPos = pivot.TransformPoint(originalLocalPos);
        Vector3 dir = target.position - desiredWorldPos;
        float dist = dir.magnitude;

        Vector3 targetLocalPos = originalLocalPos;

        RaycastHit hit;

        if (Physics.Raycast(
            desiredWorldPos,
            dir.normalized,
            out hit,
            dist,
            collisionLayer))
        {
            Vector3 hitPos = hit.point + hit.normal * wallOffset;
            targetLocalPos = pivot.InverseTransformPoint(hitPos);
        }

        currentLocalPos = Vector3.Lerp(
            currentLocalPos,
            targetLocalPos,
            smoothSpeed * Time.deltaTime
        );

        transform.localPosition = currentLocalPos;
    }
}