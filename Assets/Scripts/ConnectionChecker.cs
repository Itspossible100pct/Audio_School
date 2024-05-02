using UnityEngine;

public class ConnectionDetector : MonoBehaviour
{
    public bool isConnected = false;
    private MeshRenderer[] allRenderers;  // Array to hold all relevant renderers

    void Start()
    {
        // Get all MeshRenderer components in this GameObject and its children
        allRenderers = GetComponentsInChildren<MeshRenderer>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cable"))  // Ensure your cable GameObject has a tag "Cable"
        {
            isConnected = false;  // Assume not connected initially
            SetRenderers(true);  // Turn on the renderer when a correct connector is nearby
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cable"))
        {
            isConnected = false;  // Mark as not connected
            SetRenderers(false);  // Turn off the renderer when the connector moves away
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cable") && isConnected)
        {
            SetRenderers(false);  // Turn off the renderer if the connection is made
        }
    }

    private void SetRenderers(bool state)
    {
        foreach (var renderer in allRenderers)
        {
            renderer.enabled = state;  // Toggle each renderer based on the passed state
        }
    }
}