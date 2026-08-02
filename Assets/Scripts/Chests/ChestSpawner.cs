using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]
    private struct RangeByLevel
    {
        public DungeonLevelSO dungeonLevel;
        [Range(0, 100)] public int min;
        [Range(0, 100)] public int max;
    }

    #region Header CHEST PREFAB

    [Space(10)]
    [Header("CHEST PREFAB")]

    #endregion

    #region Tooltip
    //填充为宝箱预制体
    [Tooltip("Populate with the chest prefab")]

    #endregion
    
    [SerializeField]
    private GameObject chestPrefab;
    
    #region Header CHEST SPAWN CHANCE
    
    [Space(10)]
    [Header("CHEST SPAWN CHANCE")]
    
    #endregion
    
    #region Tooltip
    //生成宝箱的最低概率
    [Tooltip("The minimum probability for spawning a chest")]
    
    #endregion
    
    [SerializeField] 
    [Range(0, 100)]
    private int chestSpawnChanceMin;
    
    #region Tooltip
    //生成宝箱的最高概率
    [Tooltip("The maximum probability for spawning a chest")]
    
    #endregion
    
    [SerializeField]
    [Range(0, 100)]
    private int chestSpawnChanceMax;
    
    #region Tooltip
    //你可以根据地牢等级覆盖宝箱生成概率
    [Tooltip("You can override the chest spawn chance by dungeon level")]
    
    #endregion
    
    [SerializeField] 
    private List<RangeByLevel> chestSpawnChanceByLevelList;
    

    #region Header CHEST SPAWN DETAILS
    
    [Space(10)]
    [Header("CHEST SPAWN DETAILS")]
    
    #endregion
    
    [SerializeField] 
    private ChestSpawnEvent chestSpawnEvent;
    
    [SerializeField]
    private ChestSpawnPosition chestSpawnPosition;
    
    #region Tooltip
    //要生成的最小物品数量（注意，每种类型的弹药、生命值和武器最多各生成 1 个）
    [Tooltip("The minimum number of items to spawn (note that a maximum of 1 of each type of ammo, health, and weapon will be spawned")]
    
    #endregion
    
    [SerializeField] 
    [Range(0, 3)] 
    private int numberOfItemsToSpawnMin;
    
    #region Tooltip
    //要生成的最大物品数量（注意，每种类型的弹药、生命值和武器最多各生成 1 个）
    [Tooltip("The maximum number of items to spawn (note that a maximum of 1 of each type of ammo, health, and weapon will be spawned")]
    
    #endregion
    
    [SerializeField]
    [Range(0, 3)]
    private int numberOfItemsToSpawnMax;

    #region Header CHEST CONTENT DETAILS
    
    [Space(10)]
    [Header("CHEST CONTENT DETAILS")]
    
    #endregion
    
    #region Tooltip
    //每个地下城等级生成的武器及其生成比例
    [Tooltip("The weapons to spawn for each dungeon level and their spawn ratios")]
    
    #endregion
    
    [SerializeField] 
    private List<SpawnableObjectsByLevel<WeaponDetailsSO>> weaponSpawnByLevelList;
    
    #region Tooltip
    //每个等级生成的生命值范围
    [Tooltip("The range of health to spawn for each level")]
    
    #endregion
    
    [SerializeField] 
    private List<RangeByLevel> healthSpawnByLevelList;
    
    #region Tooltip
    //每个等级生成的弹药范围
    [Tooltip("The range of ammo to spawn for each level")]
    
    #endregion 
    
    [SerializeField] 
    private List<RangeByLevel> ammoSpawnByLevelList;
    
    private bool chestSpawned = false;
    private Room chestRoom;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiseDefeated;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiseDefeated;
    }

    /// <summary>
    /// 处理房间更改事件
    /// </summary>
    /// <param name="roomChangedEventArgs"></param>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        //如果我们还没有获取到胸部所在的房间，获取它所在的房间
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }
        
        //如果宝箱是在进入房间时生成的，那么生成宝箱
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room)
        {
            SpawnChest();
        }
    }

    /// <summary>
    /// 处理房间内敌人被击败事件
    /// </summary>
    /// <param name="roomEnemiesDefeatedArgs"></param>
    private void StaticEventHandler_OnRoomEnemiseDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        //如果我们还没有获取到宝箱所在的房间，那么获取宝箱所在的房间
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }
        
        //如果宝箱是在敌人被击败后生成的，并且宝箱所在的房间是敌人已被击败的房间
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated &&
            chestRoom == roomEnemiesDefeatedArgs.room)
        {
            SpawnChest();
        }
    }

    private void SpawnChest()
    {
        chestSpawned = true;
        
        //宝箱是否根据指定的几率生成，若不满足条件则返回
        if (!RandomSpawnChest()) return;
        
        //获取要生成的弹药、健康和武器物品的数量（每种最多生成1个）
        GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum);
        
        //实例化宝箱
        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);
        
        //定位宝箱
        if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition)
        {
            chestGameObject.transform.position = this.transform.position;
        }
        else if (chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            //获取离玩家最近的生成位置
            Vector3 spawnPosition =
                HelperUtilities.GetSpawnPositionNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);
            
            //计算一些随机变动
            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            chestGameObject.transform.position = spawnPosition + variation;
        }
        
        //获取宝箱组件
        Chest chest = chestGameObject.GetComponent<Chest>();
        
        //初始化宝箱
        if (chestSpawnEvent == ChestSpawnEvent.onRoomEntry)
        {
            //不要使用物品生成的效果
            chest.Initialize(false, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum),
                GetAmmoPercentToSpawn(ammoNum));
        }
        else
        {
            //使用材质化效果
            chest.Initialize(true, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum),
                GetAmmoPercentToSpawn(ammoNum));
        }
    }

    /// <summary>
    /// 检查宝箱是否应该根据宝箱生成概率生成——如果宝箱应该生成则返回true，否则返回false
    /// </summary>
    /// <returns></returns>
    private bool RandomSpawnChest()
    {
        int chancePercent = Random.Range(chestSpawnChanceMin, chestSpawnChanceMax);
        
        //检查当前关卡是否设置了覆盖的生成概率百分比
        foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
        {
            if (rangeByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                chancePercent = Random.Range(rangeByLevel.min, rangeByLevel.max + 1);
                break;
            }
        }
        
        //获取一个介于 1 和 100 之间的随机值
        int randomPercent = Random.Range(1, 101);

        if (randomPercent <= chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 获取要生成的物品数量 - 每种物品最多生成 1 个 - 总数最多为 3 个
    /// </summary>
    /// <param name="ammo"></param>
    /// <param name="health"></param>
    /// <param name="weapons"></param>
    private void GetItemsToSpawn(out int ammo, out int health, out int weapons)
    {
        ammo = 0;
        health = 0;
        weapons = 0;

        int numberOfItemsToSpawn = Random.Range(numberOfItemsToSpawnMin, numberOfItemsToSpawnMax);

        int choice;

        if (numberOfItemsToSpawn == 1)
        {
            choice = Random.Range(0, 3);
            if (choice == 0)
            {
                weapons++;
                return;
            }

            if (choice == 1)
            {
                ammo++;
                return;
            }

            if (choice == 2)
            {
                health++;
                return;
            }
            return;
        }
        else if (numberOfItemsToSpawn == 2)
        {
            choice = Random.Range(0, 3);
            if (choice == 0)
            {
                weapons++;
                ammo++;
                return;
            }

            if (choice == 1)
            {
                ammo++;
                health++;
                return;
            }

            if (choice == 2)
            {
                health++;
                ammo++;
                return;
            }
        }
        else if (numberOfItemsToSpawn >= 3)
        {
            weapons++;
            ammo++;
            health++;
            return;
        }
    }

    /// <summary>
    /// 获取弹药生成百分比
    /// </summary>
    /// <param name="ammoNumber"></param>
    /// <returns></returns>
    private int GetAmmoPercentToSpawn(int ammoNumber)
    {
        if (ammoNumber == 0) return 0;
        
        //获取该等级的弹药生成概率范围
        foreach (RangeByLevel spawnPercentByLevel in ammoSpawnByLevelList)
        {
            if (spawnPercentByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }

    /// <summary>
    /// 获取生命生成百分比
    /// </summary>
    /// <param name="healthNumber"></param>
    /// <returns></returns>
    private int GetHealthPercentToSpawn(int healthNumber)
    {
        if (healthNumber == 0) return 0;
        
        //获取关卡对应的弹药生成百分比范围
        foreach (RangeByLevel spawnPercentByLevel in healthSpawnByLevelList)
        {
            if (spawnPercentByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }

    /// <summary>
    /// 获取要生成的武器详情——如果不生成武器或玩家已拥有该武器，则返回空
    /// </summary>
    /// <param name="weaponNumber"></param>
    /// <returns></returns>
    private WeaponDetailsSO GetWeaponDetailsToSpawn(int weaponNumber)
    {
        if (weaponNumber == 0) return null;
        
        //创建一个用于根据指定物品的相对“比率”从列表中随机选择物品的类实例
        RandomSpawnableObject<WeaponDetailsSO> weaponRandom =
            new RandomSpawnableObject<WeaponDetailsSO>(weaponSpawnByLevelList);

        WeaponDetailsSO weaponDetails = weaponRandom.GetItem();

        return weaponDetails;
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestPrefab), chestPrefab);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(chestSpawnChanceMin), chestSpawnChanceMin,
            nameof(chestSpawnChanceMax), chestSpawnChanceMax, true);

        if (chestSpawnChanceByLevelList != null && chestSpawnChanceByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(chestSpawnChanceByLevelList),
                chestSpawnChanceByLevelList);

            foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel),
                    rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min,
                    nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }

        HelperUtilities.ValidateCheckPositiveRange(this, nameof(numberOfItemsToSpawnMin), numberOfItemsToSpawnMin,
            nameof(numberOfItemsToSpawnMax), numberOfItemsToSpawnMax, true);

        if (weaponSpawnByLevelList != null && weaponSpawnByLevelList.Count > 0)
        {
            foreach (SpawnableObjectsByLevel<WeaponDetailsSO> weaponDetailsByLevel in weaponSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(weaponDetailsByLevel.dungeonLevel),
                    weaponDetailsByLevel.dungeonLevel);

                foreach (SpawnableObjectRatio<WeaponDetailsSO> weaponRatio in weaponDetailsByLevel
                             .spawnableObjectRatioList)
                {
                    HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRatio.dungeonObject),
                        weaponRatio.dungeonObject);
                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponRatio.ratio), weaponRatio.ratio,
                        true);
                }
            }
        }

        if (healthSpawnByLevelList != null && healthSpawnByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(healthSpawnByLevelList), healthSpawnByLevelList);

            foreach (RangeByLevel rangeByLevel in healthSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel),
                    rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min,
                    nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }
    }
#endif

    #endregion
}
