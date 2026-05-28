using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

//编辑器创建相应的房间节点(入口、走廊、房间等)
public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;     //房间节点样式
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;    //图形偏移量
    private Vector2 graphDrag;      //图形拖动
    
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    //节点布局值(节点大小)
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    //节点之间连线值
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;
    
    //网格间距
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;
    
    //在unity编辑器 window中创建房间节点画布
    [MenuItem("Room Node Graph Editor",menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        RoomNodeGraphEditor window = GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        //订阅 "检查器选择已更改 "事件
        Selection.selectionChanged += InspectorSelectionChanged;
        
        //定义节点布局
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        //定义选中节点
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background=EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor=Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        //加载节点类型列表
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        //取消订阅 "检查器选择已更改 "事件
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// <summary>
    /// 双击创建的SO Asset(打开房间地图，节点编辑器)，打开房间图编辑器窗口
    /// </summary>
    /// <param name="instanceID"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    [OnOpenAsset(0)]    //需要引用 UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        
        return false;
    }


    /// <summary>
    /// 绘制GUI
    /// </summary>
    private void OnGUI()
    {
        #region 测试节点
        //GUILayout.BeginArea(new Rect(new Vector2(100f, 100f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);
        //EditorGUILayout.LabelField("Node 1");
        //GUILayout.EndArea();

        //GUILayout.BeginArea(new Rect(new Vector2(300f, 300f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);
        //EditorGUILayout.LabelField("Node 2");
        //GUILayout.EndArea();
        #endregion

        //如果选择了 RoomNodeGraphSO 类型的脚本对象，则处理
        if (currentRoomNodeGraph != null)
        {
            //绘制网格
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);
            
            //被拖动时画线
            DrawDraggedLine();

            //过程事件
            ProcessEvents(Event.current);

            //绘制房间节点之间的连接
            DrawRoomConnections();

            //绘制房间节点
            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    /// <summary>
    ///为房间节点图编辑器绘制背景网格
    /// </summary>
    /// <param name="gridSize"></param>
    /// <param name="gridOpacity"></param>
    /// <param name="gridColor"></param>
    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset,
                new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset,
                new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }
        
        Handles.color = Color.white;
    }

    /// <summary>
    /// 绘制拖曳线
    /// </summary>
    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            //从直线位置到直线位置画线
            //贝塞尔线
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, 
                currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, 
                currentRoomNodeGraph.linePosition,
                Color.white, null, connectingLineWidth);
        }
    }

    //处理事件
    private void ProcessEvents(Event currentEvent)
    {
        //重置图形拖动
        graphDrag = Vector2.zero;
        
        //如果鼠标经过的房间节点为空或当前未被拖动，则获取该节点
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = ISMouseOverRoomNode(currentEvent);
        }

        //如果鼠标不在房间节点上，或者我们当前正在从房间节点上移开一条线，则处理图形事件
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            //处理房间节点事件
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    /// <summary>
    /// 点击查看鼠标是否停留在房间节点上 - 如果是，则返回房间节点，否则返回 null
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    private RoomNodeSO ISMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }


    /// <summary>
    /// 处理流程节点图
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //处理鼠标点击事件
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 处理房间节点图上（而非节点上）的鼠标向下事件
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //在图形事件上单击鼠标右键的过程（显示上下文菜单）
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        //在图形事件中处理鼠标左击
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    /// <summary>
    /// 显示上下文菜单
    /// </summary>
    /// <param name="mousePosition"></param>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    /// <summary>
    /// 在鼠标位置创建房间节点
    /// </summary>
    /// <param name="mousePositionObject"></param>
    private void CreateRoomNode(object mousePositionObject)
    {
        //如果当前节点图为空，则先添加入口房间节点
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f,200f), roomNodeTypeList.list.Find(x=>x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        //创建房间节点脚本对象资产
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        //将房间节点添加到当前房间节点图的房间节点列表中
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        //设置房间节点值
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        //将房间节点添加到房间节点图脚本对象资产数据库中
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        //刷新图节点字典
        currentRoomNodeGraph.OnValidate();

    }

    /// <summary>
    /// 删除选中节点
    /// 这里注意不能直接删除，会导致一系列问题 将要删除的节点先保存到队列中再删除
    /// </summary>
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeleteQueue = new Queue<RoomNodeSO>();
        
        //循环遍历所有节点
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeleteQueue.Enqueue(roomNode);
                
                //遍历子房间节点 id
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    //检索子房间节点
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);

                    if (childRoomNode != null)
                    {
                        //移除子节点上的ParentID
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
                
                //遍历父房间节点ID
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    //检索父节点
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

                    if (parentRoomNodeID != null)
                    {
                        //移除父节点上的childID
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        
        //删除队列中的房间节点
        while (roomNodeDeleteQueue.Count > 0)
        {
           //从队列中获取房间节点
           RoomNodeSO roomNodeToDelete = roomNodeDeleteQueue.Dequeue();
           
           //从字典中移除节点
           currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
           
           //从列表中移除节点
           currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
           
           //从资产数据库中删除节点
           DestroyImmediate(roomNodeToDelete,true);
           
           //Save asset database
           AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// 删除所选房间节点之间的链接
    /// </summary>
    private void DeleteSelectedRoomNodeLinks()
    {
        //遍历所有房间节点
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    //获取子房间节点
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);
                    
                    //如果选择了子房间节点
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        //从父房间节点移除子 ID
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        
                        //从子房间节点移除父 ID
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        //清除所有选定的房间节点
        ClearAllSelectedRoomNodes();
    }
    
    

    //清除所有选中的房间节点
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }

    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        //如果松开鼠标右键并正在拖动一条直线
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            //检查是否是房间节点
            RoomNodeSO roomNode = ISMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                //如果可以添加，则将其设置为父房间节点的子节点
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDRoomNode(roomNode.id))
                {
                    //在房间子节点中设置父节点 ID
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    /// <summary>
    /// 鼠标拖曳事件
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        //处理右键拖动事件 - 画线
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        //处理左键拖动事件 - 拖动节点图
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    /// <summary>
    /// 处理鼠标右拖事件 - 画线
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    /// <summary>
    /// 处理鼠标左键拖动事件 - 拖动房间节点图
    /// </summary>
    /// <param name="dragDelta"></param>
    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;
        
        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }
        GUI.changed = true;
    }

    /// <summary>
    /// 从房间节点拖动连接线
    /// </summary>
    /// <param name="delta"></param>
    public void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    //绘制房间节点之间的连接
    private void DrawRoomConnections()
    {
        //遍历所有房间节点
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                //遍历所有孩子节点
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    //从字典中获取孩子节点
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 在父房间节点和子房间节点之间绘制连接线
    /// </summary>
    /// <param name="parentRoomNode"></param>
    /// <param name="childRoomNode"></param>
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        //获取线段起点和终点
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        //计算中点
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        //计算距离
        Vector2 direction = endPosition - startPosition;

        //计算从中点开始的归一化垂直位置
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        //计算箭头中点偏移位置
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        //Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        //draw line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }

    /// <summary>
    /// 在图形窗口中绘制房间节点
    /// </summary>
    private void DrawRoomNodes()
    {
        //循环浏览所有房间节点并绘制它们
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }

    /// <summary>
    /// 检查器中的选择已更改
    /// </summary>
    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}
