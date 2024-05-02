using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionDetector : MonoBehaviour
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private UIManager uiManager;  // Reference to the UIManager
    public bool isConnected = false;
    private MeshRenderer[] allRenderers;  // Array to hold all relevant renderers
    public int connectionIndex;  // Index to identify this connection in the UIManager

    void Start()
    {
        allRenderers = GetComponentsInChildren<MeshRenderer>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cable"))
        {
            isConnected = true;  // Assume connected when cable triggers the collider
            SetRenderers(true);
            //_audioManager.PlayConnectionSound();  // Optionally play connection sound
            uiManager.UpdateConnectionStatus(connectionIndex, isConnected);  // Notify UIManager of status change
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cable"))
        {
            isConnected = false;
            SetRenderers(false);
            uiManager.UpdateConnectionStatus(connectionIndex, isConnected);  // Notify UIManager of status change
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
        foreach (var renderer in allRenderers)
        {
            renderer.enabled = state;
        }
    }
}