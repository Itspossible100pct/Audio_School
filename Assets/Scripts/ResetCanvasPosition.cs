using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class ResetCanvasPosition : MonoBehaviour
{
    public Transform cameraTransform;  // Assign your XR camera's transform here in the inspector
    public float distanceFromCamera = 1.25f;  // How far in front of the camera the Canvas should appear

    [SerializeField] private Transform _companionCanvas1;
    [SerializeField] private Transform _companionCanvas2;
    public Vector3 companionOffset1 = new Vector3(0f, 0f, 0f);  // Example offset
    public Vector3 companionOffset2 = new Vector3(0f, 0f, 0f); // Example offset
    
    private IEnumerator Start()
    {
        // Wait for a short delay before setting the canvas position
        yield return new WaitForSeconds(1.0f);  // Delay for 1 second, adjust as needed
        ResetCanvas();
    }

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