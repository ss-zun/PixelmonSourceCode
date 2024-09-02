using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthSystem : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Slider bossHpBar;
    [SerializeField] private TextMeshProUGUI bossHpTxt;

    [SerializeField] private BigInteger currentHealth => enemy.healthSystem.currentHealth;
    [SerializeField] private BigInteger maxHealth => enemy.healthSystem.maxHealth;

    Coroutine bossCoroutine;

    public void InvokeBossHp()
    {
        bossHpBar = StageManager.Instance.GetBossSlider();
        bossHpTxt = StageManager.Instance.GetBossHpText();
        bossCoroutine = StartCoroutine(bossHealthSlider());
    }

    private IEnumerator bossHealthSlider()
    {
        while(true)
        {
            bossHpBar.value = (float)(currentHealth * 10000 / maxHealth) / 10000;
            bossHpTxt.text = ((int)(bossHpBar.value * 100)).ToString() + "%";

            if (currentHealth <= 0)
                StopCoroutine(bossCoroutine);

            yield return null;
        }
    }
}