using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistanceToWallVisualizer : MonoBehaviour
{
    public TextMeshProUGUI distanceText;

    private OVRSceneManager _oVRSceneManager;
    private OVRSceneRoom _sceneRoom;
    private OVRScenePlane[] _roomWalls;
    
    // Start is called before the first frame update
    void Awake()
    {
        _oVRSceneManager = FindObjectOfType<OVRSceneManager>();
        _oVRSceneManager.SceneModelLoadedSuccessfully += SceneLoaded;
    }

    private void SceneLoaded()
    {
        _sceneRoom = FindObjectOfType<OVRSceneRoom>();
        _roomWalls = _sceneRoom.Walls;
    }

    // Update is called once per frame
    void Update()
    {
        if (_sceneRoom != null)
        {
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            OVRScenePlane nearestWallToController = FindNearestWall(controllerPosition);
        } 
    }

    private OVRScenePlane FindNearestWall(Vector3 position)
    {
        OVRScenePlane nearestWall = null;
        float nearestDistance = float.MaxValue;

        foreach (var wall in _roomWalls)
        {
            float distance = CalculateDistanceToPlane(position, wall);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestWall = wall;
            }
        }

        return nearestWall;
    }

    private float CalculateDistanceToPlane(Vector3 position, OVRScenePlane wall)
    {
        Vector3 wallNormal = wall.transform.forward;

        float wallDistance = -Vector3.Dot(wallNormal, wall.transform.position);
        float distance = Mathf.Abs(Vector3.Dot(wallNormal, position) + wallDistance) / wallNormal.magnitude;

        return distance;
    }
}
