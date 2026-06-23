using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    #region Tooltip
    //用子对象 Weapon 游戏物体上的 SpriteRenderer 填充
    [Tooltip("Populate with the SpriteRenderer on the child Weapon gameobject")]

    #endregion

    [SerializeField]
    private SpriteRenderer weaponSpriteRenderer;
    
    #region Tooltip
    //用子对象 Weapon 游戏物体上的 PolygonCollider2D 填充
    [Tooltip("Populate with the PolygonCollider2D on the child Weapon gameobject")]

    #endregion

    [SerializeField]
    private PolygonCollider2D weaponPolygonCollider2D;
    
    #region Tooltip
    //使用 WeaponShootPosition 游戏对象上的 Transform 进行填充
    [Tooltip("Populate with the Transform on the WeaponShootPosition gameobject")]

    #endregion

    [SerializeField]
    private Transform weaponShootPositionTransform;
    
    #region Tooltip
    //使用 WeaponEffectPosition 游戏对象上的 Transform 进行填充
    [Tooltip("Populate with the Transform on the WeaponEffectPosition gameobject")]

    #endregion

    [SerializeField]
    private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent,
        SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        
        //设置当前武器的Sprite
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;
        
        //如果武器具有多边形碰撞体和精灵，则将其设置为武器精灵的物理形状
        if (weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            //获取精灵物理形状 - 这将返回精灵物理形状的点，作为 Vector2 的列表
            List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);
            
            //设置武器上的多边形碰撞器以拾取精灵的物理形状 - 将碰撞器点设置为精灵物理形状的点
            weaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();
        }
        
        //设置武器的射击位置
        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform),
            weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform),
            weaponEffectPositionTransform);
    }
#endif

    #endregion
}
