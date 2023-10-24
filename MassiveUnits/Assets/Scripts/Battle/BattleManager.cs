using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BattleManager>();
            }
            if (_instance == null)
            {
                var newGameManagerGO = new GameObject("GeneratedBattleManager");
                DontDestroyOnLoad(newGameManagerGO);
                _instance = newGameManagerGO.AddComponent<BattleManager>();
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
    Transform _player;
    public Transform Player => _player;
    public float SqrDistanceToPlayer(Vector3 position) => (position - Player.position).sqrMagnitude;
    public List<MobController> MobsAlive = new List<MobController>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (MobsAlive.Count <= 0)
        {
            return;
        }
        var allPosition = MobsAlive.Select(x => x.transform.position).ToArray();
        NativeArray<float3> MobsPosition = new NativeArray<float3>(allPosition.Length, Allocator.TempJob);
        MobsPosition.Reinterpret<Vector3>().CopyFrom(allPosition);
        NativeArray<float3> NewMobsPosition = new NativeArray<float3>(allPosition.Length, Allocator.TempJob);
        MobJob newMobJob = new MobJob()
        {
            Range = 3,
            MoveSpeed = 2.5f,
            MobsPosition = MobsPosition,
            PlayerPosition = BattleManager.Instance.Player.position,
            DeltaTIme = Time.deltaTime,
            NewPosition = NewMobsPosition
        };

        JobHandle mobJobHandle = newMobJob.Schedule(MobsPosition.Length, 20);

        mobJobHandle.Complete();
        Vector3[] newPosition = new Vector3[MobsPosition.Length];
        newMobJob.NewPosition.Reinterpret<Vector3>().CopyTo(newPosition);
        UpdateAllMobPosition(newPosition);
    }
    void UpdateAllMobPosition(Vector3[] positions)
    {
        for (int i = 0; i < MobsAlive.Count; i++)
        {
            MobsAlive[i].transform.position = positions[i];
        }
    }
}
