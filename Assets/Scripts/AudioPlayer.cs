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

    public GameObject buttonMusic;
    public GameObject buttonSFX;

    public Sprite Off;
    public Sprite On;

    private bool MusicOn = true;
    private bool SFXOn = true;

    private Image ImageMusic;
    private Image ImageSFX;
    public void Awake()
    {
        ImageMusic = buttonMusic.GetComponent<Image>();
        ImageSFX = buttonSFX.GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        int musicPref = PlayerPrefs.GetInt("MusicOn", 1);
        int SFXPref = PlayerPrefs.GetInt("SFXOn", 1);
        audioSource.volume = SFXPref;

        audioSourceMusic.volume = musicPref;
        audioSourceAmbient.volume = musicPref;
        if (musicPref == 1)
        {
            ImageMusic.sprite = On;
        }
        else
        {
            ImageMusic.sprite = Off;
        }

        if(SFXPref == 1)
        {
            ImageSFX.sprite = On;
        }
        else
        {
            ImageSFX.sprite = Off;
        }

        ChangeImage();

    }

    public void PlaySoundEffect(AudioClip soundEffect)
    {
        audioSource.PlayOneShot(soundEffect);
    }

    private void ChangeImage()
    {
        buttonMusic.GetComponent<Image>().sprite = ImageMusic.sprite;
    }
    public void MusicOnOff()
    {
        Debug.Log(MusicOn);
        if (MusicOn)
        {
            PlayerPrefs.SetInt("MusicOn", 0);
            audioSource.PlayOneShot(buttonClickSound);
            MusicOn = false;
            audioSourceMusic.volume = 0;
            audioSourceAmbient.volume = 0;
            ImageMusic.sprite = Off;
        }
        else
        {
            PlayerPrefs.SetInt("MusicOn", 1);
            MusicOn = true;

            audioSourceMusic.volume = 1;
            audioSourceAmbient.volume = 1;
            audioSource.PlayOneShot(buttonClickSound);
            ImageMusic.sprite = On;
        }
        PlayerPrefs.Save();
        ChangeImage();
    }

    public void SFXOnOff()
    {
        if (SFXOn)
        {
            SFXOn = false;
            audioSource.volume = 0;
            audioSource.PlayOneShot(buttonClickSound);
            PlayerPrefs.SetInt("SFXOn", 0);
            ImageSFX.sprite = Off;
        }
        else
        {
            SFXOn = true;
            audioSource.volume = 1;
            audioSource.PlayOneShot(buttonClickSound);
            PlayerPrefs.SetInt("SFXOn", 1);
            ImageSFX.sprite = On;
        }

        PlayerPrefs.Save();
        ChangeImage();
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
