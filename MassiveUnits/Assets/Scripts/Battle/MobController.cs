using Unity.Jobs;
using UnityEngine;

public class MobController : MonoBehaviour
{
    public Transform ModelHolder;
    public float Range;
    public float MoveSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    void ChasePlayer()
    {
        if (PlayerInRange())
        {
            return;
        }
        else
        {
            MoveToPlayer();
        }
    }
    void MoveToPlayer()
    {
        var direction = BattleManager.Instance.Player.position - transform.position;
        direction = new Vector3(direction.normalized.x, 0, direction.normalized.z);
        transform.position += direction * MoveSpeed * Time.deltaTime;
    }
    bool PlayerInRange() => BattleManager.Instance.SqrDistanceToPlayer(transform.position) <= Range * Range;
}