using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private MovementByVelocityEvent movementByVelocityEvent;

    private void Awake()
    {
        //加载组件
        rigidbody2D = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        //订阅移动事件
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        //取消订阅
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    /// <summary>
    /// 当移动事件触发时
    /// </summary>
    /// <param name="movementByVelocityEvent"></param>
    /// <param name="movementByVelocityArgs"></param>
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent,
        MovementByVelocityArgs movementByVelocityArgs)
    {
        MoveRigidBody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
    }

    /// <summary>
    /// 移动刚体组件
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <param name="moveSpeed"></param>
    private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        //确保RB碰撞方向设置为连续
        rigidbody2D.velocity = moveDirection * moveSpeed;
    }
}
