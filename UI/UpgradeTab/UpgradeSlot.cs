using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeIndex
{
    Atk,
    Dmg,
    SDmg,
    Cri,
    CriDmg,
    SCri,
    SCriDmg
}

public abstract class UpgradeSlot : MonoBehaviour
{
    protected BigInteger ownGold => SaveManager.Instance.userData.gold;

    [Header("Slot Info")]
    [HideInInspector] public UpgradeTab upgradeTab;
    [SerializeField] private UpgradeIndex slotIndex;
    [SerializeField] protected int maxLv;
    

    [Header("Upgrade UI")]
    [SerializeField] private TextMeshProUGUI slotLevelTxt;
    [SerializeField] protected TextMeshProUGUI slotValueTxt;
    [SerializeField] protected TextMeshProUGUI nextValueTxt;
    [SerializeField] private TextMeshProUGUI slotPriceTxt;
    [SerializeField] private TextMeshProUGUI buyTxt;
    [SerializeField] private Button goldBtn;

    public int _curLv;
    public int CurLv
    {
        get
        {
            return _curLv;
        }
        set
        {
            if (_curLv != value)
            {
                _curLv = value;
                if (_curLv > maxLv)
                {
                    _curLv = maxLv;
                }

                if (isStart)
                {
                    upgradeTab.SaveUpgradeLvs((int)slotIndex, _curLv);
                }
            }
        }
    }

    public float _curValue;
    protected float CurValue
    {
        get
        {
            return _curValue;
        }
        set
        {
            if (_curValue != value)
            {
                _curValue = value;
                GiveChangedStat(_curValue);
                PixelmonStatHandler.ApplyMyPixelmon(slotIndex);
            }
        }
    }

    #region Buy Button Values
    public int nextLv;
    public float nextValue;

    private BigInteger curLvPrice => Calculater.CalPrice(CurLv, b, d1, d2);
    protected BigInteger nextPrice;
    [SerializeField] private int b = 100;
    [SerializeField] private int d1 = 500;
    [SerializeField] private int d2 = 200;
    #endregion

    private int curUpgradeRate = 1;
    private bool isStart = false;

    private void Start()
    {
        curUpgradeRate = 1;
        InitSlot();
        isStart = true;
    }

    private void InitSlot()
    {
        CurValue = ValuePerLv(CurLv);
        CalculatePrice(curUpgradeRate);
        SetLvTxt();
    }

    #region UI Methods
    public void BuyBtn()
    {
        if (nextPrice > ownGold)
        {
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20008").clip);
            UIManager.Instance.ShowWarn("골드가 부족합니다!!");
        }
        else
        {
            CurLv = nextLv;
            CurValue = nextValue;

            if (QuestManager.Instance.IsMyTurn(upgradeTab.questMapping[slotIndex]))
            {
                QuestManager.Instance.OnQuestEvent();
            }

            SaveManager.Instance.SetFieldData(nameof(SaveManager.Instance.userData.gold), -nextPrice, true);

            if (curUpgradeRate == 0)
            {
                upgradeTab.CurrentToggle(0);
            }
            else
            {
                CalculatePrice(curUpgradeRate);
            }
            SetLvTxt();
        }
    }

    private void SetLvTxt()
    {
        if (CurLv == maxLv)
        {
            slotLevelTxt.text = "Lv.Max";
        }
        else
        {
            slotLevelTxt.text = "Lv." + CurLv.ToString();
        }
    }

    protected abstract void SetValueTxt();

    private void SetGoldTxt()
    {
        string printPrice = Calculater.NumFormatter(nextPrice);

        if (CurLv >= maxLv)
        {
            slotPriceTxt.text = "--";
            buyTxt.text = "MAX";
            goldBtn.enabled = false;
            return;
        }

        if (nextPrice > ownGold)
        {
            slotPriceTxt.text = $"<color=#FF0000>{printPrice}</color>";
        }
        else
        {
            slotPriceTxt.text = $"<color=#FFFFFF>{printPrice}</color>";
        }
    }
    #endregion

    private void GiveChangedStat(float value)
    {
        var fieldname = slotIndex.ToString();
        var upgradeStatType = PixelmonManager.Instance.upgradeStatus.GetType();
        var fieldInfo = upgradeStatType.GetField(fieldname);
        fieldInfo.SetValue(PixelmonManager.Instance.upgradeStatus, value);
    }

    protected abstract float ValuePerLv(int reachLv);

    public void CalculatePrice(int mulValue) //next 3종 새로고침.
    {
        curUpgradeRate = mulValue;

        if (curUpgradeRate == 0)
        {
            FindMaxPrice();
        }
        else
        {
            if (CurLv + curUpgradeRate > maxLv)
            {
                curUpgradeRate = maxLv - CurLv;
            }

            nextLv = CurLv + curUpgradeRate;
            nextValue = ValuePerLv(nextLv);
            nextPrice = Calculater.CalPriceSum(nextLv - 1, b, d1, d2) - Calculater.CalPriceSum(CurLv - 1, b, d1, d2);
        }
        SetValueTxt();
        SetGoldTxt();
    }

    private void FindMaxPrice()
    {
        nextLv = CurLv;
        BigInteger exclusion = Calculater.CalPriceSum(CurLv - 1, b, d1, d2);

        do
        {
            nextLv++;
            nextPrice = Calculater.CalPriceSum(nextLv - 1, b, d1, d2) - exclusion;
        }
        while (nextLv < maxLv && Calculater.CalPriceSum(nextLv, b, d1, d2) - exclusion <= ownGold);

        nextValue = ValuePerLv(nextLv);
        SetGoldTxt();
    }
}