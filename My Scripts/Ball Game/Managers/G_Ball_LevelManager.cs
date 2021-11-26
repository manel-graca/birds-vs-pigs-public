using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class G_Ball_LevelManager : MonoBehaviour
{
    #region Awake and Assigner

    public static G_Ball_LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        //AssignSpawners();
        StartCoroutine(InitializeLevelRoutine());
    }

    #endregion

    [BoxGroup("Level", CenterLabel = true)]
    public bool isLevelWon;
    [BoxGroup("Level")]
    public bool isLevelLost;
    [Space(5)]
    [BoxGroup("Visual Representations", CenterLabel = true)]
    [SerializeField] private GameObject ballSprite;
    [BoxGroup("Visual Representations")]
    public GameObject spawner;
    [Space(5)]
    [BoxGroup("Visual Representations")]
    public Transform[] spawners;
    [Space(10)]
    [BoxGroup("Visual Representations"), Title("Transforms")]
    [SerializeField] Transform spawnersInstancesParent;
    [BoxGroup("Visual Representations")]
    [SerializeField] Transform ballSpritesParent;

    public LevelData levelData;

    [SerializeField] GameObject[] winPanelStars;
    [SerializeField] GameObject[] winPanelStarsParticleSystems;

    private List<GameObject> ballSprites = new List<GameObject>();
    private Queue<GameObject> ballSpriteQueue = new Queue<GameObject>();
    private Queue<GameObject> ballsQueue = new Queue<GameObject>();

    private int ballsLeft;
    private GameObject[] spawnersInstances;

    [SerializeField] GameObject tutorial;


    private int currentLevelScore;

    private float timeBetweenChecks = 0.8f;
    private float timeSinceCheck2 = Mathf.Infinity;

    private bool canAddToScore = true;


    GameObject objective;
    G_Ball_Objective objectiveHandler;
    G_Ball_UIController ui;
    G_Ball_GameManager gameManager;
    G_Ball_BallController ballController;
    PlayerPrefsManager saveManager;

    /*#region Odin Validation Methods
    [Range(1, 10)] [ValidateInput("CheckInspectorValidation", "Starting balls can NOT be higher than spawners")]
    public int startingBalls;
    private bool CheckInspectorValidation()
    {
        return startingBalls < (spawners.Length + 1);
    }
    #endregion*/

    private void CacheReferences()
    {
        ui = G_Ball_UIController.instance;
        gameManager = FindObjectOfType<G_Ball_GameManager>();
        saveManager = PlayerPrefsManager.instance;
        /*var sprts = FindObjectsOfType<G_Ball_BallSprite>();
        foreach (var sprt in sprts)
        {
            ballSprites.Add(sprt.gameObject);
        }*/

        ballController = transform.parent.GetComponentInChildren<G_Ball_BallController>();
        objective = GameObject.FindGameObjectWithTag("Objective");
        objectiveHandler = objective.GetComponent<G_Ball_Objective>();
    }

    private void Start()
    {
        ballsLeft = levelData.startingBalls;
        currentLevelScore = 0;


        //SpawnBallSprites();
        CacheReferences();

        HandleFirstGameLoaded();
        Debug.Log("LEVEL MANAGER STASRT");
        //ui.UIOnLevelStart();
    }

    IEnumerator InitializeLevelRoutine()
    {
        AssignSpawners();
        yield return null;
        SpawnBallSprites();
        yield return null;
        var sprts = FindObjectsOfType<G_Ball_BallSprite>();
        foreach (var sprt in sprts)
        {
            ballSprites.Add(sprt.gameObject);
        }
        yield return new WaitForSeconds(0.2f);
        EnqueueBalls();
        yield return null;
        ui.UIOnLevelStart();
    }
    
    private void Update()
    {
        if (gameManager.isInputBlock)
        {
            Debug.Log("INPUT BLOCK");

            return;
        }
        if (isLevelWon || isLevelLost) return;
        if (timeSinceCheck2 > 0.85f)
        {
            timeSinceCheck2 = 0;
            CheckIfLostGame();
            WinGame();
            if (GetIfCanSpawnNewBall())
            {
                ballController.SpawnBall();
                Debug.Log("SPAWNING?!");
            }
            else
            {
                Debug.Log("CANT SPAWN NEW BALL");
            }
        }
        timeSinceCheck2 += Time.deltaTime;
    }

    private void EnqueueBalls()
    {
        for (var a = ballSprites.Count - 1; a > -1; a--)
        {
            ballSpriteQueue.Enqueue(ballSprites[a].gameObject);
        }
    }

    private void AssignSpawners()
    {
        int index = 0;
        for (int i = 0; i < levelData.startingBalls; i++)
        {
            index++;
            var newSpawner = Instantiate(spawner, spawners[i].position, Quaternion.identity);
            newSpawner.transform.SetParent(spawnersInstancesParent);
            newSpawner.name = "spawner " + index;
        }
    }

    private bool SpawnBallSprites()
    {
        List<GameObject> spawnedSprites = new List<GameObject>();
        int index = 0;
        spawnersInstances = GameObject.FindGameObjectsWithTag("Spawner");

        for (int i = 0; i < spawnersInstances.Length; i++)
        {
            var newSprite = Instantiate(ballSprite, spawnersInstances[i].transform.position, Quaternion.identity);
            newSprite.transform.SetParent(ballSpritesParent, false);
            newSprite.name = "ball sprite " + index;

            spawnedSprites.Add(newSprite);
        }

        for (int i = 0; i < spawnedSprites.Count; i++)
        {
            spawnedSprites[i].GetComponent<G_Ball_BallSprite>()
                .AssignSprite(levelData.spawningBallsPrefabs[i].GetComponent<G_Ball_DefaultBall>());

            ballsQueue.Enqueue(levelData.spawningBallsPrefabs[i]);

            spawnedSprites[i].GetComponent<G_Ball_BallSprite>().SetScale(levelData.spawningBallsPrefabs[i]
                .GetComponent<G_Ball_DefaultBall>().ballType.spriteSize);
        }

        if (ballsQueue.Count <= 0)
        {
            return false;
        }
        return true;
    }

    private void HandleFirstGameLoaded()
    {
        var key = "isFirstGameplay";
        if (!PlayerPrefs.HasKey(key)) // FIRST TIME LOADED
        {
            Instantiate(tutorial);
            PlayerPrefs.SetString(key, "false");
        }
    }

    private int CalculateMaxAchievableScore()
    {
        var blocks = GameObject.FindGameObjectsWithTag("Block");
        var maxScore = 0;
        for (int i = 0; i < blocks.Length; i++)
        {
            maxScore += blocks[i].GetComponent<G_Ball_Block>().scoreToAddWhenDestroyed;
        }

        maxScore = Mathf.RoundToInt(maxScore * 1.7f);
        return maxScore;
    }

    public int GetCurrentScore()
    {
        return currentLevelScore;
    }

    public void AddToScore(int amount)
    {
        if (canAddToScore)
        {
            currentLevelScore += amount;
            ui.UpdateScoreboard(currentLevelScore);
        }
    }

    public void CheckIfLostGame()
    {
        if (isLevelWon) return;
        if (ballController.ball != null) return;
        var existingBall = GameObject.FindWithTag("Ball");
        if (existingBall != null) return;
        if (ballsLeft <= 0 && objectiveHandler.isAlive)
        {
            LoseGame();
            return;
        }

        if (ballsLeft <= 0 && GetPigsAlive() > 0)
        {
            LoseGame();
        }
    }

    public void WinGame()
    {
        if (objectiveHandler.isAlive) return;
        if (isLevelLost) return;
        if (GetIfLevelWon())
        {
            StartCoroutine(HandleRemainingBallsAtWinRoutine());
        }
    }

    private bool GetIfLevelWon()
    {
        var pigsAlive = FindObjectsOfType<G_Ball_EnemyPig>();
        return pigsAlive.Length <= 0;
    }

    private int CalculateStars(int finalScore)
    {
        var oneStar = 1;
        var twoStar = 2;
        var threeStar = 3;

        var oneStarCondition = (CalculateMaxAchievableScore() + objectiveHandler.scoreToAddWhenDestroyed) / 3;
        var twoStarCondition = (CalculateMaxAchievableScore() + objectiveHandler.scoreToAddWhenDestroyed) / 2;

        if (finalScore > twoStarCondition)
        {
            return threeStar;
        }

        if (finalScore < twoStarCondition && finalScore > oneStarCondition)
        {
            return twoStar;
        }

        if (finalScore <= oneStarCondition)
        {
            return oneStar;
        }

        return 1;
    }

    private void LoseGame()
    {
        if (isLevelWon || isLevelLost) return;
        isLevelLost = true;
        StartCoroutine(LoseGameRoutine());
    }

    public void CheckLevelState()
    {
        if (isLevelWon || isLevelLost) return;
        WinGame();
    }

    public void OnBallSpawned()
    {
        if (isLevelWon || isLevelLost) return;

        Debug.Log(ballsLeft + " balls left");

        if (ballSpriteQueue.Count <= 0)
        {
            Debug.LogError("BALL SPRITE QUEUE IS EMPTY");
            var debugErrorMessage = gameManager.debugErrorMessage;
            var error = Instantiate(debugErrorMessage);
        }

        var a = ballSpriteQueue.Dequeue();
        var aSprite = a.GetComponent<G_Ball_BallSprite>();
        a.GetComponentInChildren<SpriteRenderer>().enabled = false;
        aSprite.hasBall = false;

        Destroy(aSprite);

        ballSprites.Remove(a.gameObject);
        Destroy(a.gameObject);

        for (var i = ballSprites.Count - 1; i > -1; i--)
        {
            //if (ballSprites[i].gameObject == a) continue;
            ballSprites[i].GetComponent<G_Ball_BallSprite>().HandleMovement();
        }

        ballsLeft--;
    }

    public GameObject InstantiateBall(Vector2 spawnPos)
    {
        var o = ballsQueue.Dequeue();

        var newBallInstance = Instantiate(o, spawnPos, Quaternion.identity);
        ui.UpdateBallName(newBallInstance.GetComponent<G_Ball_DefaultBall>());
        return newBallInstance;
    }

    private int GetPigsAlive()
    {
        var pigs = FindObjectsOfType<G_Ball_EnemyPig>().Length;
        return pigs;
    }

    public bool GetIfCanSpawnNewBall()
    {
        if (isLevelWon || isLevelLost) return false;

        G_Ball_Block[] blocks = FindObjectsOfType<G_Ball_Block>();
        G_Ball_EnemyPig[] pigs = FindObjectsOfType<G_Ball_EnemyPig>();
        List<GameObject> aliveRbs = new List<GameObject>();

        if (GameObject.FindWithTag("Ball"))
        {
            return false;
        }

        foreach (var t in blocks)
        {
            aliveRbs.Add(t.gameObject);
        }

        foreach (var t in pigs)
        {
            aliveRbs.Add(t.gameObject);
        }

        foreach (var aliveRb in aliveRbs)
        {
            Rigidbody2D rb = aliveRb.GetComponent<Rigidbody2D>();
            if (rb.velocity.magnitude >= 0.15f)
            {
                Debug.Log(rb.name + " IS MOVING");
                return false;
            }
        }

        if (!objectiveHandler.isAlive && GetPigsAlive() <= 0)
        {
            return false;
        }

        if (objectiveHandler != null)
        {
            if (objectiveHandler.GetComponent<Rigidbody2D>())
            {
                if (objectiveHandler.GetComponent<Rigidbody2D>().velocity.magnitude >= 0.1f)
                {
                    Debug.Log("OBJECTIVE IS MOVING");
                    return false;
                }
            }
        }

        if (ballsLeft > 0)
        {
            return true;
        }

        return false;
    }

    bool ran = true;

    IEnumerator HandleRemainingBallsAtWinRoutine()
    {
        if (ran)
        {
            ran = false;

            G_Ball_BallSprite[] ballsLeftObj = FindObjectsOfType<G_Ball_BallSprite>();

            foreach (var ball in ballsLeftObj)
            {
                if (ball == null) continue;
                yield return new WaitForSeconds(.85f);
                if (!ball.isAddedToScore)
                {
                    ball.OnVictoryAndRemaining();

                    AddToScore(300);
                    ball.SpawnOnVictoryParticles();
                    ball.SpawnOnVictoryText();
                }

                yield return new WaitForSeconds(0.85f);
            }

            var finalScore = currentLevelScore;
            var starsEarned = CalculateStars(finalScore);

            yield return new WaitForSeconds(1f);

            StartCoroutine(WinGameRoutine(starsEarned));
        }
    }

    void OnLevelWonBehaviour()
    {
        var currentStage = levelData.stage;
        var score = GetCurrentScore();
        var stars = CalculateStars(score);

        stars = Mathf.Clamp(stars, 1, 3);

        saveManager.SaveLevelStarsEarned(levelData, stars);
        saveManager.SaveLevelHighScore(levelData, score);
        saveManager.SaveLevelIsLocked(levelData, false);
        saveManager.SaveLevelCompleted(levelData, true);

        foreach (var stage in gameManager.stages)
        {
            if (stage.stageIndex == currentStage)
            {
                Debug.Log("STAGE " + stage.stageIndex);

                foreach (var level in stage.levels)
                {
                    if (level.levelIndex == levelData.levelIndex || level.levelDataClass.completed)
                    {
                        continue;
                    }

                    if (level.levelIndex == levelData.levelIndex + 1)
                    {
                        saveManager.SaveLevelIsLocked(level, false);
                    }
                    else
                    {
                        saveManager.SaveLevelIsLocked(level, true);
                    }
                }
            }
        }

        List<LevelData> allLevels = new List<LevelData>();
        foreach (var stage in gameManager.stages)
        {
            foreach (var level in stage.levels)
            {
                allLevels.Add(level);
            }
        }

        int completedLevels = 0;
        foreach (var level in allLevels)
        {
            if (level.levelDataClass.completed)
            {
                completedLevels += 1;
            }
        }

        PlayerPrefs.SetInt("completedLevels", completedLevels);

        gameManager.canLoadNextLevel = true;
        gameManager.GetDataVersions();
    }

    IEnumerator WinGameRoutine(int starsEarned)
    {
        if (isLevelLost) yield break;

        var offColor = new Color(145, 145, 145, 99);
        var defaultColor = Color.white;

        isLevelWon = true;
        canAddToScore = false;

        ui.DisplayStarsEarnedWinPanel(winPanelStars,
            winPanelStarsParticleSystems,
            offColor,
            defaultColor,
            starsEarned);

        ui.UpdateWinPanelScoreboard(GetCurrentScore());

        yield return new WaitForSeconds(gameManager.winDelay);
        OnLevelWonBehaviour();
        ui.ShowHideWinPanel(true);
        G_Ball_EffectsManager.instance.SpawnFireworks();
    }


    IEnumerator LoseGameRoutine()
    {
        if (isLevelWon) yield break;
        ui.ShowReturnMenuVerification(false);
        ui.ShowRestartVerification(false);
        ui.UpdateLosePanelScoreboard(currentLevelScore);

        yield return new WaitForSeconds(gameManager.loseDelay);

        ui.ShowHideLosePanel(true);
    }
}