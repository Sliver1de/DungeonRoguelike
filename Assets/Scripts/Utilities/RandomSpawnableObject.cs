using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据当前关卡，从配置好的可生成对象中，按照格子的权重(ratio)随机返回一个对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class RandomSpawnableObject<T>
{
    private struct chanceBoundaries
    {
        public T spawnableObject;
        public int lowBoundaryValue;    //低边界值
        public int highBoundaryValue;   //高边界值
    }

    private int ratioValueTotal = 0;
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectsByLevel<T>> spawnableObjectByLevelList;    //按关卡分类的可生成对象列表

    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectByLevelList)
    {
        this.spawnableObjectByLevelList = spawnableObjectByLevelList;
    }

    public T GetItem()
    {
        int upperBoundary = -1;
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        T spawnableObject = default(T);

        foreach (SpawnableObjectsByLevel<T> spawnableObjectByLevel in this.spawnableObjectByLevelList)
        {
            //检测当前等级
            if (spawnableObjectByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                foreach (SpawnableObjectRatio<T> spawnableObjectRatio in
                         spawnableObjectByLevel.spawnableObjectRatioList)
                {
                    int lowerBoundary = upperBoundary + 1;

                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;

                    ratioValueTotal += spawnableObjectRatio.ratio;
                    
                    //将可生成对象添加到列表
                    chanceBoundariesList.Add(new chanceBoundaries()
                    {
                        spawnableObject = spawnableObjectRatio.dungeonObject, 
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    });
                }
            }
        }

        if (chanceBoundariesList.Count == 0) return spawnableObject;

        int lookUpValue = Random.Range(0, ratioValueTotal);
        
        //遍历列表以获取选定的随机可生成对象详情
        foreach (chanceBoundaries spawnChance in chanceBoundariesList)
        {
            if (lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue)
            {
                spawnableObject = spawnChance.spawnableObject;
                break;
            }
        }

        return spawnableObject;
    }
}
