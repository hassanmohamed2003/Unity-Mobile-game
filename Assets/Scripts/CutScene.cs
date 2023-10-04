using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    public Animator animator;

    public void CameraZoom()
    {
        animator.SetTrigger("cameraZoom");
    }

    public void StartCutsceneTransition()
    {
        GameState.IsEndless = false;
        GameState.CurrentLevelID = 0;
        SceneManager.LoadScene("Game");
        PlayerPrefs.SetInt("HasWatchedCutscene", 1);
        PlayerPrefs.Save();
    }

    public void EndCutsceneTransition()
    {
        SceneManager.LoadScene("LevelSelection");
        PlayerPrefs.SetInt("HasWatchedEndCutscene", 1);
        PlayerPrefs.Save();
    }
}
