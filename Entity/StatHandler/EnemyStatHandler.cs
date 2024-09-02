using System.Numerics;
using UnityEngine;

public class EnemyStatHandler : MonoBehaviour
{
    [SerializeField]private Enemy enemy;
    public EnemyData data;

    #region Enemy Status
    public BigInteger enemyAtk;
    public BigInteger enemyMaxHp;
    public BigInteger enemyDef;
    #endregion

    public string enemyAtk1;
    public string enemyHp1;
    public string enemyDef1;

    public void UpdateEnemyStats()
    {
        int difficulty = StageManager.Instance.diffNum;

        int world = StageManager.Instance.worldNum;
        int stage = StageManager.Instance.stageNum;

        int deltaStage = difficulty * 150 + (world - 1) * 5 + stage -1;

        if (data.isBoss)
        {
            enemyAtk = data.Atk * (deltaStage * 5 + 100) / 100;
            enemyMaxHp = data.Hp * (deltaStage * 50 + 100) / 100;
            enemyDef = data.Def * (deltaStage * 3 + 100) / 100;

            enemyAtk1 = enemyAtk.ToString();
            enemyHp1 = enemyMaxHp.ToString();
            enemyDef1 = enemyDef.ToString();
        }
        else
        {
            enemyAtk = data.Atk * (deltaStage * 3 + 100) / 100;
            enemyMaxHp = data.Hp * (deltaStage * 15 + 100) / 100;
            enemyDef = data.Def * (deltaStage * 2 + 100) / 100;

            enemyAtk1 = enemyAtk.ToString();
            enemyHp1 = enemyMaxHp.ToString();
            enemyDef1 = enemyDef.ToString();
        }
        enemy.healthSystem.initEnemyHealth(enemyMaxHp);
    }

    public BigInteger GetDamage()
    {
        return enemyAtk;
    }
}
