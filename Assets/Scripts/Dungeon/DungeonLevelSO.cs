using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS
    [Space(10)]
    [Header("Basic Level Details")]
    #endregion
    
    #region Tooltip
    [Tooltip("The name of the level")]
    #endregion
    
    public string levelName;
    
    #region Header Room Templates for Levels
    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    #endregion
    
    #region Tooltip
    //（工具提示（“使用您想要作为关卡一部分的房间模板填充列表。您需要确保房间模板包含在关卡的房间节点图中指定的所有房间节点类型中。”）]
    [Tooltip("Populate the list with the room templates that you want to be part of the level. "+
    "You need to ensure that room templates are included for all room node types"+ 
    "that are specified in the Room Node Graphs for the level.")]
    #endregion
    
    public List<RoomTemplateSO> roomTemplateList;
    
    #region Header Room Node Graphs for level
    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    #endregion
    #region Tooltip
    //用应从该级别中随机选择的房间节点图填充此列表。
    [Tooltip("Populate this list with the room node graphs which should be randomly selected from for the level.")]
    #endregion
    
    public List<RoomNodeGraphSO> roomNodeGraphList;
    
    #region Validation
    #if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
        {
            return;
        }

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }
        
        //检查以确保为指定节点图中的所有节点类型指定了房间模板
        
        //首先检查是否指定了北/南走廊、东/西走廊和入口类型
        bool isNSCorridor = false;
        bool isEWCorridor = false;
        bool isEntrance = false;
        
        //循环遍历所有房间模板以检查是否已指定该节点类型
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
            {
                return;
            }

            if (roomTemplateSO.roomNodeType.isCorridorEW)
            {
                isEWCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isCorridorNS)
            {
                isNSCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isEntrance)
            {
                isEntrance = true;
            }
        }

        if (isEWCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " No E/W Corridor Room Type Specified.");
        }

        if (isNSCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " No N/S Corridor Room Type Specified.");
        }

        if (isEntrance == false)
        {
            Debug.Log("In " + this.name.ToString() + " No Entrance Room Type Specified.");
        }

        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
            {
                return;
            }

            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                {
                    continue;
                }
                
                //检查是否为每个 roomNode 类型指定了房间模板
                
                //走廊和入口已经检查过
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW ||
                    roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isCorridor ||
                    roomNodeSO.roomNodeType.isNone)
                {
                    continue;
                }

                bool isRoomNodeTypeFound = false;
                
                //循环遍历所有房间模板以检查是否已指定该节点类型
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null)
                    {
                        continue;
                    }

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("In " + this.name.ToString() + " : No room template " +
                              roomNodeSO.roomNodeType.name.ToString()
                              + " found for node graph" + roomNodeGraph.name.ToString());
                }
            }
        }

    }
#endif
    #endregion
    
}
