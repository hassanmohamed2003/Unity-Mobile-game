using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public static bool IsEndless;
    public static int CurrentLevelID;
    public static List<GameObject> Levels = new();
}
