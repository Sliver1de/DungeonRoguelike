using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
    #region Tooltip
    //使用子物体 武器旋转点 的 Transform 填充（或赋值）
    [Tooltip("Populate with the Transform from the child WeaponRotationPoint gameobject")]

    #endregion

    [SerializeField]
    private Transform weaponRotationPointTransform;

    private AimWeaponEvent aimWeaponEvent;

    private void Awake()
    {
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void OnEnable()
    {
        //订阅武器瞄准事件
        aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        //取消订阅武器瞄准事件
        aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// <summary>
    /// 武器瞄准事件处理程序
    /// </summary>
    /// <param name="aimWeaponEvent"></param>
    /// <param name="aimWeaponEventArgs"></param>
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        Aim(aimWeaponEventArgs.aimDirection, aimWeaponEventArgs.aimAngle);
    }

    /// <summary>
    /// 瞄准
    /// </summary>
    /// <param name="aimDirection"></param>
    /// <param name="aimAngle"></param>
    private void Aim(AimDirection aimDirection, float aimAngle)
    {
        //设置武器变换角度
        weaponRotationPointTransform.eulerAngles = new Vector3(0f, 0f, aimAngle);
        
        //根据玩家方向翻转武器变换
        switch (aimDirection)
        {
            case AimDirection.Left:
            case AimDirection.UpLeft:
                weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
                break;
            
            case AimDirection.Up:
            case AimDirection.UpRight:
            case AimDirection.Right:
            case AimDirection.Down:
                weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0f);
                break;
        }
    }

    #region Validation

    #if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRotationPointTransform),
            weaponRotationPointTransform);
    }

    #endif

    #endregion
}
