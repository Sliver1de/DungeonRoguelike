using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header ROOM PREFAB

    [Space(10)]
    [Header("ROOM PREFAB")]

    #endregion Header ROOM PREFAB

    #region Tooltip
    //房间的游戏对象预制件（其中包含房间和环境游戏对象的所有瓷砖贴图
    [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects")]

    #endregion Tooltip

    public GameObject prefab;

    // this is used to regenerate the guid if the so is copied and the prefab is changed
    //用于在复制 so 和更改预置时重新生成引导符
    [HideInInspector] public GameObject previousPrefab;

    #region Header ROOM MUSIC

    [Space(10)]
    [Header("ROOM MUSIC")]

    #endregion

    #region Tooltip
    //当房间尚未清除敌人时的战斗音乐SO
    [Tooltip("The battle music SO when the room hasn't been cleared of enemies")]

    #endregion

    public MusicTrackSO battleMusic;

    #region Tooltip
    //当房间清除敌人后播放的环境音乐SO
    [Tooltip("Ambient music SO for when the room has been cleared of enemies")]

    #endregion
    
    public MusicTrackSO ambientMusic;

    #region Header ROOM CONFIGURATION

    [Space(10)]
    [Header("ROOM CONFIGURATION")]

    #endregion Header ROOM CONFIGURATION

    #region Tooltip
    //房间节点类型 SO。 房间节点类型与房间节点图中使用的房间节点相对应.房间节点图中只有一种走廊类型'走廊'.
    //房间模板中有 2 种走廊节点类型--CorridorNS 和 CorridorEW。
    [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph. " +
             " The exceptions being with corridors.  In the room node graph there is just one corridor type 'Corridor'. " +
             " For the room templates there are 2 corridor node types - CorridorNS and CorridorEW.")]

    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip
    //想象一个完全包围房间瓦片地图的矩形，房间的下界表示该矩形的左下角。这个位置应该从房间的瓦片地图中获取（使用坐标刷子指针来获得该左下角的瓦片地图网格位置）
    //"注意：这是本地瓦片地图位置，而不是世界坐标位置。
    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, " +
             "the room lower bounds represent the bottom left corner of that rectangle. " +
             "This should be determined from the tilemap for the room " +
             "(using the coordinate brush pointer to get the tilemap grid position for that bottom left corner" +
             " (Note: this is the local tilemap position and NOT world position")]

    #endregion Tooltip

    public Vector2Int lowerBounds;

    #region Tooltip
    //想象一个完全包围房间瓦片地图的矩形，房间的下界表示该矩形的右上角。这个位置应该从房间的瓦片地图中获取（使用坐标刷子指针来获得该左下角的瓦片地图网格位置）
    //"注意：这是本地瓦片地图位置，而不是世界坐标位置。
    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, " +
             "the room upper bounds represent the top right corner of that rectangle. " +
             "This should be determined from the tilemap for the room " +
             "(using the coordinate brush pointer to get the tilemap grid position for that top right corner " +
             "(Note: this is the local tilemap position and NOT world position")]

    #endregion Tooltip

    public Vector2Int upperBounds;

    #region Tooltip
    //一个房间最多应该有四个门口——每个方向一个。这些门口应该具有一致的 3 个瓦片开口大小，其中中间的瓦片位置为门口的坐标 '位置'。
    [Tooltip("There should be a maximum of four doorways for a room - one for each compass direction.  " +
             "These should have a consistent 3 tile opening size," +
             " with the middle tile position being the doorway coordinate 'position'")]

    #endregion Tooltip

    [SerializeField] public List<Doorway> doorwayList;

    #region Tooltip
    //每个房间在瓦片地图中的可能生成位置（用于敌人和宝箱）应添加到这个数组中
    [Tooltip("Each possible spawn position (used for enemies and chests)" +
             " for the room in tilemap coordinates should be added to this array")]

    #endregion Tooltip

    public Vector2Int[] spawnPositionArray;

    #region Header ENEMY DETAILS

    [Space(10)]
    [Header("ENEMY DETAILS")]

    #endregion

    #region Tooltip

    //填充列表，包含在该房间中按地下城等级可生成的所有敌人，以及该敌人类型的生成比例（随机）
    [Tooltip("Populate the list with all the enemies that can be spawned in this room by dungeon level, " +
             "including the ratio (random) of this enemy type that will be spawned")]

    #endregion

    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;

    #region Tooltip
    //将敌人的生成参数填充到列表中
    [Tooltip("Populate the list with the spawn parameters for the enemies")]

    #endregion

    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;

    /// <summary>
    /// Returns the list of Entrances for the room template
    /// </summary>
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validation

