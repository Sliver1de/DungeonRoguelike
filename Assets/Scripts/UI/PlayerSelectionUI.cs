using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSelectionUI : MonoBehaviour
{
    #region Tooltip
    //填充子对象 WeaponAnchorPosition/WeaponRotationPoint/Hand 上的 Sprite Renderer
    [Tooltip("Populate with the Sprite Renderer on child gameobject WeaponAnchorPosition/WeaponRotationPoint/Hand")]

    #endregion

    public SpriteRenderer PlayerHandSpriteRenderer;

    #region Tooltip
    //填充子对象 HandNoWeapon 上的 Sprite Renderer
    [Tooltip("Populate with the Sprite Renderer on child gameobject HandNoWeapon")]

    #endregion

    public SpriteRenderer PlayerHandNoWeaponSpriteRenderer;

    #region Tooltip
    //填充子对象 WeaponAnchorPosition/WeaponRotationPoint/Weapon 上的 Sprite Renderer
    [Tooltip("Populate with the Sprite Renderer on child gameobject WeaponAnchorPosition/WeaponRotationPoint/Weapon")]

    #endregion

    public SpriteRenderer playerWeaponSpriteRenderer;

    #region Tooltip
    //填充 Animator 组件
    [Tooltip("Populate with the Animator component")]

    #endregion
    
    public Animator animator;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerHandSpriteRenderer), PlayerHandSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerHandNoWeaponSpriteRenderer),
            PlayerHandNoWeaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerWeaponSpriteRenderer), playerWeaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(animator, nameof(animator), animator);
    }
#endif

    #endregion
}
