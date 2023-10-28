using System.Numerics;

public struct MobData
{
    public int MobId;
    public float Range;
    public float Damage;
    public float MoveSpeed;
    public Vector3 LastestPosition;
    public bool IsDead;
    public int HP;
    public int MaxHP;
    public MobData(int id, int hp, float damage, float range, float moveSpeed, Vector3 lastPos)
    {
        MobId = id;
        IsDead = false;
        Range = range;
        MoveSpeed = moveSpeed;
        LastestPosition = lastPos;
        HP = hp;
        MaxHP = hp;
        Damage = damage;
    }
}
