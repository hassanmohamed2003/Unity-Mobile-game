using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScoreSystem : MonoBehaviour
{
    public UnityEvent<int, int> ScoreUpdatedEvent;
    public UnityEvent NewHighScoreEvent;
    public UnityEvent NoHighscoreEvent;
    public UnityEvent EndScoreEvent;

    private int score;
    private int stars;
    private int highscoreAmount;
    private int levelId;
    private int firstStarRequirement;
    private int secondStarRequirement;
    private int thirdStarRequirement;
    void Start()
    {
        highscoreAmount = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void SetStarScores(LevelStructure structure)
    {
        levelId = structure.LevelID;
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
        if (score > highscoreAmount && !GameState.AnyCheatsActive())
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

    public void SaveLevelResults()
    {
        int stars = GetStars();
        int previousStars = PlayerPrefs.GetInt($"Level{GameState.CurrentLevelID}Stars", stars);
        if(stars >= previousStars)
        {
            PlayerPrefs.SetInt($"Level{GameState.CurrentLevelID}Stars", stars);
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
        if(stars == 3 && !GameState.IsEndless)
        {
            EndScoreEvent.Invoke();
        }
        ScoreUpdatedEvent.Invoke(score, stars);
    }
}
