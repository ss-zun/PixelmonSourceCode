using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DgMonster : MonoBehaviour
{
    public UIDungeonProgress dgProgress;

    private int dgIndex;
    public int dgLv;

    private BigInteger maxHealth 
        => Calculater.CalPrice(dgLv, baseHealth, d1Health, d2Health);
    private BigInteger currentHealth;
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int d1Health = 50;
    [SerializeField] private int d2Health = 50;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpTxt;
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D coll;

    private int goldRwdBNum = 100000;
    private int goldRwdD1 = 300000;
    private int goldRwdD2 = 100000;
    BigInteger myReward => Calculater.CalPrice(dgLv - 1, goldRwdBNum, goldRwdD1, goldRwdD2);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PixelmonProjectile"))
        {
            ProjectileController projectile = collision.gameObject.GetComponent<ProjectileController>();
            TakeDamage(projectile.projectileDamage);
            projectile.ResetProjectile();
        }
    }

    private async void Awake()
    {
        dgProgress = await UIManager.Show<UIDungeonProgress>();
        Player.Instance.statHandler.data.baseAtkRange = 3;
        Player.Instance.fsm.ChangeState(Player.Instance.fsm.DetectState);
    }

    public void InitDgMonster(int index)
    {
        dgIndex = index;
        dgLv = 1;

        hpSlider = StageManager.Instance.GetBossSlider();
        hpTxt = StageManager.Instance.GetBossHpText();
        currentHealth = maxHealth;
        StartCoroutine(bossHealthSlider());
    }

    private void TakeDamage(BigInteger delta)
    {
        BigInteger damage = 0;
        if (delta > 0) damage = delta;
        PoolManager.Instance.SpawnFromPool<DamageText>("TXT00001").ShowDamageText(damage, gameObject.transform.position);
        currentHealth -= damage;

        while (damage > 0)
        {
            if (damage >= currentHealth)
            {
                damage -= currentHealth;
                dgLv++;
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth -= damage;
                damage = 0;
            }
        }

    }

    private void SaveCurLv()
    {
        int[] dgLvs = SaveManager.Instance.userData.bestDgLvs;
        dgLvs[dgIndex] = dgLv;
        SaveManager.Instance.SetFieldData(nameof(SaveManager.Instance.userData.bestDgLvs), dgLvs);
        
        SaveManager.Instance.SetFieldData(nameof(SaveManager.Instance.userData.gold), myReward, true);
    }

    private IEnumerator bossHealthSlider()
    {
        float hpValue;
        while (true)
        {
            BigInteger ratio = currentHealth * 10000 / maxHealth;
            hpValue = (float)ratio / 10000;
            hpSlider.value = Mathf.Clamp01(hpValue);
            hpTxt.text = ((int)(hpSlider.value * 100)).ToString() + "%";

            if (currentHealth <= 0)
            {
                hpSlider.value = 1;
                hpTxt.text = "100%";
            }

            yield return null;
        }
    }

    public void DisableDgMonster()
    {
        StartCoroutine(KillDgMonster());
    }

    public IEnumerator KillDgMonster()
    {
        dgProgress.SetActive(false);
        coll.enabled = false;
        Player.Instance.statHandler.data.baseAtkRange = 4;
        yield return new WaitForSeconds(0.5f);
        if (StageManager.Instance.isDungeonClear)
        {
            anim.SetTrigger("dgEnd");
            SaveCurLv();
            yield return new WaitForSeconds(2f);
            StageManager.Instance.isDungeonClear = false;
        }
        coll.enabled = true;
        Destroy(gameObject);
    }

    public async void ShowRwdPopup()
    {
        await UIManager.Show<UIDgResultPopup>(myReward);
    }

    public void OnPlayBoxOpen()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20001").clip);
    }
}