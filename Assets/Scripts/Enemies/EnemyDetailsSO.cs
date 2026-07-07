using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header BASE ENEMY DETAILS

    [Space(10)]
    [Header("BASE ENEMY DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip("The name of the enemy")]

    #endregion

    public string enemyName;

    #region Tooltip

    [Tooltip("The prefab for the enemy")]

    #endregion

    public GameObject enemyPrefab;

    #region Tooltip
    //敌人开始追逐之前与玩家的距离
    [Tooltip("Distance to the player before enemy starts chasing")]

    #endregion

    public float chaseDistance = 50f;

    #region Header ENEMY MATERIAL

    [Space(10)]
    [Header("ENEMY MATERIAL")]

    #endregion

    #region Tooltip
    //这是敌人的标准光照着色器材质（在敌人显现后使用）
    [Tooltip("This is the standard lit shader material for the enemy (used after the enemy materializes)")]

    #endregion

    public Material enemyStandardMaterial;

    #region Header ENEMY MATERIALIZE SETTINGS
    
    [Space(10)]
    [Header("ENEMY MATERIALIZE SETTINGS")]

    #endregion

    #region Tooltip
    //敌人显现所需的时间（以秒为单位）
    [Tooltip("The time in seconds that it takes the enemy to materialize")]

    #endregion

    public float enemyMaterializeTime;

    #region Tooltip
    //敌人显现时使用的着色器
    [Tooltip("The shader to be used when the enemy materializes")]

    #endregion

    public Shader enemyMaterializeShader;

    [ColorUsage(true, true)]

    #region Tooltip

    //敌人显现时使用的颜色。这是一个HDR颜色，因此可以设置强度以产生发光/泛光效果
    [Tooltip(
        "The color to use when the enemy materializes. This is an HDR color so intensity can be set to cause glowing/ bloom")]

    #endregion

    public Color enemyMaterializeColor;

    #region Header ENEMY WEAPON SETTINGS

    [Space(10)]
    [Header("ENEMY WEAPON SETTINGS")]

    #endregion

    #region Tooltip

    //敌人的武器 —— 如果没有这选择none
    [Tooltip("The weapon for the enemy - none if the enemy doesn't have a weapon")]

    #endregion

    public WeaponDetailsSO enemyWeapon;

    #region Tooltip
    //敌人射击间隔的最小时间延迟（秒）。此值应大于 0，将在最小值和最大值之间选择一个随机值
    [Tooltip(
        "The minimum time delay interval in seconds between burst of enemy shooting. " +
        "This value should be greater than 0. A random value will be selected between the minimum value and teh maximum value")]

    #endregion

    public float firingIntervalMin = 0.1f;

    #region Tooltip
    //敌人射击间隔的最大时间延迟（秒）。将在最小值和最大值之间选择一个随机值
    [Tooltip(
        "The maximum time delay interval in seconds between burst of enemy shooting. " +
        "A random value will be selected between the minimum value and teh maximum value")]

    #endregion
    
    public float firingIntervalMax = 1f;
    
    #region Tooltip
    //敌人在射击爆发期间的最小射击持续时间。此值应大于 0，将在最小值和最大值之间选择一个随机值
    [Tooltip(
        "The minimum firing duration that the enemy shoots for during a firing burst. " +
        "This value should be greater than 0. A random value will be selected between the minimum value and teh maximum value")]

    #endregion

    public float firingDurationMin = 1f;
    
    #region Tooltip
    //敌人在射击爆发期间的最大射击持续时间。此值应大于 0，将在最小值和最大值之间选择一个随机值
    [Tooltip(
        "The maximum firing duration that the enemy shoots for during a firing burst. " +
        "A random value will be selected between the minimum value and teh maximum value")]

    #endregion
    
    public float firingDurationMax = 2f;

    #region Tooltip
    //如果需要在敌人开火之前确认与玩家的视线，那么选择此项。如果没有选择视线，则敌人将在玩家处于“射程”内时，不论是否有障碍物都会开火
    [Tooltip("Select this if line of sight is required of the player before the enemy fires. " +
             "If line of sight isn't selected the enemy will fire regardless of obstacles whenever the player is 'in range'")]

    #endregion

    public bool firingLineOfSightRequired;
    
    
    #region Header ENEMY HEALTH

    [Space(10)]
    [Header("ENEMY HEALTH")]

    #endregion

    #region Tooltip

    [Tooltip("The health of the enemy for each level")]

    #endregion

    public EnemyHealthDetails[] enemyHealthDetailsArray;

    #region Tooltip
    //选择是否在被击中后立即进入无敌状态。如果选择是，请在另一个字段中指定无敌时间（秒）
    [Tooltip(
        "Select if has immunity period immediately after being hit. If so specify the immunity time in seconds in the other field")]

    #endregion

    public bool isImmuneAfterHit = false;

    #region Tooltip

    [Tooltip("Immunity time in seconds after being hit")]

    #endregion

    public float hitImmunityTime;

    #region Tooltip
    //选择显示敌人的血条
    [Tooltip("Select to display a health bar for the enemy")]

    #endregion
    
    public bool isHealthBarDisplayed = false;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin,
            nameof(firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin,
            nameof(firingDurationMax), firingDurationMax, false);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
#endif

    #endregion
}
