using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    [Header("Particle System")]
    public GameObject ParticleDustCloud;
    public GameObject ParticleScore;
    public GameObject ParticlePerfect;
    public GameObject ParticleHighscore;
    
    public void PlayScoreParticles()
    {
        Instantiate(ParticleScore, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0)), Quaternion.identity);
    }

    public void PlayPerfectParticles()
    {
        Instantiate(ParticlePerfect, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0)), Quaternion.identity);
    }

    public void PlayDustCloudParticles(ContactPoint2D contact)
    {
        Instantiate(ParticleDustCloud, contact.point, Quaternion.identity);
    }

    public void PlayHighscoreParticlesDelayed()
    {
        StartCoroutine(PlayHighscoreParticles());
    }

    public IEnumerator PlayHighscoreParticles()
    {
        yield return new WaitForSeconds(1.5f);
        Instantiate(ParticleHighscore, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), Quaternion.identity);
    }
}
