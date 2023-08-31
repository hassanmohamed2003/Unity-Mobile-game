using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private int frameCounter = 0;
    public BuildingBlock blockPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        frameCounter++;
        if(frameCounter == 300){
            Vector2 newBlockStartPosition = new Vector2(0, 4);
            Instantiate(blockPrefab, newBlockStartPosition, Quaternion.identity);
            frameCounter = 0;
        }
    }
}
