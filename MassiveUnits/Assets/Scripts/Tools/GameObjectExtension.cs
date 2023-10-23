using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;

public static class GameObjectExtension
{
    public static GameObject Spawn(this GameObject prefab, Vector3 worldPosition, Transform parent)
    {
        return PoolManager.Instance.Spawn(prefab, worldPosition, parent);
    }
    public static void Despawn(this GameObject obj, bool surpassWarning = false)
    {
        PoolManager.Instance.Kill(obj, surpassWarning);
    }
    /// <summary>
    /// Get GameObject from Addressables Assets
    /// </summary>
    /// <param name="codeName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    //public static AsyncOperationHandle<GameObject> GetPrefab(string codeName, string path = "Assets/Prefabs")
    //{
    //    path = $"{path}/{codeName}.prefab";
    //    return Addressables.LoadAssetAsync<GameObject>(path);
    //}
}