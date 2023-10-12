using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private static Music instance = null;
    private AudioSource _audioSource;
    private Settings settings;
    public static Music Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        settings = SaveGame.LoadSettings();
        _audioSource = GetComponent<AudioSource>();
        if (settings.MusicOn == false) {
            _audioSource.Stop();
            _audioSource.volume = 0;
        } else {
            _audioSource.Play();
            _audioSource.volume = 0.1F;
        }
    }
    public void PlayMusic()
     {
         _audioSource.volume = 0.1F;
         if (_audioSource.isPlaying) {
             return;
         }
         _audioSource.Play();
     }
 
     public void StopMusic()
     {
         _audioSource.Stop();
         _audioSource.volume = 0;
     }
}
