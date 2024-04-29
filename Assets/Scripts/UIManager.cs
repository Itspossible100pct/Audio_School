using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SimplePrefabSpawner spawner;

    public void StartTutorial()
    {
        // Show tutorial window with instructions
        // On user confirmation:
        spawner.EnablePlacing();
    }

    public void EndPlacing()
    {
        // Ask if the player is ready for the next part or wants to reposition objects
        // Based on user input, either call spawner.DisablePlacing() or allow repositioning
    }
    
    public void ShowRepositionInstructions()
    {
        // Display UI message about how to select and move objects
    }
    
    public void ProceedToNextLesson()
    {
        // Code to load the next part of the lesson or an external tutorial
    }
}
