using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObject : MonoBehaviour
{
    //Reference the snap zone collider trigger
    [SerializeField] private GameObject _snapDestination;
    
    //Reference game object that the snapped objects will become a part of
    [SerializeField] private GameObject _newParent;
    public bool isSnapped;
    private bool _objectSnapped;
    private bool _grabbed;

    // Update is called once per frame
    void Update()
    {
       //_grabbed = GetComponent<OVRGrabbable>().isGrabbed;  // need to check for different script
        
        //Set objectSnappeed equal to the snapped boolean from SnapToLocation
        Debug.Log(_snapDestination.name + "Snapped");
        _objectSnapped = _snapDestination.GetComponent<SnapToLocation>().snapped;
        
        //Set object Rigidbody to be Kinematic after is has been snapped into position
        //Set object to be a parent of the Rocket object after it has been snapped
        //set isSnapped variable to true 
        if (_objectSnapped)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.SetParent(_newParent.transform);
            isSnapped = true;
        }
        
        //Makes sure that the object can still be grabbed by the OVRGrabber script.
        if (_objectSnapped == false && _grabbed == false)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }

    }
}
