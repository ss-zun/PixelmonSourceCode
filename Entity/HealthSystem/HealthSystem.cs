using Sirenix.OdinInspector;
using System;
using UnityEngine.UI;
using UnityEngine;
using System.Numerics;

public abstract class HealthSystem : SerializedMonoBehaviour
{
    public Image hpBar;

    /*체력변수*/
    public BigInteger maxHealth = 1;
    public BigInteger currentHealth;
    protected int def;

    protected virtual void Update()
    {
        hpBar.fillAmount = (float)(currentHealth * 10000 / maxHealth) / 10000;
    }

    public virtual void GetHealed(int delta)
    {
        if (currentHealth != 0)
        {
            if (maxHealth >= currentHealth + delta)
                currentHealth = currentHealth + delta;
            else
                currentHealth = maxHealth;
        }
    }

    public virtual void TakeDamage(BigInteger delta, bool isCri = false, bool isPlayer = false)
    {
        BigInteger damage = delta - def;
        if (damage < 0) damage = 0;

        if(damage > currentHealth)
        currentHealth = 0;
        else
            currentHealth = currentHealth - damage;
        PoolManager.Instance.SpawnFromPool<DamageText>("TXT00001").ShowDamageText(damage, gameObject.transform.position, isCri, isPlayer);        
        if (currentHealth == 0)
        {
            NoticeDead();
        } 
    }

    //사망 이벤트
    protected abstract void NoticeDead();
}