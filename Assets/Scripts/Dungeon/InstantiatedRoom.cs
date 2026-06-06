using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    
    //使用这个二维数组存储来自瓦片地图的移动惩罚，以用于 A* 寻路
    [HideInInspector] public int[,] aStarMovementPenalty;
    
    //使用存储可移动物品的先前位置的变量
    [HideInInspector] public int[,] aStarItemObstacles;
    
    [HideInInspector] public Bounds roomColliderBounds;
    [HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>();

    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("OBJECT REFERENCES")]

    #endregion

    #region Tooltip
    //填充环境子占位符游戏对象
    [Tooltip("Populate with the environment child placeholder gameobject")]

    #endregion

    [SerializeField]
    private GameObject environmentGameObject;
    
    private BoxCollider2D boxCollider2D;
    
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>(); 
        
        //保存房间碰撞体边界
        roomColliderBounds = boxCollider2D.bounds;
        
        // if (environmentGameObject != null)
        // {
        //     Debug.Log($"Environment Layer: {LayerMask.LayerToName(environmentGameObject.layer)}");
        // }
    }

    private void Start()
    {
        //更新可移动物品障碍物数组
        UpdateMoveableObstacles();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果玩家触发了碰撞器
        if (collision.gameObject.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            //将房间设置为已访问
            this.room.isPreviouslyVisited = true;
            
            //回调房间更改事件
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }


    /// <summary>
    /// 初始化实例化的房间
    /// </summary>
    /// <param name="roomGameobject"></param>
    public void Initialise(GameObject roomGameobject)
    {
        PopulateTileMapMemberVariables(roomGameobject);

        BlockOffUnusedDoorWays();

        AddObstaclesAndPreferredPaths();

        CreateItemObstaclesArray();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    /// <summary>
    /// 填充tilemap和grid成员变量
    /// </summary>
    /// <param name="roomGameobject"></param>
    private void PopulateTileMapMemberVariables(GameObject roomGameobject)
    {
        //获取网格组件
        grid = roomGameobject.GetComponentInChildren<Grid>();
        
        //获取子物体中的瓦片地图
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.CompareTag("groundTilemap"))
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.CompareTag("decoration1Tilemap"))
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.CompareTag("decoration2Tilemap"))
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.CompareTag("frontTilemap"))
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.CompareTag("collisionTilemap"))
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.CompareTag("minimapTilemap"))
            {
                minimapTilemap = tilemap;
            }
        }
    }

    /// <summary>
    /// 堵住房间内未使用的门口
    /// </summary>
    private void BlockOffUnusedDoorWays()
    {
        //Debug.Log($"Room: {room}");
        //Debug.Log($"Doorway list: {room?.doorwayList}");
        //Debug.Log($"CollisionTilemap: {collisionTilemap}");
        
        //循环遍历所有门
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected)
            {
                continue;
            }
            
            //使用瓷砖地图上的瓷砖阻挡未连接的门口
            if (collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }

            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }

            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }

            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }

            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }

            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// 在图块地图图层上封锁门口
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            case Orientation.none:
                break;
        }
    }

    /// <summary>
    /// 水平封锁门口 - 适用于北门和南门
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;
        
        //遍历所有要复制的瓦片
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                //获取正在复制的图块的旋转
                Matrix4x4 transformMatrix =
                    tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));
                
                //复制 Tile
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));
                
                //设置复制的图块的旋转
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0),
                    transformMatrix);
            }
        }
    }

    /// <summary>
    /// 垂直阻挡门口 - 适用于东门和西门
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;
        
        //遍历所有要复制的瓦片
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                //获取待复制 Tile 的旋转
                Matrix4x4 transformMatrix =
                    tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));
                
                //复制 Tile
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));
                
                //设置被复制瓦片的旋转角度
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0),
                    transformMatrix);
            }
        }
    }

    /// <summary>
    /// 更新 A* 寻路所使用的障碍物数据
    /// </summary>
    private void AddObstaclesAndPreferredPaths()
    {
        //这个数组将被填充为墙壁障碍物
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
        
        //遍历所有网格方块
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++) 
            {
                //为网格方块设置默认移动惩罚
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;
                
                //为敌人无法行走的碰撞瓦片添加障碍物
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x,
                    y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTileArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }
                
                //为敌人添加优先路径（1 表示优先路径值，网格位置的默认值在设置中指定）
                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }
            }
        }
    }

    /// <summary>
    /// 如果这不是一个走廊房间，则添加开门
    /// </summary>
    private void AddDoorsToRooms()
    {
        //如果是走廊则返回
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;
        
        //在门口位置实例化门预设体
        foreach (Doorway doorway in room.doorwayList)
        {
            //若门预设体不为空且门是连接着的
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    //创建门, 并将房间作为父物体
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f,
                        doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    //创建门, 并将房间作为父物体
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f,
                        doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    //创建门, 并将房间作为父物体
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance,
                        doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    //创建门, 并将房间作为父物体
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition =
                        new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                
                //获取门组件
                Door doorComponent = door.GetComponent<Door>();
                
                //如果是boss房间则设置门
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;
                    
                    //锁住门以防止进入房间
                    doorComponent.LockDoor();
                    
                    //在门旁实例化骷髅图标用于小地图
                    GameObject skullIcon = Instantiate(GameResources.Instance.minimapSkullPrefab, gameObject.transform);
                    skullIcon.transform.localPosition = door.transform.localPosition;
                }
            }
        }
    }

    private void DisableCollisionTilemapRenderer()
    {
        //禁用碰撞图块地图渲染器
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    /// <summary>
    /// 禁用房间触发器碰撞器，该碰撞器用于触发玩家进入房间时的事件
    /// </summary>
    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }

    /// <summary>
    /// 启用房间触发器碰撞器，该碰撞器用于触发玩家进入房间时的事件
    /// </summary>
    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }

    public void ActivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(true);
        }
    }

    public void DeactivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 锁上房间门
    /// </summary>
    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }
        
        //禁用房间 Trigger 碰撞体
        DisableRoomCollider();
    }

    /// <summary>
    /// 解锁房间门
    /// </summary>
    public void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }

    /// <summary>
    /// 解锁房间门的协程
    /// </summary>
    /// <param name="doorUnlockDelay"></param>
    /// <returns></returns>
    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0f)
        {
            yield return new WaitForSeconds(doorUnlockDelay);
        }
        
        Door[] doorArray = GetComponentsInChildren<Door>();
        
        //触发开门
        foreach (Door door in doorArray)
        {
            door.UnlockDoor();
        }
        
        //启用房间触发碰撞体
        EnableRoomCollider();
    }

    /// <summary>
    /// 创建物品障碍数组
    /// </summary>
    private void CreateItemObstaclesArray()
    {
        //这个数组将在游戏过程中填充所有可移动的障碍物
        aStarItemObstacles = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
    }

    /// <summary>
    /// 初始化物品障碍物数组，使用默认的AStar移动惩罚值
    /// </summary>
    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++) 
            {
                //设置网格格子的默认移动惩罚值
                aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
            }
        }
    }

    /// <summary>
    /// 这是用于调试的——显示桌面障碍物的位置
    /// （必须注释掉更新房间预制件的代码）
    /// </summary>
    // private void OnDrawGizmos()
    // {
    //     for (int i = 0; i < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); i++)
    //     {
    //         for (int j = 0; j < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); j++) 
    //         {
    //             if (aStarItemObstacles[i, j] == 0)
    //             {
    //                 Vector3 worldCellPos = grid.CellToWorld(new Vector3Int(i + room.templateLowerBounds.x,
    //                     j + room.templateLowerBounds.y, 0));
    //
    //                 Gizmos.DrawWireCube(new Vector3(worldCellPos.x + 0.5f, worldCellPos.y + 0.5f, 0), Vector3.one);
    //             }
    //         }
    //     }
    // }

    /// <summary>
    /// 更新可移动障碍物数组
    /// </summary>
    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveItem moveItem in moveableItemsList)
        {
            Vector3Int colliderBoundsMin = grid.WorldToCell(moveItem.boxCollider2D.bounds.min);
            Vector3Int colliderBoundsMax = grid.WorldToCell(moveItem.boxCollider2D.bounds.max);
            
            //遍历并添加可移动物品的边界到障碍物数组
            for (int i = colliderBoundsMin.x; i <= colliderBoundsMax.x; i++)
            {
                for (int j = colliderBoundsMin.y; j <= colliderBoundsMax.y; j++)
                {
                    aStarItemObstacles[i - room.templateLowerBounds.x, j - room.templateLowerBounds.y] = 0;
                }
            }
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
    }
#endif

    #endregion
}
