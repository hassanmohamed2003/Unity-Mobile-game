using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    private Image ImageMusic;
    private Image ImageSFX;

    [Header("Buttons")]
    public GameObject buttonMusic;
    public GameObject buttonSFX;

    [Header("Sprites")]
    public Sprite Off;
    public Sprite On;

    [Header("AudioSources")]
    private AudioSource audioSource;
    public AudioSource audioSourceMusic;
    public AudioSource audioSourceAmbient;

    private bool MusicOn = true;
    private bool SFXOn = true;

    public AudioClip buttonClickSound;
    // Start is called before the first frame update
    void Start()
    {
        int musicPref = PlayerPrefs.GetInt("MusicOn", 1);
        int SFXPref = PlayerPrefs.GetInt("SFXOn", 1);

        ImageMusic = buttonMusic.GetComponent<Image>();
        ImageSFX = buttonSFX.GetComponent<Image>();

        audioSource = GetComponent<AudioSource>();

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

        if (SFXPref == 1)
        {
            ImageSFX.sprite = On;
        }
        else
        {
            ImageSFX.sprite = Off;
        }
    }

    private void ChangeImage()
    {
        buttonMusic.GetComponent<Image>().sprite = ImageMusic.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
