using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ConnectionDetector : MonoBehaviour
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private UIManager _uiManager;  // Reference to the UIManager
    public bool isConnected = false;
    private MeshRenderer[] _allRenderers;  // Array to hold all relevant renderers
    public int connectionIndex;  // Index to identify this connection in the UIManager

    void Start()
    {
        _allRenderers = GetComponentsInChildren<MeshRenderer>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cable"))
        {
            isConnected = true;  // Assume connected when cable triggers the collider
            SetRenderers(true);
            //_audioManager.PlayConnectionSound();  // Optionally play connection sound
            _uiManager.UpdateConnectionStatus(connectionIndex, isConnected);  // Notify UIManager of status change
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cable"))
        {
            isConnected = false;
            SetRenderers(false);
            _uiManager.UpdateConnectionStatus(connectionIndex, isConnected);  // Notify UIManager of status change
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cable") && isConnected)
        {
            StartCoroutine(TurnOffRenderersAfterDelay(2));  // Start the coroutine with a 2-second delay
        }
    }

    private IEnumerator TurnOffRenderersAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetRenderers(false);
    }

    private void SetRenderers(bool state)
    {
        foreach (var renderer in _allRenderers)
        {
            renderer.enabled = state;
        }
    }
}