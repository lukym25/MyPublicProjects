using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Lukas.MyClass;

public class SoundManager : MySingelton<SoundManager>
{
    public Sound[] sounds;
    public AudioMixerGroup audioMixer;

    // public static audioManagerScript managerA;

    private void Start()
    {
        AssignSounds();
        PlaySound("BackgroundMusic");
    }

    public void PlaySound(string name)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == name)
            {
                sound.source.Play();
                return;
            }
        }

        Debug.Log("Name :" + name + ": Not found MM");
        return;
    }

    public void PauseSound(string name)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == name)
            {
                sound.source.Pause();
                return;
            }
        }

        Debug.Log("Name :" + name + ": Not found MM");
        return;
    }

    public void StopSound(string name)
    {
        foreach(Sound sound in sounds)
        {
            if (sound.name == name)
            {
                sound.source.Stop();
                return;
            }
        }

        Debug.Log("Name :" + name + ": Not found MM");
        return;
    }

    public void AssignSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = audioMixer;
        }
    }

    [System.Serializable]
    public class Sound
    {
    public string name;
    public AudioClip clip;

    [Range(0.1f, 5f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch = 1;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
    }
}
