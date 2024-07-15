using UnityEditor.Audio;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    void Start()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.output;
            if(sound.playOnStart)
                Play(sound.name);
        }
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        if (sound == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }

        sound.source.Play();
    }
}
