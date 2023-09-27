using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    [Header("Perfect Precision")]
    public float PerfectPrecision;
    
    [Header("Arthur")]
    public Arthur arthur;

    [Header("Particles")]
    public ParticleHandler particlePlayer;
    
    [Header("Audio")]
    public AudioPlayer audioPlayer;

    [Header("Score System")]
    public ScoreSystem scoreSystem;
    
    private int comboCounter = 0;
    public void CheckPlacement(Collision2D collision, int score)
    {
        if (collision.rigidbody)
        {
            float landingBock = collision.otherRigidbody.transform.position.x;
            float landedBock = collision.rigidbody.transform.position.x;
            if (landedBock - landingBock > PerfectPrecision || landedBock - landingBock < -PerfectPrecision)
            {
                arthur.StopHappyAnimation();
                comboCounter = 0;
                particlePlayer.PlayScoreParticles();
            }
            else
            {
                particlePlayer.PlayPerfectParticles();
                scoreSystem.IncrementScore();
                ComboCheck();
            }
        }
        else
        {
            particlePlayer.PlayScoreParticles();
        }
    }

    private void ComboCheck()
    {
        if (comboCounter < 2)
        {
            arthur.StopHappyAnimation();
            audioPlayer.PlaySoundEffect(audioPlayer.ComboSounds[comboCounter]);
            comboCounter++;
        }
        else if(comboCounter == 2)
        {
            arthur.StartHappyAnimation();
            audioPlayer.PlaySoundEffect(audioPlayer.ComboSounds[comboCounter]);
            comboCounter = 0;
        }
    }
}
