using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePrefabSpawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject previewPrefab;
    private GameObject _currentPreview;
    
    
    // Start is called before the first frame update
    void Start() => _currentPreview = Instantiate(previewPrefab);
    
    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(
            OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _currentPreview.transform.position = hit.point;
            _currentPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                Instantiate(prefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
        }
    }
}
