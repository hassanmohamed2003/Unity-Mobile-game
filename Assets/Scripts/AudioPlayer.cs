using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioSource audioSourceMusic;
    public AudioSource audioSourceAmbient;
    public AudioClip BuildingHitSound;
    public AudioClip ExplosionSound;
    public AudioClip RopeBlinkSound;
    public AudioClip RopeBreakSound;
    public AudioClip PerfectSound;
    public List<AudioClip> ComboSounds;
    public AudioClip LevelCompleteSound;
    public AudioClip buttonClickSound;



    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        int musicPref = PlayerPrefs.GetInt("MusicOn", 1);
        int SFXPref = PlayerPrefs.GetInt("SFXOn", 1);

        audioSource.volume = SFXPref;

        audioSourceMusic.volume = musicPref;
        audioSourceAmbient.volume = musicPref;

    }

    public void PlaySoundEffect(AudioClip soundEffect)
    {
        audioSource.PlayOneShot(soundEffect);
    }

    public void PlayBuildingHitSound()
    {
        audioSource.PlayOneShot(BuildingHitSound);
    }

    public void PlayComboSound(int combo)
    {
        audioSource.PlayOneShot(ComboSounds[combo]);
    }

    public void PlayExplosionSound()
    {
        audioSource.PlayOneShot(ExplosionSound);
    }

    public void PlayLevelCompleteSound()
    {
        audioSource.PlayOneShot(LevelCompleteSound);
    }

    public void PlayRopeBreakSound()
    {
        audioSource.PlayOneShot(RopeBreakSound);
    }

    public void PlayRopeBlinkSound()
    {
        audioSource.PlayOneShot(RopeBlinkSound);
    }
}
