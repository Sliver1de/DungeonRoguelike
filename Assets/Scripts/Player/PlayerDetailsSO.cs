using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_",menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header PLAYER BASE DETAILS

    [Space(10)]
    [Header("Player Base Details")]

    #endregion

    #region Tooltip

    [Tooltip("Player character name.")]

    #endregion

    public string playerCharacterName;

    #region Tooltip
    [Tooltip("Prefab gameobject for the player.")]
    #endregion
    public GameObject playerPrefab;
    
    #region Tooltip
    [Tooltip("Player runtime animator cotroller")]
    #endregion
    public RuntimeAnimatorController runtimeAnimatorController;
    
    #region Header HEALTH

    [Space(10)]
    [Header("HEALTH")]

    #endregion

    #region Tooltip

    [Tooltip("Player starting health amount")]

    #endregion

    public int playerHealthAmount;

    #region Tooltip
    //选择是否在受到攻击后立即获得无敌时间。如果选择此项，请在另一个字段中指定无敌时间（秒）
    [Tooltip(
        "Select if has immunity period immediately after being hit. If so specify the immunity time in seconds in the other field")]

    #endregion

    public bool isImmuneAfterHit = false;

    #region Tooltip

    [Tooltip("immunity time in seconds after being hit")]

    #endregion

    public float hitImmunityTime;

    #region Header WEAPON

    [Space(10)]
    [Header("WEAPON")]

    #endregion

    #region Tooltip

    [Tooltip("Player initial starting weapon")]

    #endregion

    public WeaponDetailsSO startingWeapon;
    
    #region Tooltip

    [Tooltip("Populate with the list of starting weapons.")]

    #endregion

    public List<WeaponDetailsSO> startingWeaponList;
    
    #region Header OTHER

    [Space(10)]
    [Header("OTHER")]

    #endregion

    #region Tooltip

    [Tooltip("Player icon sprite to be used in the minimap")]

    #endregion

    public Sprite playerMiniMapIcon;
    
    #region Tooltip
    [Tooltip("Player hand sprite")]
    #endregion
    public Sprite playerHandSprite;

    #region Validation
    #if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        HelperUtilities.ValidateCheckNullValue(this,nameof(playerMiniMapIcon), playerMiniMapIcon);
        HelperUtilities.ValidateCheckNullValue(this,nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this,nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);

        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this,nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
    #endif
    #endregion
}
