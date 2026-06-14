using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        //if (player.movementByVelocityEvent == null)
        //{
        //    Debug.LogError("movementByVelocityEvent is null");
        //}

        //订阅通过速度移动事件
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        
        //订阅移动到位置事件
        player.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
        
        //订阅待机事件  
        player.idleEvent.OnIdle += IdleEvent_OnIdle;
        
        //订阅武器瞄准事件
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        
        player.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
        
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;
        
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// <summary>
    /// 按照刚体速度处理移动事件
    /// </summary>
    /// <param name="movementByVelocityEvent"></param>
    /// <param name="movementByVelocityArgs"></param>
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent,
        MovementByVelocityArgs movementByVelocityArgs)
    {
        InitializeRollAnimationParameters();
        SetMovementAnimationParameters();
    }

    /// <summary>
    /// 处理移动位置事件
    /// </summary>
    /// <param name="movementToPositionEvent"></param>
    /// <param name="movementToPositionArgs"></param>
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,
        MovementToPositionArgs movementToPositionArgs)
    {
        InitializeAimAnimationParameters();
        InitializeRollAnimationParameters();
        SetMovementToPositionAnimationParameters(movementToPositionArgs);
    }

    /// <summary>
    /// 处理待机状态事件
    /// </summary>
    /// <param name="idleEvent"></param>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        InitializeRollAnimationParameters();
        SetIdleAnimationParameters();
    }

    /// <summary>
    /// 处理武器瞄准事件
    /// </summary>
    /// <param name="aimWeaponEvent"></param>
    /// <param name="aimWeaponEventArgs"></param>
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitializeAimAnimationParameters();
        InitializeRollAnimationParameters();
        SetAimWeaponAnimationParameters(aimWeaponEventArgs.aimDirection);
    }

    private void InitializeAimAnimationParameters()
    {
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimUpRight, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimDown, false);
    }

    private void InitializeRollAnimationParameters()
    {
        player.animator.SetBool(Settings.rollDown, false);
        player.animator.SetBool(Settings.rollUp, false);
        player.animator.SetBool(Settings.rollRight, false);
        player.animator.SetBool(Settings.rollLeft, false);
    }

    /// <summary>
    /// 设置动画参数
    /// </summary>
    private void SetMovementAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    /// <summary>
    /// 设置位置移动动画参数
    /// </summary>
    /// <param name="movementToPositionArgs"></param>
    private void SetMovementToPositionAnimationParameters(MovementToPositionArgs movementToPositionArgs)
    {
        //翻滚动画
        if (movementToPositionArgs.isRolling)
        {
            if (movementToPositionArgs.moveDirection.x > 0f)
            {
                player.animator.SetBool(Settings.rollRight, true);
            }
            else if (movementToPositionArgs.moveDirection.x < 0f)
            {
                player.animator.SetBool(Settings.rollLeft, true);
            }
            else if (movementToPositionArgs.moveDirection.y > 0f)
            {
                player.animator.SetBool(Settings.rollUp, true);
            }
            else if (movementToPositionArgs.moveDirection.y < 0f)
            {
                player.animator.SetBool(Settings.rollDown, true);
            }
        }
    }

    /// <summary>
    /// 设置运动动画参数
    /// </summary>
    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    /// <summary>
    /// 设置AIM动画参数

    /// </summary>
    /// <param name="aimDirection"></param>
    private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
    {
        //设置瞄准方向
        switch (aimDirection)
        {
            case AimDirection.Up:
                player.animator.SetBool(Settings.aimUp, true);
                break;
            case AimDirection.UpRight:
                player.animator.SetBool(Settings.aimUpRight, true);
                break;
            case AimDirection.UpLeft:
                player.animator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.Right:
                player.animator.SetBool(Settings.aimUpRight, true);
                break;
            case AimDirection.Left:
                player.animator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.Down:
                player.animator.SetBool(Settings.aimDown, true);
                break;
        }
    }
}
