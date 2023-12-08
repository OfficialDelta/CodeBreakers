using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        Vector3 desiredPosition = playerTransform.position + offset;
        transform.position = desiredPosition;
    }
}