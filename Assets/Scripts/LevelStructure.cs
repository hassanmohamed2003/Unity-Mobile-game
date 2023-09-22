using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelStructure
{
    public int LevelID;
    public int BuildingPhysicsMaterialID;
    public List<int> LevelPieceIDs;
    public int BackgroundID;
    public int FirstStarScoreRequirement;
    public int SecondStarScoreRequirement;
    public int ThirdStarScoreRequirement;

    public static LevelStructure GetLevelStructureFromAsset(TextAsset asset)
    {
        return JsonUtility.FromJson<LevelStructure>(asset.text);
    }
}


