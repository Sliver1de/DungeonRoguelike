using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    #region Tooltip
    //定义此房间的地下城等级，决定总共应生成多少敌人
    [Tooltip("Defines the dungeon level for this room with regard to how many enemies in total should be spawned")]

    #endregion

    public DungeonLevelSO dungeonLevel;

    #region Tooltip
    //该地下城等级此房间中最小生成的敌人数量。实际生成的数量将在最小值和最大值之间随机
    [Tooltip("The minimum number of enemies to spawn in this room for this dungeon level. " +
             "The actual number will be a random value between the minimum and maximum values")]

    #endregion
    
    public int minTotalEnemiesToSpawn;

    #region Tooltip
    //该地下城等级此房间中最大生成的敌人数量。实际生成的数量将在最小值和最大值之间随机
    [Tooltip("The maximum number of enemies to spawn in this room for this dungeon level. " +
             "The actual number will be a random value between the minimum and maximum values")]

    #endregion
    
    public int maxTotalEnemiesToSpawn;

    #region Tooltip
    //此房间在该地下城等级下同时生成的最小敌人数。实际生成的数量将在最小值和最大值之间随机选择。
    [Tooltip("The minimum number of concurrent enemies to spawn in this room for this dungeon level. " +
             "The actual number will be a random value between the minimum and maximum values")]

    #endregion

    public int minConcurrentEnemies;

    #region Tooltip
    //此房间在该地下城等级下同时生成的最大敌人数。实际生成的数量将在最小值和最大值之间随机选择。
    [Tooltip("The maximum number of concurrent enemies to spawn in this room for this dungeon level. " +
             "The actual number will be a random value between the minimum and maximum values")]

    #endregion
    
    public int maxConcurrentEnemies;

    #region Tooltip
    //此房间在该地下城等级下敌人生成的最小时间间隔（秒）。实际时间间隔将在最小值和最大值之间随机选择。
    [Tooltip("The minimum spawn interval in seconds for enemies in this room for this dungeon level. " +
             "The actual number will be a random value between the minimum and maximum values")]

    #endregion
    
    public int minSpawnInterval;

    #region Tooltip
    ////此房间在该地下城等级下敌人生成的最大时间间隔（秒）。实际时间间隔将在最小值和最大值之间随机选择。
    [Tooltip("The maximum spawn interval in seconds for enemies in this room for this dungeon level. " +
             "The actual number will be a random value between the minimum and maximum values")]

    #endregion
    
    public int maxSpawnInterval;
}
