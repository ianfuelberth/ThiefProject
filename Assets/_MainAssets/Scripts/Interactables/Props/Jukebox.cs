using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField]
    private GameObject MusicPlayer;
    [SerializeField]
    private bool isMuted = false;

    private float initVolume;

    // turn off game music as easter-egg. remove interactable tag?
    public void Interact()
    {
        if (isMuted)
        {
            MusicPlayer.GetComponent<AudioSource>().volume = initVolume;
            isMuted = false;
        }
        else
        {
            MusicPlayer.GetComponent<AudioSource>().volume = 0f;
            isMuted = true;
        }
        
    }

    void Awake()
    {
        initVolume = MusicPlayer.GetComponent<AudioSource>().volume;
    }

}
