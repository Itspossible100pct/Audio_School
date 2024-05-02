using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionDetector : MonoBehaviour
{
    [SerializeField] private AudioManager _audioManager;
    public bool isConnected = false;
    private MeshRenderer[] allRenderers;  // Array to hold all relevant renderers

    void Start()
    {
        // Get all MeshRenderer components in this GameObject and its children
        allRenderers = GetComponentsInChildren<MeshRenderer>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cable"))
        {
            isConnected = true;  // Assume connected when cable triggers the collider
            SetRenderers(true);
            //_audioManager.PlayConnectionSound();  // Play connection sound
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cable"))
        {
            isConnected = false;
            SetRenderers(false);
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