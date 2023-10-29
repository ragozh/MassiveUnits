using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public struct AvoidanceJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<MobData> Mobs;
    [ReadOnly] public float3 PlayerPosition;
    [ReadOnly] public float DeltaTime;
    [ReadOnly] public NativeArray<float3> MobSteps;
    [WriteOnly] public NativeArray<float3> MobNewSteps;
    [WriteOnly] public NativeArray<bool> MobCanMove;
    const float SENSING_RADIUS = 3;
    public void Execute(int index)
    {
        var mob = Mobs[index];
        var step = MobSteps[index];
        int[] collides = new int[Mobs.Length - 1];
        int cIdx = 0;
        for (int i = 0; i < Mobs.Length; i++)
        {
            if (i == index) continue;
            if (math.distancesq(MobSteps[i], MobSteps[index]) <= SENSING_RADIUS * SENSING_RADIUS)
            {
                collides[cIdx] = i;
            }
        }
        Calculate(index, collides);
    }

    private float3 Calculate(int index, int[] collides)
    {
        float3 newPosition;
        for (int i = 0; i < collides.Length; i++)
        {

        }
        newPosition = MobSteps[index];
        return newPosition;
    }

    private float3 DirectionToPLayer(float3 position)
    {
        float3 direction = PlayerPosition - position;
        direction = new float3(math.normalize(direction).x, 0, math.normalize(direction).z);
        return direction;
    }
}
