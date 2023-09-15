using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public int LevelID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        LevelSelector.IsEndless = false;
		LevelSelector.CurrentLevelID = LevelID;
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game");   
    }
}
