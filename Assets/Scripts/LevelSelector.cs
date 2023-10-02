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

    public GameObject level3StarOne;
    public GameObject level3StarTwo;
    public GameObject level3StarThree;

    public GameObject LockLevel2;
    public GameObject LockLevel3;
    public GameObject Level1Selector;
    public GameObject Level2Selector;
    public GameObject Level3Selector;

    public GameObject StarRequirement2;
    public GameObject StarRequirement3;

    public Button buttonLevelOne;
    public Button buttonLevelTwo;
    public Button buttonLevelThree;

    private int level1Stars;
    private int level2Stars;
    private int level3Stars;

    // Start is called before the first frame update
    void Start()
    {
        level1Stars = PlayerPrefs.GetInt($"Level{0}Stars");
        level2Stars = PlayerPrefs.GetInt($"Level{1}Stars");
        level3Stars = PlayerPrefs.GetInt($"Level{2}Stars");

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

        Image starOne3 = level3StarOne.GetComponent<Image>();
        Image starTwo3 = level3StarTwo.GetComponent<Image>();
        Image starThree3 = level3StarThree.GetComponent<Image>();

        switch (level3Stars)
        {
            case 0:
                break;
            case 1:
                starOne3.color = new Color(255, 255, 255);
                break;
            case 2:
                starOne3.color = new Color(255, 255, 255);
                starTwo3.color = new Color(255, 255, 255);
                break;
            case 3:
                starOne3.color = new Color(255, 255, 255);
                starTwo3.color = new Color(255, 255, 255);
                starThree3.color = new Color(255, 255, 255);
                break;
        }
    }

    public void checkLevels()
    {
        if(level1Stars < 2)
        {
            LockLevel2.SetActive(true);
            StarRequirement2.SetActive(true);
            buttonLevelTwo.interactable = false;
            level2StarOne.SetActive(false);
            level2StarTwo.SetActive(false);
            level2StarThree.SetActive(false);
            Level2Selector.GetComponent<Image>().color = Color.white;
        }

        if(level1Stars + level2Stars < 5)
        {
            LockLevel3.SetActive(true);
            StarRequirement3.SetActive(true);
            buttonLevelThree.interactable = false;
            level3StarOne.SetActive(false);
            level3StarTwo.SetActive(false);
            level3StarThree.SetActive(false);
            Level3Selector.GetComponent<Image>().color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
