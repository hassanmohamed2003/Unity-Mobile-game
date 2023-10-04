using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicPlayer : MonoBehaviour
{
    [Header("Clips")]
    public List<AudioClip> LevelMusics;
    private AudioSource sfxSource;
    public AudioSource musicSource;

    public void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
        int musicPref = PlayerPrefs.GetInt("MusicOn", 1);
        int SFXPref = PlayerPrefs.GetInt("SFXOn", 1);

        sfxSource.volume = SFXPref;

        musicSource.volume = musicPref;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if(!GameState.IsEndless)
        {
            musicSource.clip = LevelMusics[GameState.CurrentLevelID];
            musicSource.loop = true;
            musicSource.Play();
        }  
    }
}
