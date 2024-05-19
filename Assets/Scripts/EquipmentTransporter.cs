using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;
using UnityEngine.Serialization; // Include this if you're using OVRInput

public class EquipmentTransporter : MonoBehaviour
{
    public enum TransporterMode
    {
        Placing,
        Adjusting,
        None
    }

    private TransporterMode _currentMode = TransporterMode.None;
    
    [System.Serializable]
    public struct EquipmentPair
    {
        public GameObject equipment; // Actual equipment in the scene (initially inactive).
        public GameObject preview;   // Preview (ghost) object for positioning.
    }

    [SerializeField] private UIManager _uiManager; // Reference to the UI Manager.
    [SerializeField] private EquipmentPair[] _equipmentPairs; // Array of equipment and their corresponding previews.
    [SerializeField] private ConnectionDetector[] _connections; // Array of all ConnectionDetector components.

    private GameObject _currentPreview;
    private int _currentEquipmentIndex = 0;
    private float _cumulativeYRotation = 0f;
    public bool canTransport = false;

    void Start()
    {
        InitializePreviews(); // Make sure all previews are inactive at start.
    }

    private void InitializePreviews()
    {
        foreach (var pair in _equipmentPairs)
        {
            pair.equipment.SetActive(false); // Ensure all equipment are initially inactive.
            pair.preview.SetActive(false);  // Ensure all previews are initially inactive.
        }
    }

    public void EnableTransport()
    {
        canTransport = true;
        if (_equipmentPairs.Length > 0)
            SetPreviewObject(_currentEquipmentIndex); // Activate the initial preview when transport is enabled.
    }

    public void DisableTransport()
    {
        canTransport = false;
        if (_currentPreview != null)
            _currentPreview.SetActive(false);
    }

    private void SetPreviewObject(int index)
    {
        if (_currentPreview != null)
        {
            _currentPreview.SetActive(false); // Deactivate the previous preview.
        }
        _currentPreview = _equipmentPairs[index].preview;
        _currentPreview.SetActive(true); // Activate the new preview.
    }

    void Update()
    {
        if (!canTransport || _currentPreview == null) return;

        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Ray ray = new Ray(controllerPosition, controllerRotation * Vector3.forward);
    
        if (Physics.Raycast(ray, out RaycastHit hit, 0.5f)) // Limit raycast distance to 0.5 units
        {
            // Ensure that the surface is roughly horizontal by checking the angle is close to zero
            if (Vector3.Angle(Vector3.up, hit.normal) < 10f)
            {
                _currentPreview.transform.position = hit.point;
                _currentPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                // Thumbstick input for adjusting Y-axis rotation
                Vector2 thumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
                _cumulativeYRotation += thumbstickInput.x * Time.deltaTime * 50; // accumulate rotation input over time
                _currentPreview.transform.rotation *= Quaternion.Euler(0, _cumulativeYRotation, 0);
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            PlaceCurrentEquipment();
            UpdatePreviewToNextEquipment();
        }
    }

    private void HandlePreviewPositioning()
    {
        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        Ray ray = new Ray(controllerPosition, controllerRotation * Vector3.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _currentPreview.transform.position = hit.point;
            _currentPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            HandleThumbstickRotation();
        }
        else
        {
            _currentPreview.transform.position = controllerPosition + controllerRotation * Vector3.forward * 2f; // Default position in front of the controller
        }
    }

    private void HandleThumbstickRotation()
    {
        Vector2 thumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        _cumulativeYRotation += thumbstickInput.x * Time.deltaTime * 50;
        _currentPreview.transform.RotateAround(_currentPreview.transform.position, Vector3.up, _cumulativeYRotation);
        _cumulativeYRotation = 0; // Reset cumulative rotation after applying to avoid continuous rotation
    }

    private void PlaceCurrentEquipment()
    {
        var equipment = _equipmentPairs[_currentEquipmentIndex].equipment;
        equipment.transform.position = _currentPreview.transform.position;
        equipment.transform.rotation = _currentPreview.transform.rotation;
        equipment.SetActive(true); // Activate the actual equipment.
        CheckAllConnections(); // Check connections after placing the equipment.
    }

    private void UpdatePreviewToNextEquipment()
    {
        _currentEquipmentIndex = (_currentEquipmentIndex + 1) % _equipmentPairs.Length;
        if (_currentEquipmentIndex == 0)
        {
            DisableTransport(); // Optionally stop transporting after cycling through all items.
        }
        else
        {
            SetPreviewObject(_currentEquipmentIndex); // Set up the next preview object.
        }
    }

    private void CheckAllConnections()
    {
        if (_connections != null && System.Array.TrueForAll(_connections, c => c.isConnected))
        {
            Debug.Log("All connections have been made.");
            if (_uiManager != null)
                _uiManager.FinishLesson(); // Notify UIManager that all connections are complete.
        }
    }
}
