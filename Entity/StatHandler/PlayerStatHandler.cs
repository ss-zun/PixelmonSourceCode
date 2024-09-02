using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerStatHandler : MonoBehaviour
{
    public PlayerData data;

    #region Player Status
    public BigInteger maxHp;
    public int def;
    #endregion

    public void UpdateStats(float perHp, float perDef, float addHp = 1)
    {
        maxHp = (BigInteger)(data.baseMaxHp * (1 + perHp / 100));
        def = (int)(data.baseDef * (1 + perDef / 100));

        Player.Instance.healthSystem.maxHealth = maxHp;
        Player.Instance.healthSystem.currentHealth += (BigInteger)(data.baseMaxHp * addHp);
    }
}
