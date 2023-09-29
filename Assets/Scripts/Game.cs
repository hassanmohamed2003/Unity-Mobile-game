using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    public static Game instance;

    [Header("Prefabs")]
    public List<TextAsset> AvailableLevelFiles;
    public List<GameObject> AvailableBackgrounds;

    [Header("UI")]
    public LevelUIHandler LevelUI;
    
    [Header("Block System")]
    public BlockSystem blockSystem;
    public bool IsGameOver {get; private set;} = false;

    [Header("Events")]
    public UnityEvent<LevelStructure> LevelStartEvent;
    public UnityEvent LevelGameOverEvent;
    public UnityEvent EndlessStartEvent;
    public UnityEvent EndlessGameOverEvent;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(GameState.IsEndless) StartEndless();
        else StartLevel();
    }

    public void OnPieceSpawnedFirstPlay(int spawnedBlockCounter)
    {
        if(!LevelUI.HasCompletedFirstPlay)
        {
            if(spawnedBlockCounter == 1)
            {
                StartCoroutine(LevelUI.ShowTiltTutorial());
            }
            else if(spawnedBlockCounter == 3)
            {
                // crane.EnableRopeBreak = true;
                StartCoroutine(LevelUI.ShowRopeTutorial());
            }
        }
    }

    private void StartEndless()
    {
        // Set default background
        AvailableBackgrounds.ForEach(x => x.SetActive(false));
        AvailableBackgrounds[1].SetActive(true);

        // Let other systems know endless mode has been started
        EndlessStartEvent.Invoke();
    }

    private void StartLevel()
    {
        // Get the level structure from the level file
        LevelStructure structure = LevelStructure.GetLevelStructureFromAsset(AvailableLevelFiles[GameState.CurrentLevelID]);
            
        // Turn all backgrounds off, only enable correct one
        AvailableBackgrounds.ForEach(x => x.SetActive(false));
        AvailableBackgrounds[structure.BackgroundID].SetActive(true);

        // Let other systems know a level has been started
        LevelStartEvent.Invoke(structure);
    } 

    public void LevelGameOver()
    {
        IsGameOver = true;
        LevelGameOverEvent.Invoke();
    }

    private void EndlessGameOver()
    {
        IsGameOver = true;        
        EndlessGameOverEvent.Invoke();
    }

    public void BlockHitFloor()
    {
        if(!IsGameOver && blockSystem.HasFirstBlockLanded)
        {
            if(GameState.IsEndless) EndlessGameOver();
            else LevelGameOver();
        }
    }
}
