using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PoolManager>();
            }
            if (instance == null)
            {
                var newPoolManagerGO = new GameObject("GeneratedPoolManager");
                DontDestroyOnLoad(newPoolManagerGO);
                instance = newPoolManagerGO.AddComponent<PoolManager>();
            }
            return instance;
        }
    }
    private Dictionary<GameObject, Pool> currentPools = new Dictionary<GameObject, Pool>();
    private void Awake()
    {
        instance = this;
    }
    public GameObject Spawn(GameObject prefab, Vector3 position, Transform parent = null)
    {
        if (instance == null)
            instance = this;
        if (!currentPools.ContainsKey(prefab))
        {
            AddNewPool(new Pool(prefab, 0));
        }
        return currentPools[prefab].Spawn(position, parent: parent);
    }
    private void AddNewPool(Pool pool)
    {
        currentPools[pool.Prefab] = pool;

        GameObject poolObject = new GameObject();
        poolObject.name = pool.Prefab.name;
        poolObject.transform.SetParent(instance.transform);
        pool.Transform = poolObject.transform;

        pool.InitPool();
    }
    public void Kill(GameObject obj, bool surpassWarning = false)
    {
        if (obj == null)
        {
            return;
        }

        if (!obj.activeInHierarchy) // obj is already disabled or killed
        {
            return;
        }

        foreach (KeyValuePair<GameObject, Pool> pool in currentPools)
        {
            if (pool.Value.IsResponsibleForObject(obj))
            {
                pool.Value.Kill(obj);
                return;
            }
        }
        Destroy(obj);
    }
}
