using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;
    
    [Header("Arthur")]
    public Arthur arthur;

    [Header("Particles")]
    public ParticleHandler ParticlePlayer;

    [Header("Prefabs")]
    public List<TextAsset> AvailableLevelFiles;
    public List<GameObject> AvailableBackgrounds;
    

    [Header("Audio")]
    public AudioPlayer AudioPlayer;

    [Header("Camera")]
    public CameraFollow cameraScript;
    public float CameraTargetHeight;
    bool CameraMovementEnabled = false;

    [Header("Crane")]
    public Crane crane;

    [Header("UI")]
    public LevelUIHandler LevelUI;
    public Transform canvas;

    [Header("Collision Handler")]
    public CollisionHandler collisionHandler;

    [Header("Checkpoint System")]
    public CheckpointSystem checkpointSystem;
    
    [Header("Score System")]
    public ScoreSystem scoreSystem;
    
    [Header("Combo System")]
    public ComboSystem comboSystem;
    
    [Header("Block System")]
    public BlockSystem blockSystem;
    private Transform highestBlock;
    private bool isGameOver = false;

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
                crane.EnableRopeBreak = true;
                StartCoroutine(LevelUI.ShowRopeTutorial());
            }
        }
    }

    private void StartEndless()
    {
        // Set default background
        AvailableBackgrounds.ForEach(x => x.SetActive(false));
        AvailableBackgrounds[1].SetActive(true);

        blockSystem.CreatePieceQueue(Enumerable.Empty<GameObject>());
        blockSystem.AddRandomPieceToQueue();
        blockSystem.AddRandomPieceToQueue();
        blockSystem.AddRandomPieceToQueue();
    }

    private void StartLevel()
    {
        // Get the level structure from the level file
        LevelStructure structure = LevelStructure.GetLevelStructureFromAsset(AvailableLevelFiles[GameState.CurrentLevelID]);
            
        // Set the star requirements
        scoreSystem.SetStarScores(structure.FirstStarScoreRequirement, structure.SecondStarScoreRequirement, structure.ThirdStarScoreRequirement);

        blockSystem.CreatePieceQueue(structure.LevelPieceIDs, structure.BuildingPhysicsMaterialID);

        // Turn all backgrounds off, only enable correct one
        AvailableBackgrounds.ForEach(x => x.SetActive(false));
        AvailableBackgrounds[structure.BackgroundID].SetActive(true);
    }

    public void SetCameraMovementEnabled(bool value)
    {
        CameraMovementEnabled = value;
    } 

    public void LevelGameOver()
    {
        isGameOver = true;
        crane.OnGameOver();      
        AudioPlayer.PlaySoundEffect(AudioPlayer.LevelCompleteSound);
        LevelUI.OnLevelGameOver(scoreSystem.Score, scoreSystem.GetStars());
    }

    private void EndlessGameOver()
    {
        isGameOver = true;
        crane.OnGameOver();
        
        AudioPlayer.PlaySoundEffect(AudioPlayer.ExplosionSound);

        // You better beat the highscore
        scoreSystem.UpdateHighscore(scoreSystem.Score, out bool hasHighscore);

        // If new highscore play effects
        if(hasHighscore)
        {
            StartCoroutine(ParticlePlayer.PlayHighscoreParticles());
            AudioPlayer.PlaySoundEffect(AudioPlayer.LevelCompleteSound);
        }
        else
        {
            arthur.StartSadAnimation();
        }

        // Update UI and show game over screen
        LevelUI.OnEndlessGameOver(scoreSystem.Score, scoreSystem.GetHighscore());
    }

    public void HasDropped(Collision2D collision)
    {
        if (!blockSystem.LandedBlocksContains(collision.otherRigidbody.gameObject))
        {
            scoreSystem.IncrementScore();
            collisionHandler.HandleCurrentBlockCollision(collision, out bool isHardImpact, out Vector2 contactPoint);
            if (isHardImpact)
            {
                AudioPlayer.PlaySoundEffect(AudioPlayer.BuildingHitSound);
                ParticlePlayer.PlayDustCloudParticles(contactPoint);
            }
            crane.HasLastPieceLanded = true;
            blockSystem.AddLandedBlock(collision.otherRigidbody.gameObject);

            checkpointSystem.FreezeCheckpointBlock(blockSystem.GetLandedBlocks());
            comboSystem.CheckPlacement(collision, scoreSystem.Score);
            LevelUI.OnScoreUpdate(scoreSystem.Score);
        }

        if (collision.gameObject.CompareTag("Landed Block"))
        {
            if (highestBlock != null)
            {
                SetCameraMovementEnabled(true);
            }
            if (highestBlock == null || collision.gameObject.transform.position.y > highestBlock.position.y)
            {

                highestBlock = collision.gameObject.transform;
            }
        }

        if (!blockSystem.FirstBlockPlaced)
        {
            blockSystem.SetFirstBlock(collision.otherRigidbody.gameObject);
        }
        else if (collision.gameObject.TryGetComponent(out Floor floor) && !isGameOver)
        {
            crane.Center();
            if(GameState.IsEndless) EndlessGameOver();
            else LevelGameOver();
        }
    }

    void LateUpdate()
    {
        if (CameraMovementEnabled)
        {
            // Get the camera's world position at the specified target height
            Vector3 cameraWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0, CameraTargetHeight, 0));

            // Check if the highest block is above the target height
            if( highestBlock.gameObject.TryGetComponent(out BuildingBlock block) && 
                (block.transform.position.y + block.ropeStartOffset) > cameraWorldPos.y)
            {
                // Calculate offset from bottom to highest block
                Vector3 bottomScreenWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0,0,0));
                float offset = block.transform.position.y + block.ropeStartOffset - bottomScreenWorldPos.y;

                // Add multiple of offset to camera's targetpos to update it
                cameraScript.targetPos.y = bottomScreenWorldPos.y + (0.5f/CameraTargetHeight * offset);

                // Update arthur's position based on camera's new position
                arthur.UpdatePosition(Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.1f)));
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (crane.IsReadyForNextPiece && !isGameOver){
            Vector2 newBlockPos = crane.transform.position;
            newBlockPos.y -= 2.0f;
            blockSystem.SpawnNextPiece(newBlockPos, Quaternion.identity, GameState.IsEndless);
        }
    }
}
