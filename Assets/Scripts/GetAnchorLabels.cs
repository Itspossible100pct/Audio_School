using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetAnchorLabels : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    
    void Update()
    {
        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Vector3 rayDirection = controllerRotation * Vector3.forward;

        if (Physics.Raycast(controllerPosition, rayDirection, out RaycastHit hit))
        {
            _lineRenderer.SetPosition(0, controllerPosition);

            OVRSemanticClassification
                anchor = hit.collider.gameObject.GetComponentInParent<OVRSemanticClassification>();

            if (anchor != null)
            {
                print($"Hit an Anchor with the label: {string.Join(", ", anchor.Labels)}");
                Vector3 endPoint = anchor.transform.position;
                _lineRenderer.SetPosition(1, endPoint);
            }
            else
            {
                _lineRenderer.SetPosition(1, hit.point);
            }
        }
    }
}
