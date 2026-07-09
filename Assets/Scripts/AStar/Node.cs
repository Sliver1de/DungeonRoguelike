using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0;   //距离起始节点的距离
    public int hCost = 0;   //距离目标节点的距离
    public Node parentNode;

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int CompareTo(Node other)
    {
        //如果此实例的 Fcost 小(大、等)于 要比较的节点（nodeToCompare） 的 Fcost，则比较结果将小(大、等)于 0
        //compare will be <0 if this instance Fcost is less than nodeToCompare.Fcost
        //compare will be >0 if this instance Fcost is greater than nodeToCompare.Fcost
        //compare will be ==0 if the values are the same

        int compare = FCost.CompareTo(other.FCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return compare;
    }
}
