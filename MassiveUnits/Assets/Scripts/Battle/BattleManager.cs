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
    PlayerController _player;
    public PlayerController Player => _player;
    [SerializeField]
    float _mapSize, _mobCollideRadius;
    public float MapSize => MapSize;
    public float SqrDistanceToPlayer(Vector3 position) => (position - Player.transform.position).sqrMagnitude;
    public List<MobController> MobsAlive = new List<MobController>();
    public Quadtree Quadtree;
    public void AddMob(MobController newMob)
    {
        MobsAlive.Add(newMob);
        Quadtree.Insert(newMob);
    }
    [ContextMenu("Check")]
    public void Check()
    {
        var nullQuads = MobsAlive.Where(x => !x.Data.IsDead).ToList();
    }
    // Start is called before the first frame update
    void Start()
    {
        Quadtree = new Quadtree(0, 0, -50 / 2, 50 / 2, -50 / 2, 50 / 2);
        Quadtree.CreateNewQuadTree();
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
        MobMoveJob();
    }
    void MobMoveJob()
    {
        var allMob = MobsAlive.Select(x => x.Data).ToArray();
        NativeArray<MobData> allMobsData = new NativeArray<MobData>(allMob.Length, Allocator.TempJob);
        allMobsData.CopyFrom(allMob);

        NativeArray<float3> MobSteps = new NativeArray<float3>(allMob.Length, Allocator.TempJob);
        NativeArray<bool> shouldMove = new NativeArray<bool>(allMob.Length, Allocator.TempJob);
        MobJob newMobJob = new MobJob()
        {
            Mobs = allMobsData,
            PlayerPosition = BattleManager.Instance.Player.transform.position,
            DeltaTime = Time.deltaTime,
            MobSteps = MobSteps,
            MobCanMove = shouldMove
        };
        JobHandle mobJobHandle = newMobJob.Schedule(allMob.Length, 40);
        mobJobHandle.Complete();

        //NativeArray<float3> MobNewSteps = new NativeArray<float3>(allMob.Length, Allocator.TempJob);
        //AvoidanceJob newAvoidance = new AvoidanceJob()
        //{
        //    Mobs = allMobsData,
        //    PlayerPosition = BattleManager.Instance.Player.transform.position,
        //    DeltaTime = Time.deltaTime,
        //    MobSteps = newMobJob.MobSteps,
        //    MobNewSteps = MobNewSteps,
        //    MobCanMove = shouldMove
        //};
        //JobHandle avoidanceJobHandle = newAvoidance.Schedule(allMob.Length, 40);
        //avoidanceJobHandle.Complete();

        Vector3[] newPosition = new Vector3[allMob.Length];
        newMobJob.MobSteps.Reinterpret<Vector3>().CopyTo(newPosition);
        bool[] movable = new bool[allMob.Length];
        newMobJob.MobCanMove.CopyTo(movable);
        UpdateAllMobPosition(newPosition, movable);
    }
    void UpdateAllMobPosition(Vector3[] positions, bool[] shouldMove)
    {
        for (int i = 0; i < MobsAlive.Count; i++)
        {
            var mob = MobsAlive[i];
            if (mob.Data.IsDead) continue;
            mob.transform.LookAt(BattleManager.Instance.Player.transform.position);
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
