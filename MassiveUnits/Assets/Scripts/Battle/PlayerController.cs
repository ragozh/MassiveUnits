using System.Collections;
using System.Collections.Generic;
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
    }
    // Update is called once per frame
    UnitState _state = UnitState.ILDE;
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
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
        var direction = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        return direction.normalized;
    }
}
public enum UnitState : byte
{
    ILDE = 0,
    WALK = 1,
    ATTACK = 2,
    DIE = 3
}
