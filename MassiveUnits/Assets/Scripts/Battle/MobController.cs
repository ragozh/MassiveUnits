using System;
using System.Collections;
using UnityEngine;

public class MobController : MonoBehaviour, IPoolObject
{
    public Transform ModelHolder;
    public float Range;
    public float MoveSpeed;
    public UnitState State;
    public MobData Data;
    Animator _animator;
    public Quadtree Quad;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Attack()
    {
        if (State != UnitState.ATTACK)
        {
            State = UnitState.ATTACK;
            PlayAnim("Attack01", callback: () => { State = UnitState.ILDE; });
        }
    }
    public void Step(Vector3 nextStep)
    {
        if (State != UnitState.WALK)
        {
            State = UnitState.WALK;
            PlayAnim("WalkFWD");
        }
        transform.position = nextStep;
        Data.LastestPosition = new System.Numerics.Vector3(nextStep.x, nextStep.y, nextStep.z);
        if (!Quad.IsInside(Data.LastestPosition))
        {
            Quad.RemoveMob(Data);
            BattleManager.Instance.Quadtree.Insert(this);
        }
    }
    public void PlayAnim(string animName, float animDuration = 0, Action callback = null)
    {
        float duration = animDuration;
        if (duration == 0)
        {
            var anim = FindAnimation(_animator, animName);
            duration = anim.length;
        }
        StartCoroutine(PlayAnimation(animName, duration, callback));
    }
    public IEnumerator PlayAnimation(string animName, float duration, Action callback)
    {
        _animator.CrossFade(animName, 0.1f);
        yield return new WaitForSeconds(duration);
        if (callback != null)
        {
            callback();
        }
    }
    public void Die()
    {
        Data.IsDead = true;
        PlayAnim("Die",callback: () =>
        {
            gameObject.Despawn();
        });
    }
    public AnimationClip FindAnimation(Animator animator, string name)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }
    public void OnSpawn()
    {
        Data.IsDead = false;
    }

    public void OnDespawn()
    {
        foreach (Transform child in ModelHolder)
        {
            child.gameObject.Despawn();
        }
    }

    public void OnCreated()
    {

    }
}