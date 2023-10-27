using System.Collections;
using TMPro;
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
    float _countPerSpawn = 1;
    [SerializeField]
    float _spawnRate = 0.5f;
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
    [SerializeField]
    TextMeshProUGUI _spawnRateText;
    public void SetSpawnRate() => _countPerSpawn = int.Parse(_spawnRateText.text);
    public void ToggleSpawn()
    {
        shouldSpawn = !shouldSpawn;
        StartSpawn(1);
    }

    bool shouldSpawn = false;
    IEnumerator Spawning(int spawnCount)
    {
        while (shouldSpawn)
        {
            //SpawnAMob();
            for (int i = 0; i < _countPerSpawn; i++)
            {
                var newMob = SpawnAMob(); // MeleeMob RangeMob
            }
            yield return new WaitForSeconds(_spawnRate);
        }
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnAMob();
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
            for (int i = 0; i < 1000; i++)
            {
                SpawnAMob();
            }
        }
    }
    public MobController SpawnAMob()
    {
        string model = Random.Range(0, 11) < 4 ? "RangeMob" : "MeleeMob";
        float range = model == "RangeMob" ? 4 : 2;
        float moveSpeed = model == "RangeMob" ? 2.5f : 3;
        var rdPos = GetRandomPosition();
        System.Numerics.Vector3 newPos = new System.Numerics.Vector3(rdPos.x, rdPos.y, rdPos.z);
        MobData mobData = new MobData(_mobCount, range, moveSpeed, newPos);
        var newMobObj = _mobPrefab.gameObject.Spawn(rdPos, transform);
        var newMob = newMobObj.GetComponent<MobController>();
        newMob.Data = mobData;
        var modelPref = Resources.Load<GameObject>("Prefabs/" + model);
        modelPref.Spawn(rdPos, newMob.ModelHolder);
        modelPref.transform.localPosition = Vector3.zero;
        _mobCount++;
        BattleManager.Instance.AddMob(newMob);
        return newMob;

        Vector3 GetRandomPosition()
        {
            var mapSize = 50 - 2; // prevent mob spawn in line of quad
            int x = Random.Range(-mapSize / 2, mapSize / 2);
            int z = Random.Range(-mapSize / 2, mapSize / 2);
            return new Vector3(x, 0, z);
        }
    }
}
