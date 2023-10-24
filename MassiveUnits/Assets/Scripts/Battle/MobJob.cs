using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct MobJob : IJobParallelFor
{
    public float Range;
    public float MoveSpeed;
    public NativeArray<float3> MobsPosition;
    public float3 PlayerPosition;
    public float DeltaTIme;
    public NativeArray<float3> NewPosition;
    public void Execute(int index)
    {
        float3 position = MobsPosition[index];
        float3 newPosition;
        if (math.distance(PlayerPosition, position) > Range * Range)
        {
            float3 direction = PlayerPosition - position;
            direction = new float3(math.normalize(direction).x, 0, math.normalize(direction).z);
            newPosition = position + direction * MoveSpeed * DeltaTIme;
        }
        else
        {
            newPosition = position;
        }
        NewPosition[index] = newPosition;
    }
}