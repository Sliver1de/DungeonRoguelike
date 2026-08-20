using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,
        MovementToPositionArgs movementToPositionArgs)
    {
        MoveRigidBody(movementToPositionArgs.movePosition, movementToPositionArgs.currentPosition,
            movementToPositionArgs.moveSpeed);
    }

    /// <summary>
    /// 移动刚体组件
    /// </summary>
    /// <param name="movePosition"></param>
    /// <param name="currentPosition"></param>
    /// <param name="moveSpeed"></param>
    private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);

        rigidbody2D.MovePosition(rigidbody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }
}
