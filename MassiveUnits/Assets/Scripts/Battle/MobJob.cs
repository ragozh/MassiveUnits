using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct MobJob : IJobParallelFor
{
    public NativeArray<MobData> Mobs;
    public float3 PlayerPosition;
    public float DeltaTime;
    public NativeArray<float3> NewPosition;
    public NativeArray<bool> ShouldMove;
    const float SENSING_RADIUS = 3;
    public void Execute(int index)
    {
        if (Mobs[index].IsDead) return;
        float3 newPosition = CalculateNewPosition(index, Mobs[index]);
        NewPosition[index] = newPosition;
    }

    private float3 CalculateNewPosition(int index, MobData mob)
    {
        float3 mobPos = new float3(mob.LastestPosition.X, 0, mob.LastestPosition.Z);
        float3 newPosition;
        if (math.distancesq(PlayerPosition, mobPos) > mob.Range * mob.Range)
        {
            var direction = DirectionToPLayer(mobPos);
            newPosition = mobPos + direction * mob.MoveSpeed * DeltaTime;
            ShouldMove[index] = true;
        }
        else
        {
            newPosition = mobPos;
        }
        return newPosition;
    }

    private float3 DirectionToPLayer(float3 position)
    {
        float3 direction = PlayerPosition - position;
        direction = new float3(math.normalize(direction).x, 0, math.normalize(direction).z);
        return direction;
    }
}