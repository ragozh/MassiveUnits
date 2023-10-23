using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pool
{
    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField]
    private int initialPoolsize = 10;
    private Stack<GameObject> pooledInstances;
    private List<GameObject> activeInstances;
    public Transform Transform
    {
        get; set;
    }
    public Pool(GameObject prefab, int initialPoolsize)
    {
        this.prefab = prefab;
        this.initialPoolsize = initialPoolsize;
    }
    public void InitPool()
    {
        pooledInstances = new Stack<GameObject>();
        activeInstances = new List<GameObject>();

        for (int i = 0; i < initialPoolsize; i++)
        {
            GameObject instance = GameObject.Instantiate(prefab);
            instance.transform.SetParent(Transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.transform.localEulerAngles = Vector3.zero;

            InvokeEvent(instance, PoolEvent.Despawn);

            instance.SetActive(false);

            pooledInstances.Push(instance);
        }
    }
    int count = 0;
    public GameObject Spawn(Vector3 position,
        Quaternion rotation = default,
        Vector3 scale = default,
        Transform parent = null,
        bool useLocalPosition = false,
        bool useLocalRotation = false,
        bool isActive = true)
    {
        if (pooledInstances.Count <= 0) // Every game object has been spawned!
        {
            GameObject freshObject = Object.Instantiate(prefab);
            freshObject.name += count++;
            InvokeEvent(freshObject, PoolEvent.Create);
            pooledInstances.Push(freshObject);
        }

        GameObject obj = pooledInstances.Pop();

        obj.transform.SetParent(parent);
        if (useLocalPosition)
            obj.transform.localPosition = position;
        else
            obj.transform.position = position;
        if (rotation.eulerAngles == Quaternion.identity.eulerAngles)
        {
            rotation = prefab.transform.rotation;
        }
        if (useLocalRotation)
            obj.transform.localRotation = rotation;
        else
            obj.transform.rotation = rotation;
        if (scale == default)
            scale = Vector3.one;
        obj.transform.localScale = scale;
        SetActiveSafe(obj, isActive);

        activeInstances.Add(obj);
        InvokeEvent(obj, PoolEvent.Spawn);

        return obj;
    }
    /// <summary>
    /// Deactivate an object and add it back to the pool, given that it's
    /// in alive objects array.
    /// </summary>
    /// <param name="obj"></param>
    public void Kill(GameObject obj)
    {
        int index = activeInstances.FindIndex(o => obj == o);
        if (index == -1)
        {
            Object.Destroy(obj);
            return;
        }
        InvokeEvent(obj, PoolEvent.Despawn);

        obj.transform.SetParent(Transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.localEulerAngles = Vector3.zero;

        pooledInstances.Push(obj);
        activeInstances.RemoveAt(index);

        SetActiveSafe(obj, false);
    }
    private void SetActiveSafe(GameObject obj, bool value)
    {
        if (obj.activeSelf != value)
        {
            obj.SetActive(value);
        }
    }
    public bool IsResponsibleForObject(GameObject obj)
    {
        int index = activeInstances.FindIndex(o => obj == o);
        if (index == -1)
        {
            return false;
        }
        return true;
    }
    private enum PoolEvent { Spawn, Despawn, Create }
    private void InvokeEvent(GameObject instance, PoolEvent ev)
    {
        var poolScripts = instance.GetComponentsInChildren<IPoolObject>();

        if (ev == PoolEvent.Spawn)
        {
            foreach (IPoolObject poolScript in poolScripts)
            {
                poolScript.OnSpawn();
            }
        }
        else if (ev == PoolEvent.Despawn)
        {
            foreach (IPoolObject poolScript in poolScripts)
            {
                poolScript.OnDespawn();
            }
        }
        else if (ev == PoolEvent.Create)
        {
            foreach (IPoolObject poolScript in poolScripts)
            {
                poolScript.OnCreated();
            }
        }
    }
}
