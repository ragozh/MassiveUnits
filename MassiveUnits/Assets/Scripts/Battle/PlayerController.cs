using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Transform _playerModel;
    [SerializeField]
    Transform _objDirection;
    [SerializeField]
    float _moveSpeed;
    [SerializeField]
    Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        SetupPointer();
    }
    // Update is called once per frame
    UnitState _state = UnitState.ILDE;
    void Update()
    {
    }
    bool _isAttacking = false;
    private void FixedUpdate()
    {
        if (!_isAttacking)
            _attackTick+= Time.deltaTime;
        if (_attackTick >= _attackDelay)
        {
            _attackTick -= _attackDelay;
            Attack();
        }
        var direction = InputHandler();
        if (direction.sqrMagnitude > 0)
        {
            if (_state != UnitState.WALK)
            {
                _state = UnitState.WALK;
                PlayAnim("WalkFWD");
            }
            _objDirection.localPosition = direction;
            _playerModel.LookAt(_objDirection);
            transform.position += direction * Time.deltaTime * _moveSpeed;
        }
        else
        {
            if (_state != UnitState.ILDE)
            {
                _state = UnitState.ILDE;
                PlayAnim("IdleBattle");
                _objDirection.localPosition = Vector3.zero;
            }
        }
    }
    float _attackTick = 0;
    [SerializeField]
    Transform _weapon, _weaponModel, _attackPoint, _start, _end, _mid;
    Transform _startPointer, _endPointer;
    [SerializeField]
    float _range, _arc, _closeRange, _closeArc, _attackDelay;
    public void Attack()
    {
        _isAttacking = true;
        var middle = _playerModel.forward;
        var origionPos = _weapon.localPosition;
        var origin = _weapon.rotation;   
        Sequence chained = DOTween.Sequence();
        chained.Append(_weapon.DOMove(_attackPoint.position, 0.1f));
        _weaponModel.DOLocalMove(Vector3.forward * _range, 0.1f);
        chained.Append(_weapon.DOLookAt(_startPointer.position, 0.1f));
        chained.Append(_weapon.DOLookAt(_endPointer.position, 0.3f).OnComplete(() =>
        {
            HitMobInArc(middle);
        }));
        chained.Append(_weaponModel.DOLocalMove(Vector3.zero, 0.1f));
        chained.Append(_weapon.DOLocalMove(origionPos, 0.1f).OnComplete(() =>
        {
            _isAttacking = false;
            _weapon.localPosition = origionPos;
            _weapon.rotation = origin;
        }));
    }
    void SetupPointer()
    {
        _startPointer = _start.GetChild(0);
        _endPointer = _end.GetChild(0);
        var localPos = _playerModel.forward * _range;
        localPos = new Vector3(localPos.x, _attackPoint.position.y, localPos.z);
        _mid.localPosition = localPos;
        _endPointer.localPosition = localPos;
        _startPointer.localPosition = localPos;
        _start.eulerAngles = new Vector3(0, -_arc / 2, 0);
        _end.eulerAngles = new Vector3(0, _arc / 2, 0);
    }
    void ResetPointer()
    {

    }
    public Vector3 RotateY(Vector3 v, float angle)
    {
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        var r = v;
        float tx = v.x;
        float tz = v.z;
        r.x = (cos * tx) + (sin * tz);
        r.z = (cos * tz) - (sin * tx);
        return r;
    }
    public void HitMobInArc(Vector3 middle)
    {
        //var allMobInRange = BattleManager.Instance.MobsAlive.Where(x => !x.Data.IsDead && (transform.position - x.transform.position).sqrMagnitude <= _range * _range).ToList();
        var allMobInRange = BattleManager.Instance.Quadtree.GetListMobInRange(transform.position, _range);
        List<MobController> hits = new List<MobController>();
        for (int i = 0; i < allMobInRange.Count; i++)
        {
            var mob = BattleManager.Instance.MobsAlive[allMobInRange[i].Data.MobId];
            if (mob == null || mob.Data.IsDead) continue;
            Vector3 meToTarget = mob.transform.position - _playerModel.position;
            meToTarget = new Vector3(meToTarget.x, 0, meToTarget.z);
            double angle = Vector3.Angle(middle, meToTarget);
            if (Math.Abs(angle) <= _arc / 2)
            {
                hits.Add(mob);
            }
            // incase mob too close
            if (angle <= _closeArc / 2 && (transform.position - mob.transform.position).sqrMagnitude <= _closeRange * _closeRange)
            {
                hits.Add(mob);
            }
        }
        for (int i = 0; i < hits.Count; i++)
        {
            hits[i].Die();
        }
    }
    public static double DegreeToRadian(double degrees)
    {
        double radians = (Math.PI / 180) * degrees;
        return (radians);
    }
    public void PlayAnim(string animName, float animDuration = 0)
    {
        float duration = animDuration;
        if (duration == 0)
        {
            var anim = FindAnimation(_animator, animName);
            duration = anim.length;
        }
        StartCoroutine(PlayAnimation(animName, duration));
    }
    public IEnumerator PlayAnimation(string animName, float duration)
    {
        _animator.CrossFade(animName, 0.1f);
        yield return new WaitForSeconds(duration);
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
    Vector3 InputHandler()
    {
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        return new Vector3(h, 0, v);
    }
}
public enum UnitState : byte
{
    ILDE = 0,
    WALK = 1,
    ATTACK = 2,
    DIE = 3
}
