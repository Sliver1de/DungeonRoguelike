using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("OBJECT REFERENCES")]

    #endregion

    #region Tooltip

    //使用子物体 WeaponImage 上的 Image 组件进行填充
    [Tooltip("Populate with image component on the child WeaponImage gameobject")]

    #endregion

    [SerializeField]
    private Image weaponImage;
    
    #region Tooltip

    //使用子物体 AmmoHolder 的 Transform 进行填充
    [Tooltip("Populate with the Transform from the child AmmoHolder gameobject")]

    #endregion

    [SerializeField]
    private Transform ammoHolderTransform;
    
    #region Tooltip

    //使用子物体 ReloadText 上的 TextMeshPro-Text 组件进行填充
    [Tooltip("Populate with the TextMeshPro-Text component on the child ReloadText gameobject")]

    #endregion

    [SerializeField]
    private TextMeshProUGUI reloadText;
    
    #region Tooltip

    //使用子物体 AmmoRemainingText 上的 TextMeshPro-Text 组件进行填充
    [Tooltip("Populate with the TextMeshPro-Text component on the child AmmoRemainingText gameobject")]

    #endregion

    [SerializeField]
    private TextMeshProUGUI ammoRemainingText;
    
    #region Tooltip

    //使用子物体 WeaponNameText 上的 TextMeshPro-Text 组件进行填充
    [Tooltip("Populate with the TextMeshPro-Text component on the child WeaponNameText gameobject")]

    #endregion

    [SerializeField]
    private TextMeshProUGUI weaponNameText;


    #region Tooltip
    //使用子物体 ReloadBar 的 RectTransform 进行填充
    [Tooltip("Populate with the RectTransform of the child gameobject ReloadBar")]

    #endregion

    [SerializeField]
    private Transform reloadBar;
    
    #region Tooltip
    //使用子物体 BarImage 的 Image 组件进行填充
    [Tooltip("Populate with the Image component of the child gameobject BarImage")]

    #endregion

    [SerializeField]
    private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnWeaponReload;

        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnWeaponReload;

        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        //在 UI 上更新当前激活武器的状态
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    /// <summary>
    /// 在 UI 上处理设置激活武器事件
    /// </summary>
    /// <param name="setActiveWeaponEvent"></param>
    /// <param name="setActiveWeaponEventArgs"></param>
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent,
        SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    /// <summary>
    /// 在 UI 上处理武器开火事件
    /// </summary>
    /// <param name="weaponFireEvent"></param>
    /// <param name="weaponFireEventArgs"></param>
    private void WeaponFiredEvent_OnWeaponFired(WeaponFireEvent weaponFireEvent,
        WeaponFireEventArgs weaponFireEventArgs)
    {
        WeaponFired(weaponFireEventArgs.weapon);
    }

    /// <summary>
    /// 武器开火，更新 UI
    /// </summary>
    /// <param name="weapon"></param>
    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    /// <summary>
    /// 在 UI 上处理武器换弹事件
    /// </summary>
    /// <param name="reloadWeaponEvent"></param>
    /// <param name="reloadWeaponEventArgs"></param>
    private void ReloadWeaponEvent_OnWeaponReload(ReloadWeaponEvent reloadWeaponEvent,
        ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    /// <summary>
    /// 在 UI 上处理武器已换弹事件
    /// </summary>
    /// <param name="weaponReloadedEvent"></param>
    /// <param name="weaponReloadedEventArgs"></param>
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent,
        WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    /// <summary>
    /// 武器已换弹 —— 如果是当前武器，则更新 UI
    /// </summary>
    /// <param name="weapon"></param>
    private void WeaponReloaded(Weapon weapon)
    {
        //判断装弹的武器是否为当前武器
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    /// <summary>
    /// 在 UI 上设置当前激活武器
    /// </summary>
    /// <param name="weapon"></param>
    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        
        //如果武器仍在换弹中，则更新换弹进度条
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    /// <summary>
    /// 填充当前激活武器的图像
    /// </summary>
    /// <param name="weaponDetails"></param>
    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    /// <summary>
    /// 填充当前激活武器的名称
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ") " + weapon.weaponDetails.weaponName.ToUpper();
    }

    /// <summary>
    /// 更新 UI 上的剩余弹药文本
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "无限 弹药";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + " / " +
                                     weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }

    /// <summary>
    /// 更新 UI 上的弹夹图标
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            //实例化弹药图标预设
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);
            
            ammoIconList.Add(ammoIcon);
        }
    }

    /// <summary>
    /// 清除弹药图标
    /// </summary>
    private void ClearAmmoLoadedIcons()
    {
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    /// <summary>
    /// 重新装填武器 —— 更新 UI 上的换弹进度条
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
        {
            return;
        }

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    /// <summary>
    /// 武器换弹进度条动画协程
    /// </summary>
    /// <param name="currentWeapon"></param>
    /// <returns></returns>
    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        //将换弹进度条设置为红色
        barImage.color = Color.red;
        
        //动画化武器换弹进度条
        while (currentWeapon.isWeaponReloading) 
        {
            //更新换弹进度条
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;
            
            //更新进度条填充
            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    /// <summary>
    /// 初始化 UI 上的武器换弹进度条
    /// </summary>
    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();
        
        //将bar颜色设置为绿色
        barImage.color = Color.green;
        
        //设置bar的缩放比为1
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// 停止协程更新武器换弹进度条
    /// </summary>
    private void StopReloadWeaponCoroutine()
    {
        //停止 UI 上任何激活的武器换弹进度条
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    /// <summary>
    /// 更新武器换弹文本的闪烁效果
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) &&
            (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            //设置装填bar为红色
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    /// <summary>
    /// 启动协程，使换弹文本闪烁
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "装填中";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 停止换弹文本的闪烁效果
    /// </summary>
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }

    /// <summary>
    /// 停止换弹文本闪烁协程
    /// </summary>
    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }
#endif

    #endregion
}
