using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    #region Tooltip
    //用 CursorTarget 游戏对象填充。
    [Tooltip("Populate with the CursorTarget gameobject")]
    #endregion
    [SerializeField] private Transform cursorTarget;

    private void Awake()
    {
        //加载组件
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    /// <summary>
    /// 设置cinemachine摄像机目标组
    /// </summary>
    private void SetCinemachineTargetGroup()
    {
        //为 cinemachine 摄像机创建要跟踪的目标组
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target
            { weight = 1f, radius = 1f, target = GameManager.Instance.GetPlayer().transform };

        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target
            { weight = 1f, radius = 1f, target = cursorTarget };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[]
            { cinemachineGroupTarget_player, cinemachineGroupTarget_cursor };

        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
