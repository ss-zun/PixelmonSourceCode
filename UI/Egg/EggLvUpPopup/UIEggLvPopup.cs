using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EggLvBtnType
{
    GaugeUp,
    LvUp,
    DiaBtn,
    AdBtn
}

public class UIEggLvPopup : UIBase
{
    [SerializeField] private TextMeshProUGUI CurLvNum;
    [SerializeField] private TextMeshProUGUI NextLvNum;

    #region 확률표
    [SerializeField] private TextMeshProUGUI CommonCurNum;
    [SerializeField] private TextMeshProUGUI CommonNextNum;
    [SerializeField] private TextMeshProUGUI AdvancedCurNum;
    [SerializeField] private TextMeshProUGUI AdvancedNextNum;
    [SerializeField] private TextMeshProUGUI RareCurNum;
    [SerializeField] private TextMeshProUGUI RareNextNum;
    [SerializeField] private TextMeshProUGUI EpicCurNum;
    [SerializeField] private TextMeshProUGUI EpicNextNum;
    [SerializeField] private TextMeshProUGUI LegendaryCurNum;
    [SerializeField] private TextMeshProUGUI LegendaryNextNum;
    #endregion

    #region Lv업 게이지
    [SerializeField] private TextMeshProUGUI Desc;
    [SerializeField] private TextMeshProUGUI PriceTxt;
    [SerializeField] private Button GaugeUpBtn;
    [SerializeField] private Button LvUpBtn;
    [SerializeField] private Transform Gauges;
    [SerializeField] private LvUpGauge LvUpGauge;
    [SerializeField] private GameObject GaugeAndLvUp;

    private BigInteger price;
    private List<LvUpGauge> lvUpGauges = new List<LvUpGauge>();
    #endregion

    #region Lv업 중
    [SerializeField] private GameObject Clock;
    [SerializeField] private GameObject Skip;
    [SerializeField] private Button DiaBtn;
    [SerializeField] private TextMeshProUGUI TimeTxt;

    // 1렙때 1분, 2렙이후부턴 30분 * 2^(레벨-2)
    private float totalTime;
    private float remainingTime;
    private WaitForSeconds oneSecTime = new WaitForSeconds(1f);

    [SerializeField] private TextMeshProUGUI adsSkipCountTxt;
    private int skipDia = 1000;
    private float skipDiaTime = 1800f;
    #endregion

    #region 버튼 색상 Sprite
    [SerializeField] private Sprite GaugeUpSprite;
    [SerializeField] private Sprite LvUpSprite;
    [SerializeField] private Sprite SkipSprite;
    [SerializeField] private Sprite GraySprite;
    #endregion

    private string[] descs = { "레벨업 게이지", "레벨업 중", "최대 레벨 도달" };
    private UIMiddleBar uiMiddleBar;
    private UserData userData => SaveManager.Instance.userData;
    private Coroutine updateTimerCoroutine;

    private bool isGuide = false;
    private bool isInit = false;

    private void OnDisable()
    {
        if (updateTimerCoroutine != null)
        {
            StopCoroutine(updateTimerCoroutine);
            updateTimerCoroutine = null;
        }
    }
    
    public override void Opened(object[] param) 
    { 
        SetPopup(param[0] as UIMiddleBar);  

        if (!isInit)
        {
            isInit = true;

            UIManager.Instance.UpdateUI += UpdateEggLvPopupUI;
            UpdateLvAndRateUI();

            for (int i = 0; i < userData.eggLv / 5 + 2; i++)
            {
                lvUpGauges.Add(Instantiate(LvUpGauge, Gauges));
                if (i < userData.fullGaugeCnt)
                    lvUpGauges[i].GaugeUp();
            }

            if (userData.eggLv == 10)
            {
                Gauges.gameObject.SetActive(false);
                GaugeAndLvUp.SetActive(false);
            }

            price = Calculater.CalPrice(userData.eggLv, 10000, 17000, 12000);
            PriceTxt.text = Calculater.NumFormatter(price);
        }
    }

    public override void Closed(object[] param)
    {
        if (userData.eggLv < 2)
        {
            uiMiddleBar.SetGuideArrow(GuideManager.Instance.nestLvUp);
        }
        else
        {
            GuideManager.Instance.GuideArrow.SetActive(false);
        }
    }

