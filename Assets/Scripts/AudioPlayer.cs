using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip BuildingHitSound;
    public AudioClip ExplosionSound;
    public AudioClip RopeBlinkSound;
    public AudioClip RopeBreakSound;
    public AudioClip PerfectSound;
    public List<AudioClip> ComboSounds;
    public AudioClip LevelCompleteSound;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(AudioClip soundEffect)
    {
        audioSource.PlayOneShot(soundEffect);
    }
}
