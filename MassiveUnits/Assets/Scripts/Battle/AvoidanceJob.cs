using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public struct AvoidanceJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<MobData> Mobs;
    [ReadOnly] public float3 PlayerPosition;
    [ReadOnly] public float DeltaTime;
    [WriteOnly] public NativeArray<float3> MobSteps;
    [WriteOnly] public NativeArray<bool> MobCanMove;
    const float SENSING_RADIUS = 3;
    public void Execute(int index)
    {

    }

    private float3 CalculateNewPosition(int index, MobData mob)
    {
        float3 mobPos = new float3(mob.LastestPosition.X, 0, mob.LastestPosition.Z);
        float3 newPosition;
        if (math.distancesq(PlayerPosition, mobPos) > mob.Range * mob.Range)
        {
            var direction = DirectionToPLayer(mobPos);
            newPosition = mobPos + direction * mob.MoveSpeed * DeltaTime;
            MobCanMove[index] = true;
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
