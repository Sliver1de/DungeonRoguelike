using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
    #region Header POPULATE WITH MINIMAP CAMERA

    [Header("POPULATE WITH MINIMAP CAMERA")]

    #endregion

    [SerializeField]
    private Camera miniMapCamera;

    private Camera cameraMain;

    private void Start()
    {
        //缓存主摄像机
        cameraMain = Camera.main;
        
        InvokeRepeating(nameof(EnableRooms), 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        //如果当前正在显示地下城地图UI，则不处理
        if (GameManager.Instance.gameState == GameState.dungeonOverviewMap) return;
        
        HelperUtilities.CameraWorldPositionBounds(out Vector2Int miniMapCameraWorldPositionLowerBounds,
            out Vector2Int miniMapCameraWorldPositionUpperBounds, miniMapCamera);

        HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds,
            out Vector2Int mainCameraWorldPositionUpperBounds, cameraMain);
        
        //遍历地下城房间
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            
            //如果房间在小地图摄像机视口内，则激活房间游戏对象
            if ((room.lowerBounds.x <= miniMapCameraWorldPositionUpperBounds.x &&
                 room.lowerBounds.y <= miniMapCameraWorldPositionUpperBounds.y) &&
                (room.upperBounds.x >= miniMapCameraWorldPositionLowerBounds.x &&
                 room.upperBounds.y >= miniMapCameraWorldPositionLowerBounds.y))
            {
                room.instantiatedRoom.gameObject.SetActive(true);
                
                //如果房间在主摄像机视野内，则激活环境游戏对象
                if ((room.lowerBounds.x <= mainCameraWorldPositionUpperBounds.x &&
                     room.lowerBounds.y <= mainCameraWorldPositionUpperBounds.y) &&
                    (room.upperBounds.x >= mainCameraWorldPositionLowerBounds.x &&
                     room.upperBounds.y >= mainCameraWorldPositionLowerBounds.y))
                {
                    room.instantiatedRoom.ActivateEnvironmentGameObjects();
                }
                else
                {
                    room.instantiatedRoom.DeactivateEnvironmentGameObjects();
                }
            }
            else
            {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(miniMapCamera), miniMapCamera);
    }
#endif

    #endregion
}
