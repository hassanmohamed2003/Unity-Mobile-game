using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicPlayer : MonoBehaviour
{
    [Header("Clips")]
    public List<AudioClip> LevelMusics;
    private AudioSource musicSource;
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        if(!GameState.IsEndless)
        {
            musicSource.clip = LevelMusics[GameState.CurrentLevelID];
            musicSource.loop = true;
            musicSource.Play();
        }  
    }
}
