using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    //对象引用
    [Space(10)]
    [Header("OBJECT REFERENCES")]

    #endregion

    #region Tooltip

    //用 DoorCollider 游戏对象上的 BoxCollider2D 组件填充此项
    [Tooltip("Populate this with the BoxCollider2D component on the DoorCollider gameobject")]

    #endregion

    [SerializeField]
    private BoxCollider2D doorCollider;
    
    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;  // 之前已打开的
    private Animator animator;

    private void Awake()
    {
        //默认禁用门的碰撞器
        doorCollider.enabled = false;
        
        //加载组件
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        //当父游戏对象被禁用（例如，当玩家远离房间时）动画器状态会被重置。因此，我们需要恢复动画器的状态。
        animator.SetBool(Settings.open, isOpen);
    }

    /// <summary>
    /// 开门
    /// </summary>
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;
            
            //设置 Animator 中的 open 参数
            animator.SetBool(Settings.open, true);
            
            //play sound effect
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
        }
    }

    /// <summary>
    /// 锁门
    /// </summary>
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;
        
        //关门后将开门设置为false
        animator.SetBool(Settings.open, false);
    }

    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this,nameof(doorCollider), doorCollider);
    }
#endif

    #endregion
}
