using System;
using UnityEngine;

public class EnemyFSM : FSM
{
    public Enemy enemy;
    public GameObject enemyCollision;

    #region Enemy States
    public IdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState {  get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyDieState DieState { get; private set; }
    #endregion

    private void OnEnable()
    {
        ChangeState(ChaseState);
    }

    private void Start()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }

        target = Player.Instance.HitPosition;
    }
    public void Init()
    {
        IdleState = new IdleState(this);
        ChaseState = new EnemyChaseState(this);
        AttackState = new EnemyAttackState(this);
        DieState = new EnemyDieState(this);

        ChangeState(ChaseState);
    }

    public void OnEnemyAttack()
    {
        Player.Instance.healthSystem.TakeDamage(enemy.statHandler.GetDamage(), true);
    }

    public void OnEnemyDead()
    {
        StageManager.Instance.spawner.RemoveActiveMonster(enemy);

        if (enemy.statHandler.data.isBoss)
            StageManager.Instance.isBossDieDone = true;
    }
}