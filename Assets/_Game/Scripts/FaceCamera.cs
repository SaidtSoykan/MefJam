using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main)
            transform.forward = Camera.main.transform.forward;
    }
}