    public void HidePopup()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20010").clip);
        UIManager.Hide<UIEggLvPopup>();
    }

    public void SetPopup(UIMiddleBar middleBar)
    {
        uiMiddleBar = middleBar;
        
        if (userData.isLvUpMode) // Lv업 중
            SetLvUpMode();
        else // Lv업 게이지
            SetGaugeMode();
    }

    private void UpdateEggLvPopupUI(DirtyUI dirtyU)
    {
        switch (dirtyU)
        {
            case DirtyUI.EggLv:
                UpdateLvAndRateUI();
                break;
            case DirtyUI.Gold:
                SetGaugeUpBtn();
                break;
            case DirtyUI.Diamond:
                SetDiaBtn();
                break;
        }
    }

    private void UpdateLvAndRateUI()
    {
        if (userData.eggLv != 10)
        {         
            NextLvNum.text = (userData.eggLv + 1).ToString();
            var nextData = DataManager.Instance.GetData<EggRateData>((userData.eggLv + 1).ToString());
          
            CommonNextNum.text = nextData.common.ToString("F2") + "%";           
            AdvancedNextNum.text = nextData.advanced.ToString("F2") + "%";           
            RareNextNum.text = nextData.rare.ToString("F2") + "%";         
            EpicNextNum.text = nextData.epic.ToString("F2") + "%";           
            LegendaryNextNum.text = nextData.legendary.ToString("F2") + "%";
        }
        else if(userData.eggLv == 10)
            NextLvNum.text = "X";

        CurLvNum.text = userData.eggLv.ToString();
        var curData = DataManager.Instance.GetData<EggRateData>(userData.eggLv.ToString());

        CommonCurNum.text = curData.common.ToString("F2") + "%";
        AdvancedCurNum.text = curData.advanced.ToString("F2") + "%";
        RareCurNum.text = curData.rare.ToString("F2") + "%";
        EpicCurNum.text = curData.epic.ToString("F2") + "%";
        LegendaryCurNum.text = curData.legendary.ToString("F2") + "%";
    }

    
    private void SetBtnSprite(EggLvBtnType type, Button btn, bool isInteractable)
    {
        if (!isInteractable)
        {
            btn.image.sprite = GraySprite;
            btn.interactable = false;
        }            
        else
        {
            btn.interactable = true;
            switch (type)
            {
                case EggLvBtnType.GaugeUp: btn.image.sprite = GaugeUpSprite; break;
                case EggLvBtnType.LvUp: btn.image.sprite = LvUpSprite; break;
                case EggLvBtnType.DiaBtn:
                case EggLvBtnType.AdBtn:
                    btn.image.sprite = SkipSprite; break;
            }
        }
    }

    private void SetLvUpBtn()
    {
        if (userData.fullGaugeCnt == lvUpGauges.Count)
        {
            SetBtnSprite(EggLvBtnType.LvUp, LvUpBtn, true);
            SetBtnSprite(EggLvBtnType.GaugeUp, GaugeUpBtn, false);
        }
        else
        {
            SetBtnSprite(EggLvBtnType.LvUp, LvUpBtn, false);
            SetBtnSprite(EggLvBtnType.GaugeUp, GaugeUpBtn, true);
        }

        if (isGuide)
        {
            EggLvGuide();
        }
    }

    private void SetGaugeUpBtn()
    {
        if (userData.fullGaugeCnt == lvUpGauges.Count) return;

        if (userData.gold >= price)
            SetBtnSprite(EggLvBtnType.GaugeUp, GaugeUpBtn, true);
        else
            SetBtnSprite(EggLvBtnType.GaugeUp, GaugeUpBtn, false);
    }

    private void SetDiaBtn()
    {
        if(userData.diamond >= skipDia)
            SetBtnSprite(EggLvBtnType.DiaBtn, DiaBtn, true);           
        else 
            SetBtnSprite(EggLvBtnType.DiaBtn, DiaBtn, false);
    }

    private void SetLvUpMode()
    {
        Desc.text = descs[1];
        SaveManager.Instance.SetFieldData(nameof(userData.isLvUpMode), true);

        Gauges.gameObject.SetActive(false);
        GaugeAndLvUp.SetActive(false);

        if (userData.eggLv < 10)
        {
            Clock.SetActive(true);
            Skip.SetActive(true);
            SetDiaBtn();

            if(userData.eggLv < 9)
            {
                price = Calculater.CalPrice(userData.eggLv + 1, 10000, 17000, 12000);
                PriceTxt.text = Calculater.NumFormatter(price);
            }
        }     

        totalTime = DataManager.Instance.GetData<EggRateData>(userData.eggLv.ToString()).lvUpTime;
        // 앱이 다시 시작될 때 경과된 시간 계산
        TimeSpan elapsedTime = DateTime.Now - DateTime.Parse(userData.startLvUpTime);
        remainingTime = totalTime - (float)elapsedTime.TotalSeconds - userData.skipTime;
        updateTimerCoroutine = StartCoroutine(UpdateTimer());
    }

    private void SetGaugeMode()
    {
        SaveManager.Instance.SetFieldData(nameof(userData.isLvUpMode), false);

        Clock.SetActive(false);
        Skip.SetActive(false);

        if (userData.eggLv == 10)
        {
            Desc.text = descs[2];
        }
        else if (userData.eggLv < 10)
        {
            Desc.text = descs[0];
            Gauges.gameObject.SetActive(true);
            GaugeAndLvUp.SetActive(true);
            SetLvUpBtn();
            SetGaugeUpBtn();
        }
    }

    public void OnClickGaugeUpBtn()
    {
        if (userData.gold < price) return;
        lvUpGauges[userData.fullGaugeCnt].GaugeUp();
        SaveManager.Instance.SetFieldData(nameof(userData.fullGaugeCnt), 1, true);
        SaveManager.Instance.SetFieldData(nameof(userData.gold), -price, true);
        SetLvUpBtn();
    }

    public void OnClickLvUpBtn()
    {
        SaveManager.Instance.SetFieldData(nameof(userData.startLvUpTime), DateTime.Now.ToString());
        SaveManager.Instance.SetFieldData(nameof(userData.fullGaugeCnt), 0);
        foreach (var gauge in lvUpGauges)
            gauge.ResetGauge();
        if ((userData.eggLv + 1) % 5 == 0)
            lvUpGauges.Add(Instantiate(LvUpGauge, Gauges));

        SetLvUpMode();
        EggLvGuide();
    }

    public void OnClickAdBtn(float decTime)
    {
        SaveManager.Instance.SetFieldData(nameof(userData.adsCount), --userData.adsCount);
        SaveManager.Instance.SetFieldData(nameof(userData.skipTime), userData.skipTime + decTime);
        adsSkipCountTxt.text = string.Format("{0}/4", userData.adsCount);
        remainingTime -= decTime;
        UpdateTimerText();
        if (remainingTime <= 0) LvUp();
    }

    public void OnClickDiaBtn()
    {
        if (userData.diamond < skipDia) return;
        SaveManager.Instance.SetFieldData(nameof(userData.diamond), -skipDia, true);
        SaveManager.Instance.SetFieldData(nameof(userData.skipTime), userData.skipTime + skipDiaTime);
        remainingTime -= skipDiaTime;
        UpdateTimerText();
        if (remainingTime <= 0) LvUp();
    }

    private void LvUp()
    {
        if(updateTimerCoroutine != null)
            StopCoroutine(updateTimerCoroutine);
        remainingTime = 0;
        UpdateTimerText();

        SaveManager.Instance.SetFieldData(nameof(userData.skipTime), 0);
        SaveManager.Instance.SetFieldData(nameof(userData.startLvUpTime), null);
        if(userData.eggLv < 10)
            SaveManager.Instance.SetFieldData(nameof(userData.eggLv), 1, true);    
        SetGaugeMode();

        if (QuestManager.Instance.IsMyTurn(QuestType.Nest))
        {
            QuestManager.Instance.OnQuestEvent();
        }
    }

    private IEnumerator UpdateTimer()
    {
        while (remainingTime > 1f)
        {
            remainingTime -= 1f; // 1초마다 감소
            UpdateTimerText();
            yield return oneSecTime; // 1초 대기
        }
        remainingTime = 0;
        UpdateTimerText();
        LvUp();
    }

    private void UpdateTimerText()
    {
        int hours = Mathf.FloorToInt(remainingTime / 3600f);
        int minutes = Mathf.FloorToInt((remainingTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        TimeTxt.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public void EggLvGuide()
    {
        if (userData.eggLv > 1 || userData.isLvUpMode)
        {
            isGuide = false;
            GuideManager.Instance.GuideArrow.SetActive(false);
            return;
        }

        isGuide = true;
        if (userData.fullGaugeCnt == lvUpGauges.Count)
        {
            GuideManager.Instance.SetArrow(LvUpBtn.gameObject, 20f);
        }
        else
        {
            GuideManager.Instance.SetArrow(GaugeUpBtn.gameObject, 20f);
        }
    }
}
