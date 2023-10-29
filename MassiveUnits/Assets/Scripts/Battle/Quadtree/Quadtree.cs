using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Quadtree
{
    const int _maxLevel = 3;
    int _level;
    public float MinX, MinZ, MaxX, MaxZ;
    Quadtree[] nodes;
    List<MobController> mobs = new List<MobController>();
    public List<MobController> Mobs => mobs;
    Quadtree Parent;
    public string QuadIndex = "";
    public Quadtree(int level, int index, float minX, float maxX, float minZ, float maxZ, Quadtree parent = null)
    {
        _level = level;
        MinX = minX;
        MaxX = maxX;
        MinZ = minZ;
        MaxZ = maxZ;
        Parent = parent;
        string parentIndex = parent != null ? parent.QuadIndex + "-" : "";
        QuadIndex = $"{parentIndex}{index}";
    }
    public bool IsInside(System.Numerics.Vector3 possition, float range = 0)
    {
        return !(possition.X + range <= MinX
            || possition.X - range > MaxX
            || possition.Z + range <= MinZ
            || possition.Z - range > MaxZ);
    }
    public void CreateNewQuadTree()
    {
        if (_level != 0) return;
        SplitToMaxLevel();
    }
    void SplitToMaxLevel()
    {
        if (_level == _maxLevel) return;
        Split();
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].SplitToMaxLevel();
        }
    }
    void Split()
    {
        nodes = new Quadtree[4];
        var middleX = (MaxX + MinX) / 2;
        var middleZ = (MaxZ + MinZ) / 2;
        nodes[0] = new Quadtree(_level + 1, 0, MinX, middleX, middleZ, MaxZ, this);
        nodes[1] = new Quadtree(_level + 1, 1, middleX, MaxX, middleZ, MaxZ, this);
        nodes[2] = new Quadtree(_level + 1, 2, MinX, middleX, MinZ, middleZ, this);
        nodes[3] = new Quadtree(_level + 1, 3, middleX, MaxX, MinZ, middleZ, this);
    }
    public void Insert(MobController mob)
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].IsInside(mob.Data.LastestPosition))
                {
                    nodes[i].Insert(mob);
                    break;
                }
            }
            return;
        }
        if (IsInside(mob.Data.LastestPosition))
        {
            mobs.Add(mob);
            mob.Quad = this;
        }
    }
    public void RemoveMob(MobController mob)
    {
        if (mobs.Contains(mob))
        {
            mobs.Remove(mob);
        }
    }
    public List<Quadtree> GetQuadInRange(System.Numerics.Vector3 position, float range)
    {
        List<Quadtree> result = new List<Quadtree>();
        if (_level < _maxLevel && nodes != null)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].IsInside(position, range))
                {
                    result.AddRange(nodes[i].GetQuadInRange(position, range));
                }
            }
        }
        if (_level == _maxLevel && IsInside(position, range))
        {
            result.Add(this);
        }
        return result;
    }
    public List<MobData> GetListMobInRange(MobData mob, float range)
    {
        List<MobData> result = new List<MobData>();
        var quads = GetQuadInRange(mob.LastestPosition, range);
        foreach (var quad in quads)
        {
            result.AddRange(quad.Mobs);
        }
        return result;
    }
    public List<MobController> GetListMobInRange(Vector3 position, float range)
    {
        var nPosition = new System.Numerics.Vector3(position.x, position.y, position.z);
        List<MobController> result = new List<MobController>();
        var quads = GetQuadInRange(nPosition, range);
        foreach (var quad in quads)
        {
            foreach (var mob in quad.mobs)
            {
                if ((mob.Data.LastestPosition - nPosition).LengthSquared() <= range * range)
                    result.Add(mob);
            }
        }
        return result;
    }
}
public struct QuadData
{
    public int Id;
    public float MinX;
    public float MinZ;
    public float MaxX;
    public float MaxZ;
    public QuadData(int id, float minX, float maxX, float minZ, float maxZ)
    {
        Id = id;
        MinX = minX;
        MinZ = minZ;
        MaxX = maxX;
        MaxZ = maxZ;
    }
    public bool IsInside(float3 possition, float range = 0)
    {
        return !(possition.x + range <= MinX
            || possition.x - range > MaxX
            || possition.z + range <= MinZ
            || possition.z - range > MaxZ);
    }
}