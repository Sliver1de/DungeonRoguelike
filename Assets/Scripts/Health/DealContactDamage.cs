using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    #region Header DEAL DAMAGE

    [Space(10)]
    [Header("DEAL DAMAGE")]

    #endregion

    #region Tooltip
    //造成的接触伤害（由接收者覆盖）
    [Tooltip("The contact damage to deal (is overridden by the receiver)")]

    #endregion

    [SerializeField]
    private int contactDamageAmount;

    #region Tooltip
    //指定哪些层的对象应受到接触伤害
    [Tooltip("Specify what layers objects should be on to receive contact damage")]

    #endregion

    [SerializeField]
    private LayerMask layerMask;

    private bool isColliding = false;

    /// <summary>
    /// 进入碰撞体时触发接触伤害
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果已经与某物发生碰撞，则返回
        if(isColliding) return;

        ContactDamage(collision);
    }

    /// <summary>
    /// 当停留在碰撞体内时触发接触伤害
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        //如果已经与某物发生碰撞，则返回
        if (isColliding) return;
        
        ContactDamage(collision);
    }

    private void ContactDamage(Collider2D collision)
    {
        //如果碰撞对象不在指定的层级中，则返回（使用位运算比较）
        int collisionObjectLayerMask = (1 << collision.gameObject.layer);

        if ((layerMask.value & collisionObjectLayerMask) == 0)
        {
            return;
        }
        
        //检查碰撞对象是否应受到接触伤害
        ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();

        if (receiveContactDamage != null)
        {
            isColliding = true;
            
            //在设定时间后重置接触碰撞
            Invoke("ResetContactCollision", Settings.contactDamageCollisionResetDelay);
            
            receiveContactDamage.TakeContactDamage(contactDamageAmount);
        }
    }

    /// <summary>
    /// 重置isColliding布尔值
    /// </summary>
    private void ResetContactCollision()
    {
        isColliding = false;
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif

    #endregion
}
