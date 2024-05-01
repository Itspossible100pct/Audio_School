using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.PoseDetection;
using Unity.VisualScripting;
using UnityEngine;

public class SnapToLocation : MonoBehaviour
{
   
    
    
    private bool _grabbed;
    private bool _insideSnapZone;
    public bool snapped;

    [SerializeField] private GameObject connector;
    [SerializeField] private GameObject _snapRotationReference;

    // Detects when connector game object has entered the snap zone radius
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == connector.name && !snapped)
        {
            _insideSnapZone = true;
            Debug.Log("entered");
            SnapObject();
        }
    }
    // Detects when connector game object has left the snap zone radius
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == connector.name  )
        {
            _insideSnapZone = false;
            Debug.Log("left");
        }
    }
    void SnapObject()
    {
        if (_grabbed == false && _insideSnapZone)
        {
            snapped = true;
        }
    }
    private void FixedUpdate()
    {
        // Set grabbed to equal the boolean value "isGrabbed" from OVRGrabbable script
        //_grabbed = connector.GetComponent<OVRGrabbable>().isGrabbed; // check for different script
        if (snapped)
        {
            connector.gameObject.transform.position = transform.position;
            connector.gameObject.transform.rotation = _snapRotationReference.transform.rotation;
        }
        
    }
}
