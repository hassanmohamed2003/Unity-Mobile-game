using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [Header("Stars")]
    GameObject level1StarOne;
    GameObject level1StarTwo;
    GameObject level1StarThree;

    GameObject level2StarOne;
    GameObject level2StarTwo;
    GameObject level2StarThree;

    int level1Stars;
    int level2Stars;
    // Start is called before the first frame update
    void Start()
    {
        level1Stars = PlayerPrefs.GetInt($"Level{1}Stars", 0);
        level2Stars = PlayerPrefs.GetInt($"Level{2}Stars", 0);

        Debug.Log(level1Stars);
        Debug.Log(level2Stars);
    }

    public void showStars()
    {
        if (level1Stars == 1){

        }

        switch (level1Stars)
        {
            case 1:
                level1StarOne.SetActive(true);
                break;
            case 2:
                level1StarOne.SetActive(true);
                level1StarTwo.SetActive(true);
                break;
            case 3:
                level1StarOne.SetActive(true);
                level1StarTwo.SetActive(true);
                level1StarThree.SetActive(true);
                break;
        }

        switch (level2Stars)
        {
            case 1:
                level2StarOne.SetActive(true);
                break;
            case 2:
                level2StarOne.SetActive(true);
                level2StarTwo.SetActive(true);
                break;
            case 3:
                level2StarOne.SetActive(true);
                level2StarTwo.SetActive(true);
                level2StarThree.SetActive(true);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
