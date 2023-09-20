using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    public string _sceneToLoadOnPlay = "StartMenu";

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraZoom()
    {
        animator.SetTrigger("cameraZoom");
    }

    public void Transition()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoadOnPlay);
        PlayerPrefs.SetInt("HasWatchedCutscene", 1);
        PlayerPrefs.Save();
    }
}
