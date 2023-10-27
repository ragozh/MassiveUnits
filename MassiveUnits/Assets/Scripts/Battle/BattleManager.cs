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
        var allMob = MobsAlive.Select(x => x.Data).ToArray();
        NativeArray<MobData> allMobsData = new NativeArray<MobData>(allMob.Length, Allocator.TempJob);
        allMobsData.CopyFrom(allMob);

        NativeArray<float3> NewMobsPosition = new NativeArray<float3>(allMob.Length, Allocator.TempJob);
        NativeArray<bool> shouldMove = new NativeArray<bool>(allMob.Length, Allocator.TempJob);
        MobJob newMobJob = new MobJob()
        {
            //Range = 3,
            //MoveSpeed = 2.5f,
            //MobsPosition = MobsPosition,
            Mobs = allMobsData,
            PlayerPosition = BattleManager.Instance.Player.position,
            DeltaTime = Time.deltaTime,
            NewPosition = NewMobsPosition,
            ShouldMove = shouldMove
        };

        JobHandle mobJobHandle = newMobJob.Schedule(allMob.Length, 20);

        mobJobHandle.Complete();
        Vector3[] newPosition = new Vector3[allMob.Length];
        newMobJob.NewPosition.Reinterpret<Vector3>().CopyTo(newPosition);
        bool[] movable = new bool[allMob.Length];
        newMobJob.ShouldMove.CopyTo(movable);
        UpdateAllMobPosition(newPosition, movable);
    }
    void UpdateAllMobPosition(Vector3[] positions, bool[] shouldMove)
    {
        for (int i = 0; i < MobsAlive.Count; i++)
        {
            var mob = MobsAlive[i];
            if (mob.Data.IsDead) continue;
            mob.transform.LookAt(BattleManager.Instance.Player.position);
            if (shouldMove[i])
            {
                mob.Step(positions[i]);
            }
            else
            {
                mob.Attack();
            }
        }
    }
}
