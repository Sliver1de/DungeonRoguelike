using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public class MiniMap : MonoBehaviour
{
    #region Tooltip
    //填充子对象 MiniMapPlayer
    [Tooltip("Populate with the child MiniMapPlayer gameobject")]

    #endregion

    [SerializeField]
    private GameObject miniMapPlayer;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameManager.Instance.GetPlayer().transform;
        
        //将玩家设为 Cinemachine 相机目标
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;
        
        //设置小地图玩家图标
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMiniMapIcon();
        }
    }

    private void Update()
    {
        //移动小地图上的玩家图标以跟随玩家
        if (playerTransform != null && miniMapPlayer != null)
        {
            miniMapPlayer.transform.position = playerTransform.position;
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(miniMapPlayer), miniMapPlayer);
    }
#endif

    #endregion
}
