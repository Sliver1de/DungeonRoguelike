using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AmmoPattern : MonoBehaviour, IFireable
{
    #region Tooltip

    [Tooltip("Populate the array with the child ammo gameobjects")]

    #endregion

    [SerializeField]
    private Ammo[] ammoArray;

    private float ammoRange;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed,
        Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        
        this.ammoSpeed = ammoSpeed;
        
        //设置开火距离
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);
        
        //设置弹药范围
        ammoRange = ammoDetails.ammoRange;
        
        //激活弹药模式游戏对象
        gameObject.SetActive(true);
        
        //遍历所有子弹药并初始化它们
        foreach (Ammo ammo in ammoArray)
        {
            ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
        }
        
        //设置弹药充能计时器——这将暂时保持弹药状态
        if (ammoDetails.ammoChargeTime > 0f)
        {
            ammoChargeTimer = ammoDetails.ammoChargeTime;
        }
        else
        {
            ammoChargeTimer = 0f;
        }
    }

    private void Update()
    {
        //子弹充能特效
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        
        //计算移动子弹的距离向量
        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;
        
        //旋转弹药
        transform.Rotate(new Vector3(0f, 0f, ammoDetails.ammoRotationSpeed * Time.deltaTime));
        
        //最大范围达到后的距离
        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0f)
        {
            DisableAmmo();
        }
    }
    
    /// <summary>
    /// 根据输入方向，调整随机散布后的弹药射击方向和角度
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="weaponAimDirectionVector"></param>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle,
        Vector3 weaponAimDirectionVector)
    {
        //计算最小值和最大值之间的随机散布角度
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);
        
        //获取随机散布，切换为 1 或 -1
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }
        
        //通过随机散布调整弹药射击角度
        fireDirectionAngle += spreadToggle * randomSpread;
        
        //设置弹药开火距离
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// <summary>
    /// 禁用弹药 —— 从而将其返回到对象池
    /// </summary>
    private void DisableAmmo()
    {
        //禁用弹药样式游戏对象
        gameObject.SetActive(false);
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoArray), ammoArray);
    }
#endif

    #endregion
}
