using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public string RestartScene = "Game";
    public string MenuScene = "StartMenu";
    private int stars;

    public void Setup()
    {
        gameObject.SetActive(true);
    }

    public void OnUpdateScore(int score, int stars)
    {
        this.stars = stars;
    }
    
    private void SetTimescaleZero()
    {
        /*
        Time.timeScale = 0;
        */
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(RestartScene);
        Time.timeScale = 1;
    }
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(MenuScene);
        Time.timeScale = 1;
    }

    public void GoToEndCutscene()
    {
        bool HasWatchedEndCutscene = PlayerPrefs.GetInt("HasWatchedEndCutscene", 0) == 1;
        if(GameState.CurrentLevelID == 2 && !HasWatchedEndCutscene && stars > 0)
        {
            SceneManager.LoadScene("EndCutscene");
        }        
    }
}
