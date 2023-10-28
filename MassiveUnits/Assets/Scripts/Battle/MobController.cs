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
    public HPBarController HPBar;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    bool showHPBar = false;
    public void SetHPBar(HPBarController hpBar)
    {
        HPBar = hpBar;
        HPBar.Setup();
        HPBar.gameObject.SetActive(showHPBar);
    }
    public void Attack()
    {
        if (State != UnitState.ATTACK)
        {
            State = UnitState.ATTACK;
            PlayAnim("Attack01", callback: () =>
            {
                if (Data.Range <= 2)
                {
                    BattleManager.Instance.Player.TakeDamage((int)Data.Damage);
                }
                State = UnitState.ILDE;
            });
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
        if (showHPBar)
        {
            HPBar.UpdatePosition(nextStep);
        }
        Data.LastestPosition = new System.Numerics.Vector3(nextStep.x, nextStep.y, nextStep.z);
        if (!Quad.IsInside(Data.LastestPosition))
        {
            Quad.RemoveMob(this);
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
    public void TakeDamage(int damage)
    {
        Data.HP -= damage;
        if (Data.HP < Data.MaxHP)
        {
            showHPBar = true;
            HPBar.gameObject.SetActive(showHPBar);
            HPBar.UpdatePosition(transform.position);
        }
        HPBar.UpdateValue(Data.HP, Data.MaxHP);
        if (Data.HP <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Data.IsDead = true;
        PlayAnim("Die", callback: () =>
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
        showHPBar = false;
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