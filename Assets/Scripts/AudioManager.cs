using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _speakerSource00;
    [SerializeField] private AudioSource _speakerSource01;
    [SerializeField] private AudioSource _mixerSource;
    [SerializeField] private AudioSource _micSource;
    [SerializeField] private AudioClip[] _connectionSound;
    [SerializeField] private AudioClip _speakerFeedback;


    void PlayConnectionSound()
    {
        
    }



}
