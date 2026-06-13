using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private IdleEvent idleEvent;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRighdBody();
    }

    /// <summary>
    /// 移动刚体组件
    /// </summary>
    private void MoveRighdBody()
    {
        //确保将刚体碰撞检测设置为连续
        rigidbody2D.velocity = Vector2.zero;
    }
}
