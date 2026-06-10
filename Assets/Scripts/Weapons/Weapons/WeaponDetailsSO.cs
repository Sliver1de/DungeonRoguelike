using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_",menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    #region Header WEAPON BASE DETAILS

    [Space(10)]
    [Header("WEAPON BASE DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip("Weapon name")]

    #endregion

    public string weaponName;
    
    #region Tooltip
    //武器的精灵图 —— 该精灵应选择 ‘生成物理形状’ 选项
    [Tooltip("The sprite for the weapon - the sprite should have the 'generate physics shape' option selected")]

    #endregion

    public Sprite weaponSprite;
    
    #region Header WEAPON CONFIGURATION     武器配置

    [Space(10)]
    [Header("WEAPON CONFIGURATION")]

    #endregion

    #region Tooltip

    //武器射击位置 —— 从精灵枢轴点到武器末端的偏移位置
    [Tooltip("Weapon Shoot Position - the offset position for the end of the weapon from the sprite pivot pont")]

    #endregion

    public Vector3 weaponShootPosition;

    #region Tooltip

    [Tooltip("Weapon current ammo")]

    #endregion

    public AmmoDetailsSO weaponCurrentAmmo;

    #region Tooltip
    //武器射击效果 SO —— 包含粒子效果参数，用于与 weaponShootEffectPrefab 配合使用
    [Tooltip(
        "Weapon shoot effect SO - contains particle effect parameters to be used in conjunction with the weaponShootEffectPrefab")]

    #endregion

    public WeaponShootEffectSO weaponShootEffect;

    #region Tooltip
    //武器的射击音效脚本化对象（SO）
    [Tooltip("The firing sound effect SO for the weapon")]

    #endregion

    public SoundEffectSO weaponFiringSoundEffect;
    
    #region Tooltip
    //武器的射击音效脚本化对象（SO）
    [Tooltip("The reloading sound effect SO for the weapon")]

    #endregion

    public SoundEffectSO weaponReloadingSoundEffect;
    
    #region Header WEAPON OPERATING VALUES   武器配置

    [Space(10)]
    [Header("WEAPON OPERATING VALUES")]

    #endregion

    #region Tooltip
    //选择武器是否拥有无限弹药
    [Tooltip("Select if the weapon has infinite ammo")]

    #endregion

    public bool hasInfiniteAmmo = false;
    
    #region Tooltip
    //选择武器是否拥有无限弹匣容量
    [Tooltip("Select if the weapon has infinite clip capacity")]

    #endregion

    public bool hasInfiniteClipCapacity = false;
    
    #region Tooltip
    //武器容量 —— 在重新装填之前可以发射的子弹数
    [Tooltip("The Weapon capacity - shots before a reload")]

    #endregion

    public int weaponClipAmmoCapacity = 6;
    
    #region Tooltip
    //武器弹药容量 —— 该武器可携带的最大子弹数
    [Tooltip("Weapon ammo capacity - the maximum number of rounds at that can be held for this weapon")]

    #endregion

    public int weaponAmmoCapacity = 100;
    
    #region Tooltip
    //武器射击速率 —— 0.2 表示每秒 5 发射击
    [Tooltip("Weapon Fire Rate - 0.2 means 5 shots a second")]

    #endregion

    public float weaponFireRate = 0.2f;
    
    #region Tooltip
    //武器预充电时间 —— 按住开火按钮直到开火的时间（以秒为单位）
    [Tooltip("Weapon Precharge Time - time in seconds to hold fire button down before firing")]

    #endregion

    public float weaponPrechargeTime = 0f;
    
    #region Tooltip
    //这是武器重新装填的时间（以秒为单位
    [Tooltip("This is the weapon reload time in seconds")]

    #endregion

    public float weaponReloadTime = 0f;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity,
                false);
        }
    }
#endif

    #endregion
}
