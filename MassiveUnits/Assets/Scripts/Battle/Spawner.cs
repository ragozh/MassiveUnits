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
    public void StartSpawn(int spawnCount)
    {
        StartCoroutine(Spawning(spawnCount));
    }
    IEnumerator Spawning(int spawnCount)
    {
        while (_mobCount < spawnCount)
        {
            //SpawnAMob();
            for (int i = 0; i < _countPerSpawn; i++)
            {
                var newMob = SpawnAMob(); // MeleeMob RangeMob
                BattleManager.Instance.MobsAlive.Add(newMob);
            }
            yield return new WaitForSeconds(_spawnRate);
        }
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            StartSpawn(1);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            StartSpawn(10);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            StartSpawn(100);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            StartSpawn(1000);
        }
    }
    public MobController SpawnAMob()
    {
        string model = Random.Range(0, 11) < 4 ? "RangeMob" : "MeleeMob";
        float range = model == "RangeMob" ? 6 : 2;
        float moveSpeed = model == "RangeMob" ? 2.5f : 3;
        var randomPosition = GetRandomPosition();
        var newMobObj = _mobPrefab.gameObject.Spawn(randomPosition, transform);
        var newMob = newMobObj.GetComponent<MobController>();
        newMob.Range = range;
        newMob.MoveSpeed = moveSpeed;
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
