using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//房间节点类型
[CreateAssetMenu(fileName = "RoomNodeType_",menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    //仅标记应该在编辑器中可见的 RoomNodeTypes
    [Header("Only flag the RoomNodeTypes that should be visible in the editor")]
    #endregion
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("One Type Should Be A Corridor")]   //是否为走廊
    #endregion
    public bool isCorridor;
    #region Header
    [Header("One Type Should Be A CorridorNS")]     //是否为南北走廊
    #endregion
    public bool isCorridorNS;
    #region Header
    [Header("One Type Should Be A CorridorEW")]     //是否为东西走廊
    #endregion
    public bool isCorridorEW;
    #region Header
    [Header("One Type Should Be An Entrance")]      //是否为入口
    #endregion
    public bool isEntrance;
    #region Header
    [Header("One Type Should Be A Boss Room")]      //是否为Boss房间
    #endregion
    public bool isBossRoom;
    #region Header
    [Header("One Type Should Be None (Unassigned)")]    //是否为空（未分配）
    #endregion
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    //OnValidate函数只在编辑器环境下触发
    private void OnValidate()
    {
        //检测roomNodeTypeName是否被留空
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}