using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private readonly string highScoreKey = "HighScore";
    private int highscoreAmount;
    private int firstStarRequirement;
    private int secondStarRequirement;
    private int thirdStarRequirement;
    public int Score {
        get;
        private set;
    }

    void Start()
    {
        highscoreAmount = PlayerPrefs.GetInt(highScoreKey, 0);
    }

    public void SetStarScores(int first, int second, int third)
    {
        firstStarRequirement = first;
        secondStarRequirement = second;
        thirdStarRequirement = third;
    }

    public int GetStars()
    {
        if(Score >= thirdStarRequirement) return 3;
        else if(Score >= secondStarRequirement) return 2;
        else if(Score >= firstStarRequirement) return 1;
        else return 0;
    }

    public void UpdateHighscore(int score, out bool isHighscore)
    {
        if (score > highscoreAmount)
        {
            // Save highscore
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            highscoreAmount = score;
            isHighscore = true;
        }
        else
        {
            isHighscore = false;
        }
    }

    public int GetHighscore()
    {
        return highscoreAmount;
    }

    public void IncrementScore()
    {
        Score++;
    }
}
