using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUIHandler : MonoBehaviour
{
    [Header("Game Over Screens")]
    public GameOverScreen EndlessGameOverScreen;
    public GameOverScreen LevelGameOverScreen;

    [Header("Texts")]
    public TMP_Text TiltTutorial;
    public TMP_Text TapTutorial;
    public TMP_Text RopeTutorial;
    public TMP_Text EndScoreLevel;
    public TMP_Text EndScoreEndless;
    public TMP_Text EndStars;
    public TMP_Text EndHighScore;
    public TMP_Text CurrentScore;

    [Header("Crane")]
    public Crane crane;

    [Header("Animator")]
    public Animator Animator;
    public bool HasCompletedFirstPlay{ get; private set; }

    void Awake()
    {
        HasCompletedFirstPlay = PlayerPrefs.GetInt("HasCompletedFirstPlay", 0) == 1;
        if(!HasCompletedFirstPlay)
        {
            SetScoreTextVisibility(false);
            crane.EnableRopeBreak = false;
        }
    }

    public void SetScoreTextVisibility(bool visible)
    {
        CurrentScore.gameObject.SetActive(visible);
    }

    public void OnScoreUpdate(int score)
    {
        CurrentScore.text = $"{score}";
    }

    public void OnLevelGameOver(int score, int stars)
    {
        CurrentScore.text = "";
        EndScoreLevel.text = $"{score}";
        EndStars.text = $"{stars}";
        Animator.SetTrigger("onGameOver");
        LevelGameOverScreen.Setup();
    }

    public void OnEndlessGameOver(int score, int previousHighscore)
    {
        SetScoreTextVisibility(false);
        EndScoreEndless.text = $"{score}";
        EndHighScore.text = $"{PlayerPrefs.GetInt("HighScore", previousHighscore)}";
        Animator.SetTrigger("onGameOver");
        EndlessGameOverScreen.Setup();
    }

    public IEnumerator ShowTiltTutorial()
    {
        yield return new WaitForSeconds(2.0f);
        TiltTutorial.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        TiltTutorial.gameObject.SetActive(false);

        StartCoroutine(ShowTapTutorial());
    }

    public IEnumerator ShowTapTutorial()
    {
        yield return new WaitForSeconds(3.0f);
        TapTutorial.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        TapTutorial.gameObject.SetActive(false); 
    }

    public IEnumerator ShowRopeTutorial()
    {
        yield return new WaitForSeconds(3.0f);
        RopeTutorial.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        RopeTutorial.gameObject.SetActive(false);
        SetScoreTextVisibility(true);
        PlayerPrefs.SetInt("HasCompletedFirstPlay", 1);
        PlayerPrefs.Save();
        HasCompletedFirstPlay = true;
    }
}
