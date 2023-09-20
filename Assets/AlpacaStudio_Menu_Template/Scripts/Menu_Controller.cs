using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Controller : MonoBehaviour {

	[Tooltip("_sceneToLoadOnPlay is the name of the scene that will be loaded when users click play")]
	public string _sceneToLoadOnPlay = "Game";
	[Tooltip("_webpageURL defines the URL that will be opened when users click on your branding icon")]
	public string _webpageURL = "http://www.alpaca.studio";
	[Tooltip("_soundButtons define the SoundOn[0] and SoundOff[1] Button objects.")]
	public Button[] _soundButtons;
	public AudioClip _backgroundMusic;
	[Tooltip("_audioClip defines the audio to be played on button click.")]
	public AudioClip _audioClip;
	[Tooltip("_audioSource defines the Audio Source component in this scene.")]
	public AudioSource _audioSource;
	
	//The private variable 'scene' defined below is used for example/development purposes.
	//It is used in correlation with the Escape_Menu script to return to last scene on key press.
	UnityEngine.SceneManagement.Scene scene;

    public Animation anim;

    public Animator animatorReverse;
    public Animator animator;

    public GameObject transition;
	public float transitionTime;

    void Awake () {
		Debug.Log(PlayerPrefs.GetInt("HasWatchedCutscene", 0));
		if (PlayerPrefs.GetInt("HasWatchedCutscene", 0) == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CutScene");
        }


		if(!PlayerPrefs.HasKey("_Mute")){
			PlayerPrefs.SetInt("_Mute", 0);
		}
		_audioSource.clip = _backgroundMusic;
		_audioSource.Play();
		scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		PlayerPrefs.SetString("_LastScene", scene.name.ToString());
        anim = GetComponent<Animation>();
        //Debug.Log(scene.name);
    }
	
	public void OpenWebpage () {
		_audioSource.PlayOneShot(_audioClip);
		Application.OpenURL(_webpageURL);
	}
	
	public void PlayGame () {
        transition.SetActive(true);
		IEnumerator coroutine = Transition();
		StartCoroutine(coroutine);
        _audioSource.PlayOneShot(_audioClip);
    }
	IEnumerator Transition()
	{
        animator.SetTrigger("Start");
		yield return new WaitForSeconds(transitionTime);
        PlayerPrefs.SetString("_LastScene", scene.name);
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoadOnPlay);
        animator.ResetTrigger("Start");
    }
	
	public void Mute () {
		_audioSource.PlayOneShot(_audioClip);
		_soundButtons[0].interactable = true;
		_soundButtons[1].interactable = false;
		PlayerPrefs.SetInt("_Mute", 1);
	}
	
	public void Unmute () {
		_audioSource.PlayOneShot(_audioClip);
		_soundButtons[0].interactable = false;
		_soundButtons[1].interactable = true;
		PlayerPrefs.SetInt("_Mute", 0);
	}
	
	public void QuitGame () {
		_audioSource.PlayOneShot(_audioClip);
		#if !UNITY_EDITOR
			Application.Quit();
		#endif
		
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}

	public void buttonClick()
	{
		anim.Play("click");
	}
}
