using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnemyHealthSystem : HealthSystem
{
    [SerializeField] protected Enemy enemy;

    [SerializeField] private Transform fillBar;

    protected override void Update()
    {
        if(currentHealth <= 0)
            fillBar.localScale = new Vector3(0, 1, 1);
        else
            fillBar.localScale = new Vector3((float)currentHealth / (float)maxHealth, 1, 1);
    }

    public void initEnemyHealth(BigInteger hp)
    {
        maxHealth = hp;
        currentHealth = maxHealth;
    }

    public override void TakeDamage(BigInteger delta, bool isCri = false, bool isPlayer = false)
    {
        def = (int)enemy.statHandler.enemyDef;
        base.TakeDamage(delta, isCri, isPlayer);
    }

    protected override void NoticeDead()
    {
        enemy.fsm.ChangeState(enemy.fsm.DieState);
    }
}
