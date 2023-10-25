using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct MobJob : IJobParallelFor
{
    //public float Range;
    //public float MoveSpeed;
    //public NativeArray<float3> MobsPosition;
    public NativeArray<MobData> Mobs;
    public float3 PlayerPosition;
    public float DeltaTime;
    public NativeArray<float3> NewPosition;
    public NativeArray<bool> ShouldMove;
    public void Execute(int index)
    {
        //float3 position = MobsPosition[index];
        float3 position = new float3(Mobs[index].LastestPosition.X, Mobs[index].LastestPosition.Y, Mobs[index].LastestPosition.Z);
        float range = Mobs[index].Range;
        float moveSpeed = Mobs[index].MoveSpeed;
        float3 newPosition;
        if (math.distancesq(PlayerPosition, position) > range * range)
        {
            float3 direction = PlayerPosition - position;
            direction = new float3(math.normalize(direction).x, 0, math.normalize(direction).z);
            newPosition = position + direction * moveSpeed * DeltaTime;
            ShouldMove[index] = true;
        }
        else
        {
            newPosition = position;
        }
        NewPosition[index] = newPosition;
    }
}