using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Table : MonoBehaviour, IUseable
{
    #region Tooltip
    //桌子的质量，用于控制其被推动时的移动速度
    [Tooltip("The mass of the table to control the speed that it moves when pushed")]

    #endregion

    [SerializeField]
    private float itemMass;

    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private bool itemUsed = false;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void UseItem()
    {
        if (!itemUsed)
        {
            //获取物品碰撞体的边界
            Bounds bounds = boxCollider2D.bounds;
            
            //计算碰撞体边界上最接近玩家的点
            Vector3 closestPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());
            
            //如果玩家在桌子的右侧，则向左翻转
            if (closestPointToPlayer.x == bounds.max.x)
            {
                animator.SetBool(Settings.flipLeft, true);
            }
            //如果玩家在桌子的左侧，则向右翻转
            else if (closestPointToPlayer.x == bounds.min.x)
            {
                animator.SetBool(Settings.flipRight, true);
            }
            //如果玩家在桌子下方，则向上翻转
            else if (closestPointToPlayer.y == bounds.min.y)
            {
                animator.SetBool(Settings.flipUp, true);
            }
            else
            {
                animator.SetBool(Settings.flipDown, true);
            }
            
            //将图层设置为“环境”——子弹现在会与桌子碰撞
            gameObject.layer = LayerMask.NameToLayer("Environment");
            
            //将物体的质量设置为指定值，以便玩家可以移动该物体
            rigidbody2D.mass = itemMass;

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlip);
            
            itemUsed = true;
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(itemMass), itemMass, false);
    }
#endif

    #endregion
}