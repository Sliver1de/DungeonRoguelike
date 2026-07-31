using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
    #region Tooltip
    //将此设置为用于实体化效果的颜色
    [Tooltip("Set this to the color to be used for the materialization effect")]

    #endregion

    [ColorUsage(false, true)]
    [SerializeField]
    private Color materializeColor;

    #region Tooltip
    //将此设置为宝箱实体化所需的时间
    [Tooltip("Set this to the time is will take to materialize the chest")]

    #endregion

    [SerializeField]
    private float materializeTime = 3f;

    #region Tooltip
    //填充为 ItemSpawnPoint 变换
    [Tooltip("Populate withItemSpawnPoint transform")]

    #endregion

    [SerializeField]
    private Transform itemSpawnPoint;

    private int healthPercent;
    private WeaponDetailsSO weaponDetails;
    private int ammoPercent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    private bool isEnabled = false;
    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameObject;
    private ChestItem chestItem;
    private TextMeshPro messageTextTMP;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
        messageTextTMP = GetComponentInChildren<TextMeshPro>();
    }

    // private void Start()
    // {
    //     Initialize(true, 50, weaponDetails, 60);
    // }

    /// <summary>
    /// 初始化宝箱，使其立即可见或逐步显现
    /// </summary>
    /// <param name="shouldMaterialize"></param>
    /// <param name="healthPercent"></param>
    /// <param name="weaponDetails"></param>
    /// <param name="ammoPercent"></param>
    public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetails, int ammoPercent)
    {
        this.healthPercent = healthPercent;
        this.weaponDetails = weaponDetails;
        this.ammoPercent = ammoPercent;

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest());
        }
        else
        {
            EnableChest();
        }
    }

    /// <summary>
    /// 使宝箱具象化
    /// </summary>
    /// <returns></returns>
    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader,
            materializeColor, materializeTime, spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest();
    }

    /// <summary>
    /// 启用宝箱
    /// </summary>
    private void EnableChest()
    {
        //Set use to enabled
        isEnabled = true;
    }

    /// <summary>
    /// 使用宝箱 - 动作将根据宝箱的状态而有所不同
    /// </summary>
    public void UseItem()
    {
        if (!isEnabled) return;

        switch (chestState)
        {
            case ChestState.closed:
                OpenChest();
                break;
            case ChestState.healthItem:
                CollectHealthItem();
                break;
            case ChestState.ammoItem:
                CollectAmmoItem();
                break;
            case ChestState.weaponItem:
                CollectWeaponItem();
                break;
            case ChestState.empty:
                return;
            default:
                return;
        }
    }

    /// <summary>
    /// 首次使用时打开宝箱
    /// </summary>
    private void OpenChest()
    {
        animator.SetBool(Settings.use, true);
        
        //宝箱打开的音效
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpen);
        
        //检查玩家是否已经拥有该武器，如果是，则将武器设置为null
        if (weaponDetails != null)
        {
            if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
            {
                weaponDetails = null;
            }
        }

        UpdateChestState();
    }

    /// <summary>
    /// 根据应该生成的物品和箱子的状态创建物品
    /// </summary>
    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            chestState = ChestState.healthItem;
            InstantiateHealthItem();
        }
        else if (ammoPercent != 0)
        {
            chestState = ChestState.ammoItem;
            InstantiateAmmoItem();
        }
        else if (weaponDetails != null)
        {
            chestState = ChestState.weaponItem;
            InstantiateWeaponItem();
        }
        else
        {
            chestState = ChestState.empty;
        }
    }

    /// <summary>
    /// 实例化一个箱子物品
    /// </summary>
    private void InstantiateItem()
    {
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);

        chestItem = chestItemGameObject.GetComponent<ChestItem>();
    }

    /// <summary>
    /// 实例化一个供玩家收集的生命物品
    /// </summary>
    private void InstantiateHealthItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.heartIcon, healthPercent.ToString() + "%", itemSpawnPoint.position,
            materializeColor);
    }

    /// <summary>
    /// 收集生命物品并将其添加到玩家的生命值中
    /// </summary>
    private void CollectHealthItem()
    {
        //检查物品是否存在并且已经物化
        if (chestItem == null || !chestItem.isItemMaterialized) return;
        
        //将生命值添加到玩家
        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent);
        
        //播放拾取音效
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickup);
        
        healthPercent = 0;
        
        Destroy(chestItemGameObject);
        
        UpdateChestState();
    }

    /// <summary>
    /// 实例化一个玩家可拾取的弹药道具
    /// </summary>
    private void InstantiateAmmoItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.bulletIcon, ammoPercent.ToString() + "%", itemSpawnPoint.position,
            materializeColor);
    }

    /// <summary>
    /// 收集一个弹药道具并将其添加到玩家当前武器的弹药中
    /// </summary>
    private void CollectAmmoItem()
    {
        //检查物品是否存在且已被具象化
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        Player player = GameManager.Instance.GetPlayer();
        
        //更新当前武器的弹药
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent);
        
        //播放拾取音效
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickup);
        
        ammoPercent = 0;
        
        Destroy(chestItemGameObject);
        
        UpdateChestState();
    }

    /// <summary>
    /// 实例化一个武器物品供玩家收集
    /// </summary>
    private void InstantiateWeaponItem()
    {
        InstantiateItem();

        chestItemGameObject.GetComponent<ChestItem>().Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName,
            itemSpawnPoint.position, materializeColor);
    }

    /// <summary>
    /// 收集武器并将其添加到玩家的武器列表中
    /// </summary>
    private void CollectWeaponItem()
    {
        //检查物品是否存在并已被具现化
        if (chestItem == null || !chestItem.isItemMaterialized) return;
        
        //如果玩家还没有这把武器，则将其添加到玩家的武器列表中
        if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
        {
            //将武器添加到玩家
            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails);
            
            //播放拾取音效
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickup);
        }
        else
        {
            //显示消息，告诉玩家他们已经拥有该武器
            StartCoroutine(DisplayMessage("WEAPON\nALREADY\nEQUIPPED", 5f));
        }

        weaponDetails = null;
        
        Destroy(chestItemGameObject);
        
        UpdateChestState();
    }

    /// <summary>
    /// 在箱子上方显示消息
    /// </summary>
    /// <param name="messageText"></param>
    /// <param name="messageDisplayTime"></param>
    /// <returns></returns>
    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
    {
        messageTextTMP.text = messageText;

        yield return new WaitForSeconds(messageDisplayTime);

        messageTextTMP.text = "";
    }
}
