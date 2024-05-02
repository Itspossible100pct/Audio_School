using UnityEngine;
using UnityEngine.XR;

public class ResetCanvasPosition : MonoBehaviour
{
    public Transform cameraTransform;  // Assign your XR camera's transform here in the inspector
    public float distanceFromCamera = 1.5f;  // How far in front of the camera the Canvas should appear

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
        {
            ResetCanvas();
        }
    }

    private void ResetCanvas()
    {
        // Position the Canvas at the camera's position (at eye level)
        transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;

        // Rotate the Canvas to face directly towards the camera
        Vector3 toCamera = (cameraTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-toCamera);
        transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
    }
}