using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundEffect
{
    Door_Open,
    Door_Close,
    Door_Locked,
    Door_Unlock,

    Keycard_Accept,
    Keycard_Reject,

    Vehicle_Standby,

    Footstep_Grass,
    Footstep_Stone,

    Drop_Regular,
    Drop_Wood,
    Drop_Plastic,

    Throw_Regular,

    Alarm_TimeLow,

    Inventory_Collect

}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [SerializeField] private Dictionary<SoundEffect, List<AudioClip>> soundMap = new Dictionary<SoundEffect, List<AudioClip>>();


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializeSoundMap();
    }

    private void InitializeSoundMap()
    {
        // Door
        MapSoundEffectToAudioClip(SoundEffect.Door_Open, "doorOpen");
        MapSoundEffectToAudioClip(SoundEffect.Door_Close, "doorClose");
        MapSoundEffectToAudioClip(SoundEffect.Door_Locked, "doorLocked");
        MapSoundEffectToAudioClip(SoundEffect.Door_Unlock, "doorUnlock");
        // Keycard
        MapSoundEffectToAudioClip(SoundEffect.Keycard_Accept, "keycardReaderAccept");
        MapSoundEffectToAudioClip(SoundEffect.Keycard_Reject, "keycardReaderDecline");
        // Vehicle
        MapSoundEffectToAudioClip(SoundEffect.Vehicle_Standby, "carStandby_LOOP");
        // Footstep
        MapSoundEffectToAudioClips(SoundEffect.Footstep_Grass, 
            new string[] {"step_grass1", "step_grass2", "step_grass3", "step_grass4"});
        MapSoundEffectToAudioClips(SoundEffect.Footstep_Stone,
            new string[] { "step_stone1", "step_stone2", "step_stone3", "step_stone4" });
        // Drop
        MapSoundEffectToAudioClip(SoundEffect.Drop_Regular, "dropRegular");
        MapSoundEffectToAudioClip(SoundEffect.Drop_Wood, "dropWood");
        MapSoundEffectToAudioClip(SoundEffect.Drop_Plastic, "dropPlastic");
        // Throw
        MapSoundEffectToAudioClip(SoundEffect.Throw_Regular, "throw");
        // Alarm
        MapSoundEffectToAudioClip(SoundEffect.Alarm_TimeLow, "timeLow");
        // Inventory
        MapSoundEffectToAudioClip(SoundEffect.Inventory_Collect, "inventoryCollect");
    }

    private void MapSoundEffectToAudioClip(SoundEffect key, string clipPath)
    {
        AudioClip clip = Resources.Load<AudioClip>(clipPath);

        if (clip != null)
        {
            soundMap.Add(key, new List<AudioClip> { clip });
        }
        else
        {
            Debug.LogError($"Failed to load audio clip: {clipPath}");
        }
    }

    private void MapSoundEffectToAudioClips(SoundEffect key, string[] clipPaths)
    {
        List<AudioClip> clips = new List<AudioClip>();

        foreach (string clipPath in clipPaths)
        {
            AudioClip clip = Resources.Load<AudioClip>(clipPath);

            if (clip != null)
            {
                clips.Add(clip);
            }
            else
            {
                Debug.LogError($"Failed to load audio clip: {clipPath}");
            }
        }

        if (clips.Count > 0)
        {
            soundMap.Add(key, clips);
        }
    }

    public static void PlaySound(GameObject sourceObj, SoundEffect key, float volume = 1.0f, bool overrideExistingSounds = false, bool loop = false, bool playOnAwake = false)
    {
        if (instance != null && instance.soundMap.ContainsKey(key))
        {
            // Add AudioSource to source object if it doesn't already exist.
            AudioSource audioSource = sourceObj.GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = sourceObj.AddComponent<AudioSource>();
            }
            else if (audioSource.isPlaying && overrideExistingSounds)
            {
                audioSource.Stop();
            }

            // Get AudioClip based on SoundEffect key. (Uses random clip from list if count > 1).
            List<AudioClip> clips = instance.soundMap[key];

            if (clips.Count > 1)
            {
                audioSource.clip = clips[Random.Range(0, clips.Count)];
            }
            else
            {
                audioSource.clip = clips[0];
            }

            // Assign remaining settings
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.playOnAwake = playOnAwake;

            // Play the sound
            audioSource.Play();
        }
        else
        {
            Debug.LogError($"Sound clip with key '{key}' not found.");
        }
        
    }

    public static void StopSound(GameObject sourceObj)
    {
        AudioSource audioSource = sourceObj.GetComponent<AudioSource>();

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
