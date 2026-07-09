using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// 为房间构建一条路径，从 startGridPosition（起始网格位置） 到 endGridPosition（终点网格位置），并将移动步骤添加到返回的 Stack（栈） 中。
    /// 如果未找到路径，则返回 null。
    /// </summary>
    /// <param name="room"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    /// <returns></returns>
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        //根据下限调整位置
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;
        
        //创建开放列表（Open List）和封闭哈希集（Closed HashSet
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();
        
        //为路径寻找创建网格节点（GridNodes）
        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet,
            room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    /// <summary>
    /// 寻找最短路径——如果找到路径，则返回终点节点，否则返回null
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <param name="gridNodes"></param>
    /// <param name="openNodeList"></param>
    /// <param name="closedNodeHashSet"></param>
    /// <param name="instantiateRoom"></param>
    /// <returns></returns>
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiateRoom)
    {
        //将起始节点添加到开放列表
        openNodeList.Add(startNode);
        
        //遍历开放节点列表，直到为空
        while (openNodeList.Count > 0)
        {
            //sort list
            openNodeList.Sort();
            
            //当前节点——开放列表中 fCost 最低的节点
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);
            
            //如果当前节点是目标节点，则结束
            if (currentNode == targetNode)
            {
                return currentNode;
            }
            
            //将当前节点添加到关闭列表中
            closedNodeHashSet.Add(currentNode);
            
            //计算当前节点每个相邻节点的 fCost
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet,
                instantiateRoom);
        }
        return null;
    }

    /// <summary>
    /// 创建包含移动路径的 Stack
    /// </summary>
    /// <param name="targetNode"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;
        
        //获取网格单元的中心点
        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null) 
        {
            //将网格坐标转换为世界坐标
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(
                nextNode.gridPosition.x + room.templateLowerBounds.x,
                nextNode.gridPosition.y + room.templateLowerBounds.y, 0));
            
            //将世界坐标设置为网格单元的中心点
            worldPosition += cellMidPoint;
            
            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    /// <summary>
    /// 评估相邻节点
    /// </summary>
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes,
        List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiateRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;
        
        //loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j,
                    gridNodes, closedNodeHashSet, instantiateRoom);

                if (validNeighbourNode != null)
                {
                    //计算相邻节点的新 gCost
                    int newCostToNeighbour;
                    
                    // 获取移动惩罚, 不可行走的路径值为 0。默认移动惩罚在设置中定义，适用于其他网格方块
                    int movementPenaltyForGridSpace =
                        instantiateRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x,
                            validNeighbourNode.gridPosition.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) +
                                         movementPenaltyForGridSpace;

                    //newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);

                    bool isValidNeighbourNodeToOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeToOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeToOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 计算网格上的曼哈顿距离，但允许对角线移动，以降低寻路代价
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
        {
            //使用 10 代替 1，而 14 是毕达哥拉斯近似值 根号下（10方+10方）—— 以避免使用浮点数(提升计算效率)
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    /// <summary>
    /// 评估位于 neighbourNodeXPosition 和 neighbourNodeYPosition 的相邻节点，
    /// 使用指定的 gridNodes、closedNodeHashSet 和已实例化的房间。如果节点无效，则返回 null
    /// </summary>
    /// <param name="neighbourNodeXPosition"></param>
    /// <param name="neighbourNodeYPosition"></param>
    /// <param name="gridNodes"></param>
    /// <param name="closedNodeHashSet"></param>
    /// <param name="instantiateRoom"></param>
    /// <returns></returns>
    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition,
        GridNodes gridNodes, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiateRoom)
    {
        //如果相邻节点的位置超出网格范围，则返回 null
        if (neighbourNodeXPosition >=
            instantiateRoom.room.templateUpperBounds.x - instantiateRoom.room.templateLowerBounds.x ||
            neighbourNodeXPosition < 0 ||
            neighbourNodeYPosition >=
            instantiateRoom.room.templateUpperBounds.y - instantiateRoom.room.templateLowerBounds.y ||
            neighbourNodeYPosition < 0)
        {
            return null;
        }
        
        //获取相邻节点。
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);
        
        //检查该位置是否有障碍物
        int movementPenaltyForGridSpace =
            instantiateRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];
        
        //检查该位置是否存在可移动障碍物
        int itemObstacleForGridSpace =
            instantiateRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];
        
        //如果相邻节点是障碍或在关闭列表中，则跳过
        if (movementPenaltyForGridSpace == 0 || closedNodeHashSet.Contains(neighbourNode)) 
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
}
