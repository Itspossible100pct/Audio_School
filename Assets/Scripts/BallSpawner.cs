using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _ball;
    [SerializeField] private float _force = 4.0f;
    
    private void Update()
    {
        if (OVRInput.GetDown((OVRInput.Button.SecondaryIndexTrigger)))
        {
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            GameObject spawnedBall = Instantiate(_ball, controllerPosition, controllerRotation);
            Rigidbody rigidbody = spawnedBall.GetComponent<Rigidbody>();
            rigidbody.velocity = controllerRotation * Vector3.forward * _force;
        }
    }
}
