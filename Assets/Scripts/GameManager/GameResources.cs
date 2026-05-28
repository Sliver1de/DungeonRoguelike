using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    protected GameResources() { }

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header PLAYER SELECTION

    [Space(10)]
    [Header("PLAYER SELECTION")]

    #endregion

    #region Tooltip

    [Tooltip("The playerSelection prefab")]

    #endregion
    
    public GameObject playerSelectionPrefab;

    #region Header Player
    [Space(10)]
    [Header("PLAYER")]
    #endregion

    #region Tooltip
    //玩家详情列表 - 使用 PlayerDetails 的 ScriptableObjects 填充该列表
    [Tooltip("Player details list - populate the list with the playerdetails scriptable objects")]

    #endregion
    
    public List<PlayerDetailsSO> playerDetailsList;

    #region Tooltip
    //当前玩家可编写脚本的对象 - 用于在场景之间引用当前玩家
    [Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header MUSIC

    [Space(10)]
    [Header("MUSIC")]

    #endregion

    #region Tooltip
    //填充为音乐主混音组
    [Tooltip("Populate with the music master mixer group")]

    #endregion

    public AudioMixerGroup musicMasterMixerGroup;

    #region Tooltip

    [Tooltip("Main menu music scriptable object")]

    #endregion
    
    public MusicTrackSO mainMenuMusic;

    #region Tooltip
    //音乐全量快照
    [Tooltip("music on full snapshot")]

    #endregion

    public AudioMixerSnapshot musicOnFullSnapshot;

    #region Tooltip
    //音乐低量快照
    [Tooltip("music low snapshot")]

    #endregion
    
    public AudioMixerSnapshot musicLowSnapshot;

    #region Tooltip
    //音乐关闭快照
    [Tooltip("music off snapshot")]

    #endregion
    
    public AudioMixerSnapshot musicOffSnapshot;

    #region Header SOUNDS

    [Space(10)]
    [Header("SOUNDS")]

    #endregion

    #region Tooltip

    [Tooltip("Populate with the sounds master mixer group")]

    #endregion

    public AudioMixerGroup soundsMasterMixerGroup;

    #region Tooltip

    [Tooltip("Door open close sound effect")]

    #endregion

    public SoundEffectSO doorOpenCloseSoundEffect;

    #region Tooltip

    [Tooltip("Populate with the table flip sound effect")]

    #endregion

    public SoundEffectSO tableFlip;

    #region Tooltip

    [Tooltip("Populate with the chest open sound effect")]

    #endregion

    public SoundEffectSO chestOpen;

    #region Tooltip

    [Tooltip("Populate with the health pickup sound effect")]

    #endregion

    public SoundEffectSO healthPickup;

    #region Tooltip

    [Tooltip("Populate with the weapon pickup sound effect")]

    #endregion

    public SoundEffectSO weaponPickup;

    #region Tooltip

    [Tooltip("Populate with the ammo pickup sound effect")]

    #endregion
    
    public SoundEffectSO ammoPickup;
    
    #region Header Materials

    [Space(10)]
    [Header("MATERIALS")]

    #endregion

    #region Tooltip

    [Tooltip("Dimmed Material")]

    #endregion

    public Material dimmedMaterial;
    
    #region Tooltip

    [Tooltip("Sprite-Lit-Default Material")]

    #endregion
    public Material litMaterial;
    
    #region Tooltip
    //用 可变光照着色器（Variable Lit Shader） 填充
    [Tooltip("Populate with the Variable Lit Shader")]

    #endregion

    public Shader variableLitShader;

    #region Tooltip

    [Tooltip("Populate with the Materialize Shader")]

    #endregion
    
    public Shader materializeShader;

    #region Header SPECIAL TILEMAP TILES

    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]

    #endregion

    #region Tooltip
    //敌人可以通过的碰撞瓦片
    [Tooltip("Collision tiles that the enemies can navigate to")]

    #endregion

    public TileBase[] enemyUnwalkableCollisionTileArray;

    #region Tooltip
    //敌人导航的首选路径瓦片
    [Tooltip("Preferred path tile for enemy navigation")]

    #endregion
    public TileBase preferredEnemyPathTile;

    #region Header UI

    [Space(10)]
    [Header("UI")]

    #endregion

    #region Tooltip

    [Tooltip("Populate with heart image prefab")]

    #endregion

    public GameObject heartPrefab;
    
    #region Tooltip

    [Tooltip("Populate with ammo icon prefab")]

    #endregion

    public GameObject ammoIconPrefab;

    #region Tooltip

    [Tooltip("The score prefab")]

    #endregion
    
    public GameObject scorePrefab;

    #region Header CHESTS

    [Space(10)]
    [Header("CHESTS")]

    #endregion

    #region Tooltip

    [Tooltip("Chest item prefab")]

    #endregion

    public GameObject chestItemPrefab;

    #region Tooltip

    [Tooltip("Populate with heart icon sprite")]

    #endregion

    public Sprite heartIcon;

    #region Tooltip

    [Tooltip("Populate with bullet icon sprite")]

    #endregion
    
    public Sprite bulletIcon;

    #region Header MINIMAP

    [Space(10)]
    [Header("MINIMAP")]

    #endregion
    
    #region Tooltip

    [Tooltip("Minimap skull prefab")]

    #endregion
    
    public GameObject minimapSkullPrefab;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDetailsList), playerDetailsList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(mainMenuMusic), mainMenuMusic);
        HelperUtilities.ValidateCheckNullValue(this,nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(tableFlip), tableFlip);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestOpen), chestOpen);
        HelperUtilities.ValidateCheckNullValue(this, nameof(healthPickup), healthPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoPickup), ammoPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPickup), weaponPickup);
        HelperUtilities.ValidateCheckNullValue(this,nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this,nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this,nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this,nameof(materializeShader), materializeShader);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTileArray),
            enemyUnwalkableCollisionTileArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilities.ValidateCheckNullValue(this,nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this,nameof(musicOnFullSnapshot), musicOnFullSnapshot);
        HelperUtilities.ValidateCheckNullValue(this,nameof(musicLowSnapshot), musicLowSnapshot);
        HelperUtilities.ValidateCheckNullValue(this,nameof(musicOffSnapshot), musicOffSnapshot);
        HelperUtilities.ValidateCheckNullValue(this,nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(scorePrefab), scorePrefab);
        HelperUtilities.ValidateCheckNullValue(this,nameof(chestItemPrefab), chestItemPrefab);
        HelperUtilities.ValidateCheckNullValue(this,nameof(heartIcon), heartIcon);
        HelperUtilities.ValidateCheckNullValue(this,nameof(bulletIcon), bulletIcon);
        HelperUtilities.ValidateCheckNullValue(this,nameof(minimapSkullPrefab), minimapSkullPrefab);
    }
#endif

    #endregion
}
