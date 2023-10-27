using System.Numerics;

public struct MobData
{
    public int MobId;
    public float Range;
    public float MoveSpeed;
    public Vector3 LastestPosition;
    public bool IsDead;
    public MobData(int id, float range, float moveSpeed, Vector3 lastPos)
    {
        MobId = id;
        IsDead = false;
        Range = range;
        MoveSpeed = moveSpeed;
        LastestPosition = lastPos;
    }
}
