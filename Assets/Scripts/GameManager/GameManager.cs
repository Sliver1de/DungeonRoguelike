using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES

    [Space(10)]
    [Header("GAME OBJECT REFERENCES")]

    #endregion

    #region Tooltip
    //在层级中填充暂停菜单游戏对象
    [Tooltip("Populate with pause menu gameobject in hierarchy")]

    #endregion
    
    [SerializeField]
    private GameObject pauseMenu;
    
    #region Tooltip
    //在 FadeScreenUI 中填充 MessageText 的 TextMeshPro 组件
    [Tooltip("Populate with the MessageText textmeshpro component in the FadeScreenUI")]

    #endregion

    [SerializeField]
    private TextMeshProUGUI messageTextTMP;

    #region Tooltip
    //在 FadeScreenUI 中填充 FadeImage 的 CanvasGroup 组件
    [Tooltip("Populate with the FadeImage canvasgroup component in the FadeScreenUI")]

    #endregion

    [SerializeField]
    private CanvasGroup canvasGroup;

    #region Header UI

    [Space(10)]
    [Header("UI REFERENCES")]

    #endregion
    
    [SerializeField]
    private Button pauseButton;
    
    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("Dungeon Levels")]

    #endregion

    #region Tooltip
    //填充地牢关卡的可编程对象（Scriptable Objects）
    [Tooltip("Populate with the dungeon level scriptable objects")]

    #endregion

    [SerializeField]
    private List<DungeonLevelSO> dungeonLevelList;

    #region Tootip
    //填充用于测试的起始地下城级别，第一级别 = 0
    [Tooltip("Populate with the starting dungeon level for testing, first level = 0")]

    #endregion

    [SerializeField]
    private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;
    private long gameScore;
    private int scoreMultiplier;
    private InstantiatedRoom bossRoom;
    private bool isFading = false;

    protected override void Awake()
    {
        base.Awake();
        
        //设置玩家详情 —— 从主菜单保存到当前玩家的 ScriptableObject
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;
        
        //实例化玩家
        InstantiatePlayer();
    }

    /// <summary>
    /// 在场景中指定位置创建玩家
    /// </summary>
    private void InstantiatePlayer()
    {
        //实例化玩家
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);
        //Debug.Log(playerGameObject.name.ToString());
        
        //初始化玩家
        player = playerGameObject.GetComponent<Player>();
        
        player.Initialize(playerDetails);
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;

        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;
        
        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }

    /// <summary>
    /// 处理房间变化事件
    /// </summary>
    /// <param name="roomChangedEventArgs"></param>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    /// <summary>
    /// 处理房间内敌人被击败事件
    /// </summary>
    /// <param name="roomEnemiesDefeatedArgs"></param>
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    /// <summary>
    /// 处理得分事件
    /// </summary>
    /// <param name="pointsScoredArgs"></param>
    private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
    {
        //触发得分
        gameScore += pointsScoredArgs.points * scoreMultiplier;
        
        //调用得分改变事件
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    /// <summary>
    /// 处理得分倍率事件
    /// </summary>
    /// <param name="multiplierArgs"></param>
    private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
    {
        if (multiplierArgs.multiplier)
        {
            scoreMultiplier++;
        }
        else
        {
            scoreMultiplier--;
        }
        
        //限制在 1 到 30 之间
        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);
        
        //调用得分改变事件
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    /// <summary>
    /// 处理玩家被销毁事件
    /// </summary>
    /// <param name="destroyedEvent"></param>
    /// <param name="destroyedEventArgs"></param>
    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
        
        //将分数设置为0
        gameScore = 0;
        
        //将倍率设置为1
        scoreMultiplier = 1;
        
        //将屏幕设为黑色
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        pauseButton.onClick.AddListener(PauseGameMenu);
    }

    void Update()
    {
        HandleGameState();

        //用于测试重建地下城
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            gameState = GameState.gameStarted;
        }
    }

    /// <summary>
    /// 处理游戏状态
    /// </summary>
    private void HandleGameState()
    {
        //处理游戏状态
        switch (gameState)
        {
            case GameState.gameStarted:
                //进入Level1
                PlayDungeonLevel(currentDungeonLevelListIndex);
                
                gameState = GameState.playingLevel;
                
                //触发房间敌人被击败事件，因为我们从入口开始，那里没有敌人（以防有一个只有 boss 房间的关卡）
                RoomEnemiesDefeated();
                
                break;
            
            //在游戏进行时处理地下城概览地图的tap点击键
            case GameState.playingLevel:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    DisplayDungeonOverviewMap();
                }
                
                break;
            
            case GameState.engagingEnemies:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                
                break;

            //如果在地下城概览地图中，处理tap点击键释放以清除地图
            case GameState.dungeonOverviewMap:
                
                //松开Tap键
                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    //清除地下城概览地图
                    DungeonMap.Instance.ClearDungeonOverViewMap();
                }
                
                break;
            
            //在关卡进行中且未与Boss对战之前，处理地下城概览地图的点击键
            case GameState.bossStage:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    DisplayDungeonOverviewMap();
                }
                
                break;
            
            case GameState.engagingBoss:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;

            //处理关卡完成的情况
            case GameState.levelCompleted:
                
                //显示关卡完成文本
                StartCoroutine(LevelCompleted());
                break;
            
            //处理游戏胜利（仅触发一次 - 测试先前的游戏状态以进行此操作）
            case GameState.gameWon:

                if (previousGameState != GameState.gameWon)
                {
                    StartCoroutine(GameWon());
                }
                break;
            
            //处理游戏失败（仅触发一次 - 测试先前的游戏状态以进行此操作）
            case GameState.gameLost:

                if (previousGameState != GameState.gameLost)
                {
                    //如果在你被击败的同时清除关卡，防止显示消息
                    StopAllCoroutines();
                    StartCoroutine(GameLost());
                }
                break;
            
            case GameState.restartGame:

                RestartGame();
                break;
            
            //如果游戏处于暂停状态且显示暂停菜单，再次按下 Esc 键将关闭暂停菜单
            case GameState.gamePaused:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;
        }
    }

    /// <summary>
    /// 设置玩家当前所在的房间
    /// </summary>
    /// <param name="room"></param>
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
        
        //debug
        //Debug.Log(room.prefab.name.ToString());
    }

    /// <summary>
    /// 房间敌人被击败 - 检测是否所有地牢房间的敌人都已清除 - 如果是，则加载下一个地牢游戏关卡
    /// </summary>
    private void RoomEnemiesDefeated()
    {
        //将地牢初始化为已清除状态，但随后测试每个房间
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;
        
        //循环遍历所有地牢房间，以检查是否清除了敌人
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            //暂时跳过Boss房间
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }
            
            //检查其他房间是否已清除敌人
            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }
        
        //Set game state
        //如果地牢等级已完全清除（即地牢已清除，仅剩Boss，并且没有Boss房间，或者地牢已清除，仅剩Boss，并且Boss房间也已清除）
        if ((isDungeonClearOfRegularEnemies && bossRoom == null) ||
            (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            //是否还有更多的地下城关卡？
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        //否则，如果地下城关卡除了 boss 房间外都已清除
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }
    }

    /// <summary>
    /// 暂停游戏菜单——也可从暂停菜单中的“继续游戏”按钮调用
    /// </summary>
    public void PauseGameMenu()
    {
        if (gameState != GameState.gamePaused)
        {
            pauseMenu.SetActive(true);
            GetPlayer().playerControl.DisablePlayer();
            
            //设置游戏状态
            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        else if (gameState == GameState.gamePaused)
        {
            pauseMenu.SetActive(false);
            GetPlayer().playerControl.EnablePlayer();
            
            //设置游戏状态
            gameState = previousGameState;
            previousGameState = GameState.gamePaused;
        }
        
        pauseButton.interactable = false; // 先禁用
        pauseButton.interactable = true;  // 再启用
    }

    /// <summary>
    /// 进入 Boss 关卡
    /// </summary>
    /// <returns></returns>
    private IEnumerator BossStage()
    {
        //激活 boss 房间
        bossRoom.gameObject.SetActive(true);
        
        //解锁 Boss 房间
        bossRoom.UnlockDoors(0f);
        
        //等待2s
        yield return new WaitForSeconds(2f);
        
        //淡入画布以显示文本消息
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        
        //Display boss message
        // yield return StartCoroutine(
        //     DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayer.playerName +
        //                           "! YOU'VE SURVIVED ....SO FAR\n\nNOW FIND DEFEAT THE BOSS ....GOOD LUCK!",
        //         Color.white, 5f));
        
        //显示 Boss 信息
        yield return StartCoroutine(
            DisplayMessageRoutine(
                "干得好，" + GameResources.Instance.currentPlayer.playerName + "！你已经活下来...到目前为止\n\n现在去找到并击败boss...祝你好运！",
                Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        //Debug.Log("Boss stage - find and destroy the boss");
    }

    /// <summary>
    /// 显示关卡已完成，并加载下一个关卡
    /// </summary>
    /// <returns></returns>
    private IEnumerator LevelCompleted()
    {
        //进入下一个Level
        gameState = GameState.playingLevel;
        
        //等待2s
        yield return new WaitForSeconds(2f);
        
        //Debug.Log("Level Completed - Press Return To Process To The Next Level");
        
        //淡入画布以显示文本消息
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        
        //Display level completed
        // yield return StartCoroutine(DisplayMessageRoutine(
        //     "WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! \n\nYOU'VE SURVIVED THIS DUNGEON LEVEL",
        //     Color.white, 5f));
        
        yield return StartCoroutine(DisplayMessageRoutine(
            "干得好，" + GameResources.Instance.currentPlayer.playerName + "! \n\n你已经成功通过了这个地下城关卡！",
            Color.white, 5f));

        // yield return StartCoroutine(DisplayMessageRoutine(
        //     "COLLECT ANY LOOT ....THEN PRESS RETURN\n\nTO DESCEND FURTHER INTO THE DUNGEON", Color.white, 5f));
        
        yield return StartCoroutine(DisplayMessageRoutine(
            "收集任何战利品……然后按返回键\n\n深入地下城更深层", Color.white, 5f));
        
        //画布淡出
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        
        //当玩家按下回车键时，处理进入下一个关卡
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }
        
        //为了避免中心被检测两次
        yield return null;
        
        //将索引增加到下一个关卡
        currentDungeonLevelListIndex++;
        
        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    /// <summary>
    /// 淡入淡出画布组
    /// </summary>
    /// <param name="startFadeAlpha"></param>
    /// <param name="targetFadeAlpha"></param>
    /// <param name="fadeSeconds"></param>
    /// <param name="backgroundColor"></param>
    /// <returns></returns>
    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        isFading = true;
        
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
        
        isFading = false;
    }

    /// <summary>
    /// 游戏获胜
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;
        
        //Debug.Log("Game Won - All levels completed and bossed defeated. Game will restart in 10 seconds.");
        
        //Wait 10 seconds
        // yield return new WaitForSeconds(10f);
        
        //禁用玩家
        GetPlayer().playerControl.DisablePlayer();
        
        //Get rank
        int rank = HighScoreManager.Instance.GetRank(gameScore);

        string rankText;
        
        //测试该分数是否在排行榜内
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            // rankText = "YOUR SCORE IS RANKED" + rank.ToString("#0") + " IN THE TOP " +
            //            Settings.numberOfHighScoresToSave.ToString("#0");
            
            rankText = "您的得分排名是" + rank.ToString("#0") + "，位于前 " +
                       Settings.numberOfHighScoresToSave.ToString("#0") + " 名之内";

            string name = GameResources.Instance.currentPlayer.playerName;

            if (name == "")
            {
                name = playerDetails.playerCharacterName.ToUpper();
            }
            
            //更新分数
            HighScoreManager.Instance.AddScore(new Score()
            {
                playerName = name,
                levelDescription = "等级 " + (currentDungeonLevelListIndex + 1).ToString() + " - " +
                                   GetCurrentDungeonLevel().levelName.ToUpper(),
                playerScore = gameScore
            }, rank);
        }
        else
        {
            // rankText = "YOU SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
            rankText = "您的得分未进入前 " + Settings.numberOfHighScoresToSave.ToString("#0") + " 名";
        }

        yield return new WaitForSeconds(1f);

        //淡出
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));
        
        //Display game won
        // yield return StartCoroutine(DisplayMessageRoutine(
        //     "WELL DONE" + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE DEFEATED THE DUNGEON",
        //     Color.white, 3f));
        
        yield return StartCoroutine(DisplayMessageRoutine(
            "干得好，" + GameResources.Instance.currentPlayer.playerName + "！你已经通关了地下城",
            Color.white, 3f));

        // yield return StartCoroutine(DisplayMessageRoutine(
        //     "YOUR SCORE " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white,
        //     4f));
        
        yield return StartCoroutine(DisplayMessageRoutine("你的得分 " + gameScore.ToString("###,###0"), Color.white,
            4f));

        // yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));
        
        yield return StartCoroutine(DisplayMessageRoutine("按下回车重新开始游戏", Color.white, 0f));
        
        //将游戏状态设置为重启游戏
        gameState = GameState.restartGame;
    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;
        
        // Debug.Log("Game Lost - Bad luck!. Game will restart in 10 seconds.");
        
        //Wait 10 seconds
        // yield return new WaitForSeconds(10f);
        
        //禁用玩家
        GetPlayer().playerControl.DisablePlayer();
        
        //获取排名
        int rank = HighScoreManager.Instance.GetRank(gameScore);
        string rankText;
        
        //测试该分数是否在排行榜内
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            // rankText = "YOUR SCORE IS RANKED" + rank.ToString("#0") + " IN THE TOP " +
            //            Settings.numberOfHighScoresToSave.ToString("#0");
            
            rankText = "您的得分排名是" + rank.ToString("#0") + "，位于前 " +
                       Settings.numberOfHighScoresToSave.ToString("#0") + " 名之内";


            string name = GameResources.Instance.currentPlayer.playerName;

            if (name == "")
            {
                name = playerDetails.playerCharacterName.ToUpper();
            }
            
            //更新得分
            HighScoreManager.Instance.AddScore(new Score()
            {
                playerName = name,
                levelDescription = "等级 " + (currentDungeonLevelListIndex + 1).ToString() + " - " +
                                   GetCurrentDungeonLevel().levelName.ToUpper(),
                playerScore = gameScore
            }, rank);
        }
        else
        {
            // rankText = "YOU SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
            rankText = "您的得分未进入前 " + Settings.numberOfHighScoresToSave.ToString("#0") + " 名";
        }
        
        //等待1s
        yield return new WaitForSeconds(1f);
        
        //淡出
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));
        
        //禁用敌人（FindObjectOfType 比较消耗资源 —— 但在游戏结束这种情况下使用是可以的）
        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }
        
        //Display game lost
        // yield return StartCoroutine(DisplayMessageRoutine(
        //     "BAD LUCK " + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE SUCCUMBED TO THE DUNGEON",
        //     Color.white, 2f));
        
        yield return StartCoroutine(DisplayMessageRoutine(
            "很遗憾 " + GameResources.Instance.currentPlayer.playerName + "！你已在地下城中陨命",
            Color.white, 2f));

        // yield return StartCoroutine(DisplayMessageRoutine(
        //     "YOUR SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white,
        //     4f));
        
        yield return StartCoroutine(DisplayMessageRoutine("你的得分 " + gameScore.ToString("###,###0"), Color.white,
            4f));
        
        // yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART GAME", Color.white, 0f));
        
        yield return StartCoroutine(DisplayMessageRoutine("按回车重新开始游戏", Color.white, 0f));

        //将游戏状态设置为重启游戏
        gameState = GameState.restartGame;
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    private void RestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// 地下城地图屏幕显示
    /// </summary>
    private void DisplayDungeonOverviewMap()
    {
        //如果正在渐变，则返回
        if (isFading) return;
        
        //显示地下城概览地图
        DungeonMap.Instance.DisplayDungeonOverViewMap();
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        //构建第一关地下城
        bool dungeonBuiltSuccessfully =
            DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }
        
        //调用静态事件，表示房间已更改
        StaticEventHandler.CallRoomChangedEvent(currentRoom);
        
        //将玩家大致设置在房间中间
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);
        
        //获取房间中最靠近玩家的最近生成点
        player.gameObject.transform.position =
            HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
        
        //显示地下城层数文本
        StartCoroutine(DisplayDungeonLevelText());
        
        //** Demo code
        // RoomEnemiesDefeated();
    }

    /// <summary>
    /// 显示地牢关卡文本
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayDungeonLevelText()
    {
        //设置屏幕为黑色
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
        
        GetPlayer().playerControl.DisablePlayer();

        string messageText = "等级 " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" +
                             dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper();
        
        yield return StartCoroutine(DisplayMessageRoutine(messageText,Color.white, 2f));
        
        GetPlayer().playerControl.EnablePlayer();
        
        //淡入
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    /// <summary>
    /// 显示消息文本，持续 displaySeconds 秒。如果 displaySeconds = 0，则消息会一直显示，直到按下回车键。
    /// </summary>
    /// <param name="text"></param>
    /// <param name="textColor"></param>
    /// <param name="displaySeconds"></param>
    /// <returns></returns>
    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        //设置文本
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;
        
        //按指定时间显示消息
        if (displaySeconds > 0)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        //否则显示消息，直到按下回车键
        else
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }
        
        yield return null;
        
        //清除文本
        messageTextTMP.SetText("");
    }

    /// <summary>
    /// 获取玩家
    /// </summary>
    /// <returns></returns>
    public Player GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// 获取玩家小地图图标
    /// </summary>
    /// <returns></returns>
    public Sprite GetPlayerMiniMapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }

    /// <summary>
    /// 获取玩家当前所在的房间
    /// </summary>
    /// <returns></returns>
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    /// <summary>
    /// 获取当前地下城层数
    /// </summary>
    /// <returns></returns>
    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
    }

    #region Validation
    #if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(pauseMenu), pauseMenu);
        HelperUtilities.ValidateCheckNullValue(this,nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this,nameof(canvasGroup), canvasGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}
