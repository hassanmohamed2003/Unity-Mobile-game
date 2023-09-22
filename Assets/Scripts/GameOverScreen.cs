using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public string RestartScene = "Game";
    public string MenuScene = "StartMenu";
    public Game game;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup()
    {
        gameObject.SetActive(true);
    }
    
    private void SetTimescaleZero()
    {
        Time.timeScale = 0;
    }

    private void launchConfetti()
    {
        game.launchConfetti();
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
}
