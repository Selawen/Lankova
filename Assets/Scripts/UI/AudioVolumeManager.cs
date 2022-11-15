using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolumeManager : MonoBehaviour
{
    public static AudioVolumeManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null) 
        { 
            foreach(AudioSource a in GetComponentsInChildren<AudioSource>())
            {
                a.Stop();
            }

            Destroy(this); 
            return; 
        }
        Instance = this;
       
        //DontDestroyOnLoad(this);
        
        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            AudioListener.volume = 0.5f;
        }
    }
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
}
