using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip
    //MovementDetailsSO 脚本化对象包含了诸如速度等的移动细节
    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]

    #endregion

    [SerializeField]
    private MovementDetailsSO movementDetails;

    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1; //默认值,由敌人生成器设置
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        //创建 WaitForFixedUpdate 用于协程中
        waitForFixedUpdate = new WaitForFixedUpdate();
        
        //重置玩家参考位置
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    /// <summary>
    /// 使用AStar路径寻找来构建到玩家的路径，然后将敌人移动到路径上的每个网格位置
    /// </summary>
    private void MoveEnemy()
    {
        //移动冷却计时器
        currentEnemyPathRebuildCooldown -= Time.deltaTime;
        
        //检查与玩家的距离，判断敌人是否应该开始追击
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) <
            enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }
        
        //如果距离玩家不够近，则返回。
        if (!chasePlayer) return;
        
        //仅在特定帧处理 A* 路径重建，以在敌人之间分摊计算负载
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;
        
        //如果移动冷却计时器已达到，或者玩家已移动超过所需距离，则重新构建敌人的路径并移动敌人。
        if (currentEnemyPathRebuildCooldown <= 0 ||
            (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) >
             Settings.playerMoveDistaanceToRebuildPath))
        {
            //重置路径重建冷却计时器
            currentEnemyPathRebuildCooldown = Settings.enemyPathbuildCooldown;
            
            //重置玩家基准坐标
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
            
            //使用 AStar 寻路移动敌人 - 触发重新构建至玩家的路径
            CreatePath();
            
            //如果找到路径，则移动敌人
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    //触发空闲事件
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }
                
                //通过协程沿路径移动敌人
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    /// <summary>
    /// 协程将敌人移动到路径上的下一个位置
    /// </summary>
    /// <param name="movementSteps"></param>
    /// <returns></returns>
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();
            
            //当敌人未非常接近目标时，继续移动，接近目标时则进入下一个步骤。
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f) 
            {
                //触发移动事件
                enemy.movementToPositionEvent.CallMovementToPosition(nextPosition, transform.position, moveSpeed,
                    (nextPosition - transform.position).normalized);
                
                //使用2D物理来移动敌人，因此等待下一个FixedUpdate。
                yield return waitForFixedUpdate;
            }
            
            yield return waitForFixedUpdate;
        }
        
        //路径步骤结束 - 触发敌人闲置事件。
        enemy.idleEvent.CallIdleEvent();
    }

    /// <summary>
    /// 设置敌人路径重新计算的帧编号，以避免性能峰值
    /// </summary>
    /// <param name="updateFrameNumber"></param>
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    /// <summary>
    /// 使用 AStar 静态类为敌人创建路径
    /// </summary>
    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;
        
        //获取玩家在网格上的位置
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);
        
        //获取敌人在网格上的位置
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);
        
        //为敌人构建一条移动路径
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);
        
        //移除路径中的第一步——这是敌人当前所在的网格格子
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            //当没有路径时，触发闲置事件
            enemy.idleEvent.CallIdleEvent();
        }
    }

    /// <summary>
    /// 获取离玩家最近且不在障碍物上的位置
    /// </summary>
    /// <param name="currentRoom"></param>
    /// <returns></returns>
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle =
            Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x,
                    adjustedPlayerCellPosition.y],
                currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x,
                    adjustedPlayerCellPosition.y]);
        
        //如果玩家不在标记为障碍物的格子上，则返回该位置
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        // 找到一个周围的格子，确保它不是障碍物 - 这是因为“半碰撞”瓷砖和桌子的存在，  玩家可能站在一个标记为障碍物的格子上
        else
        {
            //清空周围位置列表
            surroundingPositionList.Clear();
            
            //填充周围位置列表——该列表将包含围绕 (0,0) 网格方块的 8 个可能的向量位置
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;
                    
                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }
            
            //遍历所有位置
            for (int i = 0; i < 8; i++)
            {
                //生成列表的随机索引
                int index = Random.Range(0, surroundingPositionList.Count);
                
                //查看选定的周围位置是否有障碍物
                try
                {
                    obstacle = Mathf.Min(
                        currentRoom.instantiatedRoom.aStarMovementPenalty[
                            adjustedPlayerCellPosition.x + surroundingPositionList[index].x,
                            adjustedPlayerCellPosition.y + surroundingPositionList[index].y],
                        currentRoom.instantiatedRoom.aStarItemObstacles[
                            adjustedPlayerCellPosition.x + surroundingPositionList[index].x,
                            adjustedPlayerCellPosition.y + surroundingPositionList[index].y]);
                    
                    //如果没有障碍物，则返回可以导航到的单元格位置
                    if (obstacle != 0)
                    {
                        return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x,
                            playerCellPosition.y + surroundingPositionList[index].y, 0);
                    }
                }
                //捕捉错误，当周围位置超出网格时
                catch
                {
                    
                }
                
                //移除带有障碍物的周围位置，以便我们可以重新尝试
                surroundingPositionList.RemoveAt(index);
            }
            
            //如果没有找到没有障碍物的单元格，围绕玩家的位置——则将敌人发送到敌人生成位置的方向
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

            // for (int i = -1; i <= 1; i++)
            // {
            //     for (int j = -1; j <= 1; j++)
            //     {
            //         if (j == 0 && i == 0) continue;
            //
            //         try
            //         {
            //             obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + i,
            //                 adjustedPlayerCellPosition.y + j];
            //             if (obstacle != 0)
            //             {
            //                 return new Vector3Int(playerCellPosition.x + i, playerCellPosition.y + j, 0);
            //             }
            //         }
            //         catch
            //         {
            //             continue;
            //         }
            //     }
            // }
            
            //没有玩家周围的非障碍物格子，直接返回玩家的位置
            return playerCellPosition;
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this,nameof(movementDetails), movementDetails);
    }
#endif

    #endregion
}
