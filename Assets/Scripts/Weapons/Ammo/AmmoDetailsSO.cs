using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS

    [Space(10)]
    [Header("BASIC AMMO DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip("Name for the ammo")]

    #endregion

    public string ammoName;
    public bool isPlayerAmmo;
    
    
    
    #region Header AMMO SPRITE, PREFAB & MATERIALS

    [Space(10)]
    [Header("AMMO SPRITE, PREFAB & MATERIALS")]

    #endregion

    #region Tooltip

    [Tooltip("Sprite to be used for the ammo")]

    #endregion

    public Sprite ammoSprite;

    #region Tooltip
    //用将要用于弹药的预制件填充。如果指定了多个预制件，则会从数组中随机选择一个预制件。预制件可以是弹药模式——只要它符合 IFireable 接口。
    [Tooltip("Populate with the prefab to be used for the ammo. " +
             "If multiple prefabs are specified then a random prefab from the array will be selected. " +
             "The prefab can be an ammo pattern - as long as it conforms to the IFireable interface.")]

    #endregion

    public GameObject[] ammoPrefabArray;

    #region Tooltip

    [Tooltip("The material to be used for the ammo")]

    #endregion

    public Material ammoMaterial;
    
    #region Tooltip
    //如果弹药在移动前需要稍微‘充能’，则设置弹药在开火后被保持充能的时间（以秒为单位），然后释放
    [Tooltip("If the ammo should ‘charge’ briefly before moving then " +
             "set the time in seconds that the ammo is held charging after firing before release")]

    #endregion

    public float ammoChargeTime = 0.1f;
    
    #region Tooltip
    //如果弹药有充能时间，则指定在充能过程中应使用的材质来渲染弹药
    [Tooltip("If the ammo has a charge time then specify what material should be used to render the ammo while charging")]
    
    #endregion
    
    public Material ammoChargeMaterial;

    #region Header AMMO HIT EFFECT

    [Space(10)]
    [Header("AMMO HIT EFFECT")]

    #endregion

    #region Tooltip
    //定义命中效果预设参数的脚本化对象（SO）
    [Tooltip("The scriptable object that defines the parameters for the hit effect prefab")]

    #endregion

    public AmmoHitEffectSO ammoHitEffect;
    
    #region Header AMMO BASE PARAMETERS

    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]

    #endregion

    #region Tooltip

    [Tooltip("The damage each ammo deals")]

    #endregion

    public int ammoDamage = 1;
    
    #region Tooltip

    //弹药的最小速度 —— 速度将在最小值和最大值之间随机选择
    [Tooltip("The minimum speed of the ammo  - the speed will be a random value between min and max")]

    #endregion

    public float ammoSpeedMin = 20f;
    
    #region Tooltip

    //弹药的最大速度 —— 速度将在最小值和最大值之间随机选择
    [Tooltip("The minimum speed of the ammo  - the speed will be a random value between min and max")]

    #endregion

    public float ammoSpeedMax = 20f;
    
    #region Tooltip
    //弹药（或弹药模式）的射程（以 Unity 单位为单位）
    [Tooltip("The range of the ammo (or ammo pattern) in unity units")]

    #endregion

    public float ammoRange = 20f;

    #region Tooltip
    //以每秒多少度为单位的弹药喷型旋转速度
    [Tooltip("The rotation speed in degrees per second of the ammo pattern")]

    #endregion
    
    public float ammoRotationSpeed = 1f;

    
    
    #region Header AMMO SPEED DETAILS

    [Space(10)]
    [Header("AMMO SPEED DETAILS")]

    #endregion

    #region Tooltip

    //这是弹药的最小散布角度。较高的散布角度意味着较低的准确度。散布会在最小值和最大值之间随机计算
    [Tooltip("This is the minimum spread angle of the ammo. A higher spread means less accuracy. " +
             "A random spread is calculated between the min and max values")]

    #endregion

    public float ammoSpreadMin = 0f;

    #region Tooltip
    //这是弹药的最大散布角度。较高的散布角度意味着较低的准确度。散布会在最小值和最大值之间随机计算
    [Tooltip("This is the maximum spread angle of the ammo. A higher spread means less accuracy. " +
             "A random spread is calculated between the min and max values")]

    #endregion
    
    public float ammoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS

    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]

    #endregion

    #region Tooltip

    //这是每次射击生成的最小弹药数量。每次射击会在最小值和最大值之间随机生成弹药数量
    [Tooltip("This is the minimum number of ammo that are spawned per shot. " +
             "A random number of ammo are spawned between the minimum and maximum values.")]

    #endregion

    public int ammoSpawnAmountMin = 1;
    
    #region Tooltip

    //这是每次射击生成的最大弹药数量。每次射击会在最小值和最大值之间随机生成弹药数量
    [Tooltip("This is the maximum number of ammo that are spawned per shot. " +
             "A random number of ammo are spawned between the minimum and maximum values.")]

    #endregion

    public int ammoSpawnAmountMax = 1;
    
    #region Tooltip

    //最小生成间隔时间。每次生成弹药之间的时间间隔（以秒为单位）是最小值和最大值之间的随机值
    [Tooltip("Minimum spawn interval time. " +
             "The time interval in seconds between spawned ammo is a random value between the minimum and maximum values specified.")]

    #endregion

    public float ammoSpawnIntervalMin = 0f;
    
    #region Tooltip

    //最大生成间隔时间。每次生成弹药之间的时间间隔（以秒为单位）是最小值和最大值之间的随机值
    [Tooltip("Maximum spawn interval time. " +
             "The time interval in seconds between spawned ammo is a random value between the minimum and maximum values specified.")]

    #endregion

    public float ammoSpawnIntervalMax = 0f;



    #region Header AMMO TRAIL DETAILS

    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]

    #endregion

    #region Tooltip

    //如果需要弹药轨迹，请选择，否则取消选择。如果选择了，则应该填充其余的弹药轨迹值。
    [Tooltip(
        "Selected if an ammo trail is required, otherwise deselect. If selected then the rest of the anmo trail values should be populated.")]

    #endregion

    public bool isAmmoTrail = false;

    #region Tooltip

    [Tooltip("Ammo trail lifetime in seconds")]

    #endregion

    public float ammoTrailTime = 3f;
    
    #region Tooltip
    
    [Tooltip("Ammo trail material")]
    
    #endregion
    
    public Material ammoTrailMaterial;
    
    #region Tooltip

    [Tooltip("The starting width for the ammo trail")]

    #endregion

    [Range(0f, 1f)]
    public float ammoTrailStartWidth;

    #region Tooltip

    [Tooltip("The ending width for the ammo trail")]

    #endregion

    [Range(0f, 1f)]
    public float ammoTrailEndWidth;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this,nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this,nameof(ammoMaterial), ammoMaterial);

        if (ammoChargeTime > 0)
        {
            HelperUtilities.ValidateCheckNullValue(this,nameof(ammoChargeMaterial), ammoChargeMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax),
                ammoSpeedMax, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax),
                ammoSpeedMax, true);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin,
                nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin,
                nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        }

        if (isAmmoTrail)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilities.ValidateCheckNullValue(this,nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }
#endif

    #endregion
}
