using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    #region Tooltip
    //用你想要添加到对象池中的预制体填充这个数组，并指定每个预制体要创建的游戏对象数量
    [Tooltip("Populate this array with prefabs that you want to add to the pool, " +
             "and specify the number of game objects to be created for each.")]

    #endregion

    [SerializeField]
    private Pool[] poolArray = null;

    private Transform objectPoolTransform;
    
    private Dictionary<int,Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();
    
    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        //这个单例游戏对象将作为对象池的父物体
        objectPoolTransform = this.gameObject.transform;
        
        //在开始时创建对象池
        for (int i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }
    }

    /// <summary>
    /// 使用指定的预制件和每个预制件的指定池大小创建对象池
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    /// <param name="componentType"></param>
    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        int poolKey = prefab.GetInstanceID();
        
        string prefabName = prefab.name;    //获取prefab名字

        //创建父游戏对象，将子对象作为其子物体
        GameObject parentGameobject = new GameObject(prefabName + "Anchor");    
        
        parentGameobject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey,new Queue<Component>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameobject.transform) as GameObject;
                
                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
            }
        }
    }

    /// <summary>
    /// 在对象池中重用一个游戏对象组件。‘prefab’ 是包含该组件的预制件游戏对象。‘position’ 是游戏对象启用时应出现的世界位置。
    /// ‘rotation’ 应该设置，如果游戏对象需要旋转的话
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            //从对象池队列中获取object
            Component componentToReuse = GetComponentFromPool(poolKey);

            ResetObject(position, rotation, componentToReuse, prefab);

            return componentToReuse;
        }
        else
        {
            Debug.Log("No object pool for " + prefab);
            return null;
        }
    }

    /// <summary>
    /// 使用 ‘poolKey’ 从对象池中获取一个游戏对象组件。
    /// </summary>
    /// <param name="poolKey"></param>
    /// <returns></returns>
    private Component GetComponentFromPool(int poolKey)
    {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);

        if (componentToReuse.gameObject.activeSelf == true)
        {
            componentToReuse.gameObject.SetActive(false);
        }
        
        return componentToReuse;
    }

    /// <summary>
    /// 重置游戏对象
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="componentToReuse"></param>
    /// <param name="prefab"></param>
    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        componentToReuse.transform.position = position;
        componentToReuse.transform.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif

    #endregion
}
