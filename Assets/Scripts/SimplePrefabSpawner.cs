using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class SimplePrefabSpawner : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    
    [System.Serializable]
    public struct PrefabPair
    {
        public GameObject prefab;
        public GameObject previewPrefab;
    }

    public List<PrefabPair> prefabPairs;
    private GameObject _currentPreview;
    private int _currentPrefabIndex = 0; // Example index to track which prefab is selected

    private float _cumulativeYRotation = 0f;
    public bool canPlaceObjects = false;
    
    public ConnectionDetector[] connections; // Assign all the ConnectionDetector components

    
    
    void Awake()
    {
        //_oVRSceneManager = FindObjectOfType<OVRSceneManager>();
        //_oVRSceneManager.SceneModelLoadedSuccessfully += SceneLoaded;
    }
    
    void Start()
    {
        canPlaceObjects = false;
        if (prefabPairs != null && prefabPairs.Count > 0)
            _currentPreview = Instantiate(prefabPairs[_currentPrefabIndex].previewPrefab);
    }
    
    public void EnablePlacing()
    {
        canPlaceObjects = true;
    }

    public void DisablePlacing()
    {
        canPlaceObjects = false;
    }
    
    void Update()
    {
        if (!canPlaceObjects || _currentPreview == null) return;
        
        Debug.Log(canPlaceObjects + "is the answer I Seek");
        Ray ray = new Ray(
            OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);

        if (Physics.Raycast(ray, out RaycastHit hit) && canPlaceObjects)
        {
            if (_currentPreview != null)
            {
                _currentPreview.transform.position = hit.point;
                _currentPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                Vector2 thumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
                _cumulativeYRotation += thumbstickInput.x * Time.deltaTime * 50;
                _currentPreview.transform.rotation *= Quaternion.Euler(0, _cumulativeYRotation, 0);
            }

            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                InstantiateCurrentPrefab(hit.point);
                UpdatePreviewToNextPrefab();
            }
        }
    }

    void InstantiateCurrentPrefab(Vector3 position)
    {
        if (prefabPairs != null && _currentPrefabIndex < prefabPairs.Count)
        {
            Instantiate(prefabPairs[_currentPrefabIndex].prefab, position, _currentPreview.transform.rotation);
        }
    }

    /* void UpdatePreviewToNextPrefab()
    {
        Destroy(_currentPreview); // Destroy current preview
        _currentPrefabIndex = (_currentPrefabIndex + 1) % prefabPairs.Count; // Cycle through the list

        if (prefabPairs.Count > _currentPrefabIndex) // Check if there's another prefab to preview
        {
            _currentPreview = Instantiate(prefabPairs[_currentPrefabIndex].previewPrefab);
        }
    }*/
    
    void UpdatePreviewToNextPrefab()
    {
        Destroy(_currentPreview);
        if (prefabPairs != null && prefabPairs.Count > 0)
        {
            _currentPrefabIndex = (_currentPrefabIndex + 1) % prefabPairs.Count;

            if (_currentPrefabIndex == 0) // All objects have been cycled through
            {
                DisablePlacing();
            }
            else
            {
                _currentPreview = Instantiate(prefabPairs[_currentPrefabIndex].previewPrefab);
            }
        }
    }
    
    public void CheckAllConnections()
    {
        if (connections != null && connections.All(c => c.isConnected))
        {
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.FinishLesson(); // Call the method that signifies completion
            }
        }
    }
    
    
}
