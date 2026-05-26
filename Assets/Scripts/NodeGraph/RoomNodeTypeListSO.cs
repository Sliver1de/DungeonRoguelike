using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//房间节点类型列表SO
[CreateAssetMenu(fileName = "RoomNodeTypeListSO",menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header ROOM NODE TYPE LIST
    [Space(10)]
    [Header("ROOM NODE TYPE LIST")]
    #endregion
    #region TOOltip
    //该列表应包含游戏中所有的 RoomNodeTypeSO，它用于替代枚举（enum）
    [Tooltip("This list should be populated with all the RoomNodeTypeSO for the game - it is used instead of an enum")]
    #endregion
    public List<RoomNodeTypeSO> list;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion

}