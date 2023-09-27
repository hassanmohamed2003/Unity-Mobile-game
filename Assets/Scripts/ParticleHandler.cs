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

    public void PlayDustCloudParticles(Vector2 position)
    {
        Instantiate(ParticleDustCloud, position, Quaternion.identity);
    }

    public IEnumerator PlayHighscoreParticles()
    {
        yield return new WaitForSeconds(2);
        Instantiate(ParticleHighscore, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), Quaternion.identity);
    }
}
