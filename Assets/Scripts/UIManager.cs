using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI; // Ensure this is added to handle UI components directly

public class UIManager : MonoBehaviour
{
    [SerializeField] private SimplePrefabSpawner spawner;
    [SerializeField] private Button continueButton1; // For the first continue
    [SerializeField] private Button continueButton2; // For the second continue
    [SerializeField] private Button continueButton3; // For the third continue
    [SerializeField] private Button skipButton;
    [SerializeField] private GameObject helpCanvas; // UI canvas that contains help materials

    [SerializeField] private GameObject _onboard01;
    [SerializeField] private GameObject _onboard02;
    [SerializeField] private GameObject _onboard03;
    [SerializeField] private GameObject _onboardMain;
    [SerializeField] private GameObject _diagram;
    [SerializeField] private GameObject _xlrCanvas;

    [SerializeField] private GameObject _cableGroup;

    [SerializeField] private AudioSource _canvasAudioSource;
    [SerializeField] private AudioClip _audioBoard01;
    [SerializeField] private AudioClip _audioBoard02;
    [SerializeField] private AudioClip _audioBoard03;
    [SerializeField] private AudioClip _audioBoard04;
    [SerializeField] private AudioClip _audioBoard05;
    [SerializeField] private AudioClip _audioBoard06;

    [SerializeField] private AudioSource _bGM;
    
    
   

    void Start()
    {
        
        // Add listeners to buttons
        continueButton1.onClick.AddListener(ShowMessageTwo);
        continueButton2.onClick.AddListener(ShowMessage3);
        continueButton3.onClick.AddListener(IntroToXLRCables);
        skipButton.onClick.AddListener(HandleSkip);
        // startButton.onClick.AddListener(StartLesson);

        // Initially disable object placement and help canvas
        spawner.DisablePlacing();
        // helpCanvas.enabled = false; // Ensure the help canvas is initially disabled
        
        _onboardMain.SetActive(false);
        _onboard01.SetActive(false);
        _onboard02.SetActive(false);
        _onboard03.SetActive(false);

        ShowOnboarding();
        StartCoroutine(DelayBeforeVoiceover(2));
        }

    public void ShowOnboarding()
    {
        StartCoroutine(DelayBeforeVoiceover(2));
        _onboardMain.SetActive(true);
        _onboard01.SetActive(true);
        // Display initial onboarding message
        Debug.Log("Welcome to Audio School: Learn your sound tech skills!");
        // Additional logic for playing voiceover or showing visuals can be added here
        
        StartCoroutine(DelayBeforeVoiceover(2));
        _canvasAudioSource.PlayOneShot(_audioBoard01);
    }

    public void ShowMessageTwo()
    {
        // Continue to the next part of onboarding
        _onboard02.SetActive(true);
        
        Debug.Log("Lesson 1: Mapping the Vibrations - Connect the system.");
        
       // Additional voiceover or visual effects can be triggered here
        StartCoroutine(DelayBeforeVoiceover(3));
        _canvasAudioSource.PlayOneShot(_audioBoard02);
    }

    public void ShowMessage3()
    {
        _onboard03.SetActive(true);
        // Show the message to prepare for the lesson
        Debug.Log("Let's get sound out of the system. Avoid feedback.");
        spawner.EnablePlacing(); // Enable placing objects if part of onboarding requires this
        
        // Additional voiceover or visual effects can be triggered here
        StartCoroutine(DelayBeforeVoiceover(3));
        _canvasAudioSource.PlayOneShot(_audioBoard03);
    }

    public void IntroToXLRCables()
    {
        _onboardMain.SetActive(false);
        
        // Show Audio Signal Flow Diagram
        _diagram.SetActive(true);
        
        
        StartCoroutine(DelayBeforeVoiceover(1));
        _canvasAudioSource.PlayOneShot(_audioBoard04);
        StartCoroutine(DelayBeforeVoiceover(15));
        StartLesson();
    }

    public void StartLesson()
    {
        //Show XLR image. 
        _xlrCanvas.SetActive(true);
        
        // Actions to start the actual lesson
        Debug.Log("Starting the lesson...");
        spawner.EnablePlacing();
        helpCanvas.SetActive(true); // Activate the help canvas
        _cableGroup.SetActive(true);
        
    }

    public void HandleSkip()
    {
        
        Debug.Log("Skipping to lesson.");
        StartLesson(); // Use StartLesson to unify the behavior
    }

    private IEnumerator DelayBeforeVoiceover(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
    }

    public void FinishLesson()
    {
        _canvasAudioSource.PlayOneShot(_audioBoard06);
        StartCoroutine(DelayBeforeVoiceover(8));
        
        _canvasAudioSource.PlayOneShot(_audioBoard05);
    }

   
    
}