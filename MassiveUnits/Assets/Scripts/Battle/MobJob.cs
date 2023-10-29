using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct MobJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<MobData> Mobs;
    [ReadOnly] public float3 PlayerPosition;
    [ReadOnly] public float DeltaTime;
    [WriteOnly] public NativeArray<float3> MobSteps;
    [WriteOnly] public NativeArray<bool> MobCanMove;
    const float SENSING_RADIUS = 2;
    public void Execute(int index)
    {
        if (Mobs[index].IsDead) return;

        NativeArray<int> collides = new NativeArray<int>(Mobs.Length - 1, Allocator.Temp);
        int cIdx = 0;
        float3 position = new float3(Mobs[index].LastestPosition.X, Mobs[index].LastestPosition.Y, Mobs[index].LastestPosition.Z);
        for (int i = 0; i < Mobs.Length; i++)
        {
            if (i == index) continue;
            if (Vector3.DistanceSquared(Mobs[index].LastestPosition, Mobs[i].LastestPosition) <= SENSING_RADIUS * SENSING_RADIUS)
            {
                collides[cIdx] = i;
            }
        }
        float3 newPosition = CalculateNewPosition(index, Mobs[index], collides);
        MobSteps[index] = newPosition;
    }

    private float3 CalculateNewPosition(int index, MobData mob, NativeArray<int> collides)
    {
        float3 mobPos = new float3(mob.LastestPosition.X, 0, mob.LastestPosition.Z);
        float3 newPosition;
        if (math.distancesq(PlayerPosition, mobPos) > mob.Range * mob.Range)
        {
            newPosition = Avoidance(index, collides);
            //var direction = DirectionToPLayer(mobPos);
            //newPosition = mobPos + direction * mob.MoveSpeed * DeltaTime;
        }
        else
        {
            newPosition = mobPos;
        }
        return newPosition;
    }
    float3 Avoidance(int index, NativeArray<int> collides)
    {
        float3 mobPos = new float3(Mobs[index].LastestPosition.X, 0, Mobs[index].LastestPosition.Z);
        var direction = DirectionToPLayer(mobPos);
        NativeArray<int> sameDirection = new NativeArray<int>(Mobs.Length - 1, Allocator.Temp);
        int sameIdx = 0;
        for (int i = 0; i < collides.Length; i++)
        {
            if (collides[i] == 0) break;
            var neighborIndex = collides[i];
            var neighborPos = new float3(Mobs[neighborIndex].LastestPosition.X, 0, Mobs[neighborIndex].LastestPosition.Z);
            // neighbor dont move
            if (math.distancesq(PlayerPosition, neighborPos) < Mobs[neighborIndex].Range * Mobs[neighborIndex].Range)
            {
                continue;
            }
            var neighborDirection = DirectionToPLayer(neighborPos);
            // same direction
            var cross = math.cross(direction, neighborDirection);
            if (cross.x == 0 && cross.y == 0 && cross.z == 0)
            {
                sameDirection[sameIdx++] = collides[i];
                continue;
            }
            direction += (neighborDirection - direction);
        }
        for (int i = 0; i < sameDirection.Length; i++)
        {
            if (sameDirection[i] == 0) break;
            var neighborIndex = sameDirection[i];
            var neighbotPos = new float3(Mobs[neighborIndex].LastestPosition.X, 0, Mobs[neighborIndex].LastestPosition.Z);
            var neighborDirection = DirectionToPLayer(neighbotPos);
            // same direction
            var cross = math.cross(direction, neighborDirection);
            if (cross.x == 0 && cross.y == 0 && cross.z == 0)
            {
                if (Mobs[neighborIndex].MoveSpeed < Mobs[index].MoveSpeed)
                {
                    direction = float3.zero;
                    break;
                }
                continue;
            }
            direction += (neighborDirection - direction);
        }
        float3 newPosition;
        if (!(direction.x == 0 && direction.y == 0 && direction.z == 0))
        {
            MobCanMove[index] = true;
            newPosition = mobPos + direction * Mobs[index].MoveSpeed * DeltaTime;
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