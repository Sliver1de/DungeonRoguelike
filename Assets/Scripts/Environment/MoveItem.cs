using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveItem : MonoBehaviour
{
    #region Header SOUND EFFECT

    [Header("Sound Effect")]

    #endregion

    #region Tooltip

    [Tooltip("The sound effect when this item is moved")]

    #endregion

    [SerializeField]
    private SoundEffectSO moveSoundEffect;

    [HideInInspector] public BoxCollider2D boxCollider2D;
    
    private Rigidbody2D rigidbody2D;
    private InstantiatedRoom instantiatedRoom;
    private Vector3 previousPosition;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        instantiatedRoom = GetComponentInParent<InstantiatedRoom>();
        
        //将此物品添加到 itemObstacles 数组中
        instantiatedRoom.moveableItemsList.Add(this);
    }

    /// <summary>
    /// 在有物体接触时更新障碍物位置
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacle();
    }

    /// <summary>
    /// 更新障碍物位置
    /// </summary>
    private void UpdateObstacle()
    {
        //确保该物品保持在房间内
        ConfineItemToRoomBounds();
        
        //更新障碍物数组中的可移动物品
        instantiatedRoom.UpdateMoveableObstacles();
        
        //捕获碰撞后的新位置
        previousPosition = transform.position;
        
        //如果物体在移动（允许小速度），则播放音效
        if (Mathf.Abs(rigidbody2D.velocity.x) > 0.001f || Mathf.Abs(rigidbody2D.velocity.y) > 0.001f)
        {
            //每 10 帧播放一次移动音效
            if (moveSoundEffect != null && Time.frameCount % 10 == 0)
            {
                SoundEffectManager.Instance.PlaySoundEffect(moveSoundEffect);
            }
        }
    }

    /// <summary>
    /// 将物品限制在房间边界内
    /// </summary>
    private void ConfineItemToRoomBounds()
    {
        Bounds itemBounds = boxCollider2D.bounds;
        Bounds roomBounds = instantiatedRoom.roomColliderBounds;
        
        //如果物品被推动超出房间边界，则将其位置设置为先前的位置
        if (itemBounds.min.x <= roomBounds.min.x || itemBounds.max.x >= roomBounds.max.x ||
            itemBounds.min.y <= roomBounds.min.y || itemBounds.max.y >= roomBounds.max.y)
        {
            transform.position = previousPosition;
        }
    }
}
