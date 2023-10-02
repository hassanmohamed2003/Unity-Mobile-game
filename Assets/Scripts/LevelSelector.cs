using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public GameObject level1StarOne;
    public GameObject level1StarTwo;
    public GameObject level1StarThree;

    public GameObject level2StarOne;
    public GameObject level2StarTwo;
    public GameObject level2StarThree;

    public GameObject LockLevel2;
    public GameObject Level1Selector;
    public GameObject Level2Selector;

    public Button buttonLevelOne;
    public Button buttonLevelTwo;

    private int level1Stars;
    private int level2Stars;

    // Start is called before the first frame update
    void Start()
    {
        level1Stars = PlayerPrefs.GetInt($"Level{0}Stars");
        level2Stars = PlayerPrefs.GetInt($"Level{1}Stars");

        Debug.Log(level1Stars + "level1");
        Debug.Log(level2Stars + "level2");
        showStars();
        checkLevels();
    }

    public void showStars()
    {
        Image starOne1 = level1StarOne.GetComponent<Image>();
        Image starTwo1 = level1StarTwo.GetComponent<Image>();
        Image starThree1 = level1StarThree.GetComponent<Image>();


        switch (level1Stars)
        {
            case 0:
                break;
            case 1:
                starOne1.color = new Color(255, 255, 255);
                break;
            case 2:
                starOne1.color = new Color(255, 255, 255);
                starTwo1.color = new Color(255, 255, 255);
                break;
            case 3:
                starOne1.color = new Color(255, 255, 255);
                starTwo1.color = new Color(255, 255, 255);
                starThree1.color = new Color(255, 255, 255);
                break;
        }

        Image starOne2 = level2StarOne.GetComponent<Image>();
        Image starTwo2 = level2StarTwo.GetComponent<Image>();
        Image starThree2 = level2StarThree.GetComponent<Image>();

        switch (level2Stars)
        {
            case 0:
                break;
            case 1:
                starOne2.color = new Color(255, 255, 255);
                break;
            case 2:
                starOne2.color = new Color(255, 255, 255);
                starTwo2.color = new Color(255, 255, 255);
                break;
            case 3:
                starOne2.color = new Color(255, 255, 255);
                starTwo2.color = new Color(255, 255, 255);
                starThree2.color = new Color(255, 255, 255);
                break;
        }

    }

    public void checkLevels()
    {
        if(level1Stars < 2)
        {
            LockLevel2.SetActive(true);
            buttonLevelTwo.interactable = false;
            level2StarOne.SetActive(false);
            level2StarTwo.SetActive(false);
            level2StarThree.SetActive(false);
            Level2Selector.GetComponent<Image>().color = new Color(255, 255, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
