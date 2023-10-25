using System.Numerics;

public struct MobData
{
    public float Range;
    public float MoveSpeed;
    public Vector3 LastestPosition;
    public MobData(float range, float moveSpeed, Vector3 lastPos)
    {
        Range = range;
        MoveSpeed = moveSpeed;
        LastestPosition = lastPos;
    }
}
