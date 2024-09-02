using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class PixelmonFSM : FSM
{
    public Pixelmon pixelmon;
    public string EnemyTag = "Enemy";

    #region Pixelmon States
    public IdleState IdleState { get; private set; }
    public PixelmonMoveState MoveState { get; private set; }
    public PixelmonAttackState AttackState { get; private set; }
    #endregion

    Coroutine attackCoroutine;
    public float attackSpeed = 2f; //패시브 쪽에서 만약 관련 패시브 보유한 픽셀몬이 장착되면 여기서 빼주기.
    WaitUntil waitAttack;
    float coolTime = 0;
    float minDistance;
    private void Start()
    {
        waitAttack = new WaitUntil(() => AttackCoolTime());
    }

    public void InitStates()
    {
        IdleState = new IdleState(this);
        MoveState = new PixelmonMoveState(this);
        AttackState = new PixelmonAttackState(this);
        minDistance = Player.Instance.statHandler.data.baseAtkRange;
        ChangeState(IdleState);
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (coolTime == 0)
            {
                var enemies = Search(1);
                if (enemies.Count == 0)
                {
                    target = null;
                    if (Player.Instance.fsm.target != null)
                        Player.Instance.fsm.ChangeState(Player.Instance.fsm.DetectState);
                }
                else
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        target = enemies[i].gameObject;
                        UnityEngine.Vector2 direction = enemies[i].transform.position - transform.position;
                        (BigInteger, bool) damage = pixelmon.status.GetTotalDamage(pixelmon.myData);
                        PoolManager.Instance.SpawnFromPool<ProjectileController>("ATV00000").GetAttackSign(transform.position, direction, damage.Item1, damage.Item2, 10, 5);
                        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20011").clip);
                    };
                }
            }
            yield return waitAttack;
        }
    }

    private bool AttackCoolTime()
    {
        coolTime += Time.deltaTime;
        if (coolTime >= attackSpeed)
        {
            coolTime = 0;
            return true;
        }
        return false;
    }

    public void InvokeAttack(bool isAttack)
    {
        if (isAttack && attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(Attack());
        }
        else if (!isAttack && attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    public List<Collider2D> Search(int count)
    {
        var hitColliders = Physics2D.OverlapCircleAll(Player.Instance.transform.position, minDistance).ToList();
        hitColliders.RemoveAll(obj => !obj.gameObject.CompareTag(EnemyTag));
        hitColliders.Sort((a, b) =>
        {
            var aDist = (transform.position - a.gameObject.transform.position).sqrMagnitude;
            var bDist = (transform.position - b.gameObject.transform.position).sqrMagnitude;
            return aDist.CompareTo(bDist);
        });
        count = Mathf.Min(count, hitColliders.Count);
        hitColliders.RemoveRange(count, hitColliders.Count - count);
        return hitColliders;
    }
}