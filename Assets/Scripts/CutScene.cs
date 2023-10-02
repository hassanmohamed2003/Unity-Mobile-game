using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    public string _sceneToLoadOnPlay = "Game";

    public Animator animator;

    public void CameraZoom()
    {
        animator.SetTrigger("cameraZoom");
    }

    public void Transition()
    {
        GameState.IsEndless = false;
        GameState.CurrentLevelID = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoadOnPlay);
        PlayerPrefs.SetInt("HasWatchedCutscene", 1);
        PlayerPrefs.Save();
    }
}
