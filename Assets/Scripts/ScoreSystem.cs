using UnityEngine;
using UnityEngine.Events;

public class ScoreSystem : MonoBehaviour
{
    public UnityEvent<int, int> ScoreUpdatedEvent;
    public UnityEvent NewHighScoreEvent;
    public UnityEvent NoHighscoreEvent;

    private int score;
    private int stars;
    private int highscoreAmount;
    private int firstStarRequirement;
    private int secondStarRequirement;
    private int thirdStarRequirement;
    void Start()
    {
        highscoreAmount = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void SetStarScores(LevelStructure structure)
    {
        firstStarRequirement = structure.FirstStarScoreRequirement;
        secondStarRequirement = structure.SecondStarScoreRequirement;
        thirdStarRequirement = structure.ThirdStarScoreRequirement;
    }

    public int GetStars()
    {
        if(score >= thirdStarRequirement) return 3;
        else if(score >= secondStarRequirement) return 2;
        else if(score >= firstStarRequirement) return 1;
        else return 0;
    }

    public void UpdateHighscore()
    {
        if (score > highscoreAmount)
        {
            // Save highscore
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            highscoreAmount = score;
            NewHighScoreEvent.Invoke();
        }
        else
        {
            NoHighscoreEvent.Invoke();
        }
    }

    public void SaveLevelResults(int levelID)
    {
        int stars = GetStars();
        int previousStars = PlayerPrefs.GetInt($"Level{levelID}Stars", stars);
        if(stars > previousStars)
        {
            PlayerPrefs.SetInt($"Level{levelID}Stars", stars);
        }
        PlayerPrefs.Save();
    }

    public int GetHighscore()
    {
        return highscoreAmount;
    }

    public int GetScore()
    {
        return score;
    }

    public void IncrementScore()
    {
        score++;
        stars = GetStars();
        ScoreUpdatedEvent.Invoke(score, stars);
    }
}
