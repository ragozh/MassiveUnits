using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private static Spawner _instance;
    public static Spawner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Spawner>();
            }
            if (_instance == null)
            {
                var newGameManagerGO = new GameObject("GeneratedSpawner");
                DontDestroyOnLoad(newGameManagerGO);
                _instance = newGameManagerGO.AddComponent<Spawner>();
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    [SerializeField]
    GameObject _mobPrefab;
    [SerializeField]
    int _mapSize, _countPerSpawn;
    [SerializeField]
    float _spawnRate;
    int _mobCount = 0;
    public int MobCount => _mobCount;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void StartSpawn()
    {
        StartCoroutine(Spawning());
    }
    IEnumerator Spawning()
    {
        while (_mobCount < 1050)
        {
            for (int i = 0; i < _countPerSpawn; i++)
            {
                SpawnAMob("MeleeMob"); // MeleeMob RangeMob
            }
            yield return new WaitForSeconds(_spawnRate);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            StartSpawn();
        }
    }
    public MobController SpawnAMob(string model)
    {
        var randomPosition = GetRandomPosition();
        var newMobObj = _mobPrefab.gameObject.Spawn(randomPosition, transform);
        var newMob = newMobObj.GetComponent<MobController>();
        var modelPref = Resources.Load<GameObject>("Prefabs/" + model);
        modelPref.Spawn(randomPosition, newMob.ModelHolder);
        modelPref.transform.localPosition = Vector3.zero;
        _mobCount++;
        return newMob;

        Vector3 GetRandomPosition()
        {
            int x = Random.Range(-_mapSize / 2, _mapSize / 2);
            int z = Random.Range(-_mapSize / 2, _mapSize / 2);
            return new Vector3(x, 0, z);
        }
    }
}
