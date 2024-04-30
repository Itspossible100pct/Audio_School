using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnapToLocation : MonoBehaviour
{
    enum Connector
    {
        XlrSend = 0,
        XlrRecieve = 1,
        JackSend = 2,
        JackRecieve = 3,
        RcaWhite = 4,
        RcaRed = 5,
        MiniJack = 6
    }
    
    
    private bool _grabbed;
    private bool _insideSnapZone;
    public bool snapped;

    [SerializeField] private GameObject connector;
    [SerializeField] private GameObject _snapRotationReference;

    // Detects when connector game object has entered the snap zone radius
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == connector.name)
        {
            _insideSnapZone = true;
            Debug.Log("entered");
        }
    }
    // Detects when connector game object has left the snap zone radius
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == connector.name )
        {
            _insideSnapZone = false;
            Debug.Log("left");
        }
    }
    void SnapObject()
    {
        if (_grabbed == false && _insideSnapZone)
        {
            connector.gameObject.transform.position = transform.position;
            connector.gameObject.transform.rotation = _snapRotationReference.transform.rotation;
            snapped = true;
        }
    }
    private void Update()
    {
        // Set grabbed to equal the boolean value "isGrabbed" from OVRGrabbable script
        //_grabbed = connector.GetComponent<OVRGrabbable>().isGrabbed; // check for different script
        SnapObject();
    }
}