#if UNITY_EDITOR

    // Validate SO fields   验证 SO 文件
    private void OnValidate()
    {
        // Set unique GUID if empty or the prefab changes   如果预制件为空或发生变化，则设置唯一的 GUID
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        
        HelperUtilities.ValidateCheckNullValue(this,nameof(prefab), prefab);
        HelperUtilities.ValidateCheckNullValue(this,nameof(battleMusic), battleMusic);
        HelperUtilities.ValidateCheckNullValue(this,nameof(ambientMusic), ambientMusic);
        HelperUtilities.ValidateCheckNullValue(this,nameof(roomNodeType), roomNodeType);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);
        
        //check enemies and room spawn parameters for levels    检查敌人和房间的生成参数，针对不同的关卡
        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList),
                roomEnemySpawnParametersList);

            foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomEnemySpawnParametersList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(roomEnemySpawnParameters.dungeonLevel),
                    roomEnemySpawnParameters.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this,
                    nameof(roomEnemySpawnParameters.minTotalEnemiesToSpawn),
                    roomEnemySpawnParameters.minTotalEnemiesToSpawn,
                    nameof(roomEnemySpawnParameters.maxTotalEnemiesToSpawn),
                    roomEnemySpawnParameters.maxTotalEnemiesToSpawn, true);
                HelperUtilities.ValidateCheckPositiveRange(this, 
                    nameof(roomEnemySpawnParameters.minSpawnInterval),
                    roomEnemySpawnParameters.minSpawnInterval, 
                    nameof(roomEnemySpawnParameters.maxSpawnInterval),
                    roomEnemySpawnParameters.maxSpawnInterval, true);
                HelperUtilities.ValidateCheckPositiveRange(this, 
                    nameof(roomEnemySpawnParameters.minConcurrentEnemies),
                    roomEnemySpawnParameters.minConcurrentEnemies,
                    nameof(roomEnemySpawnParameters.maxConcurrentEnemies),
                    roomEnemySpawnParameters.maxConcurrentEnemies, false);

                bool isEnemyTypesListForDungeonLevel = false;
                
                //Validate enemy types list
                foreach (SpawnableObjectsByLevel<EnemyDetailsSO> dungeonObjectBLevel in enemiesByLevelList)
                {
                    if (dungeonObjectBLevel.dungeonLevel == roomEnemySpawnParameters.dungeonLevel &&
                        dungeonObjectBLevel.spawnableObjectRatioList.Count > 0)
                    {
                        isEnemyTypesListForDungeonLevel = true;
                    }

                    HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectBLevel.dungeonLevel),
                        dungeonObjectBLevel.dungeonLevel);

                    foreach (SpawnableObjectRatio<EnemyDetailsSO> dungeonObjectRatio in dungeonObjectBLevel
                                 .spawnableObjectRatioList)
                    {
                        HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectRatio.dungeonObject),
                            dungeonObjectRatio.dungeonObject);
                        HelperUtilities.ValidateCheckPositiveValue(this, nameof(dungeonObjectRatio.ratio),
                            dungeonObjectRatio.ratio, false);
                    }
                }
                
                if (isEnemyTypesListForDungeonLevel == false && roomEnemySpawnParameters.dungeonLevel != null)
                {
                    Debug.Log("No enemy types specified in for dungeon level " +
                              roomEnemySpawnParameters.dungeonLevel.levelName + " in gameobject" +
                              this.name.ToString());
                }
            }
        }


        // Check spawn positions populated  检查生成位置是否已填充
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

#endif

    #endregion Validation
}