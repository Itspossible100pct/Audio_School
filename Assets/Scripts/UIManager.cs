using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Ensure this is added to handle UI components directly

public class UIManager : MonoBehaviour
{
    [SerializeField] private SimplePrefabSpawner spawner;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button skipButton;

    void Start()
    {
        // Add listeners to buttons
        continueButton.onClick.AddListener(HandleContinue);
        skipButton.onClick.AddListener(HandleSkip);

        // Initially disable object placement
        spawner.DisablePlacing();
        
        ShowOnboarding();
    }

    void ShowOnboarding()
    {
        // Display initial onboarding message
        // Placeholder: Insert your UI code to show onboarding message
        Debug.Log("Welcome to Audio School: Learn your sound tech skills!");
    }

    public void HandleContinue()
    {
        // Continue to the next part of onboarding or lesson
        spawner.EnablePlacing(); // Enable placing objects if part of onboarding requires this
        // Placeholder: Update UI to show next steps or messages
        Debug.Log("Lesson 1: Mapping the Vibrations - Connect the system.");
    }

    public void HandleSkip()
    {
        // Skip the onboarding directly to the lesson
        spawner.EnablePlacing();
        // Possibly load the lesson scene or continue in the same scene
        Debug.Log("Skipping to lesson.");
    }

    // Add additional methods as needed for other UI interactions
}