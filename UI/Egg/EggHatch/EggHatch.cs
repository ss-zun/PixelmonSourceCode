using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggHatch : MonoBehaviour
{
    #region 애니메이션
    public AnimationData AnimData = new AnimationData();
    public Animator BreakAnim;
    public Animator HatchAnim;
    public GameObject HatchAnimGO;
    public AnimationClip BreakClip;
    #endregion

    #region 알뽑기 결과
    public PxmRank PxmRank;
    public Image HatchedPixelmonImg;
    public PixelmonData HatchPxmData;
    public MyPixelmonData HatchMyPxmData;
    public PxmPsvData[] PsvData;
    public bool IsOwnedPxm;

    private WaitUntil getPixelmon;
    private bool isDoneGetPxm;

    private UIHatchResultPopup HatchResultPopup;
    #endregion

    #region 알 자동 뽑기
    public bool isAutoMode;
    public bool isWantStopAuto;
    private bool isHighRankPsvSet;

    private PxmRank autoPxmRank;
    private PsvRank autoPsvRank;

    private WaitForSeconds delayAutoTime;
    public Animator autoBtnAnim;
    #endregion

    private GuideManager guideManager;
    private UserData userData => SaveManager.Instance.userData;

    private async void Awake()
    {
        guideManager = GuideManager.Instance;
        HatchResultPopup = await UIManager.Show<UIHatchResultPopup>();

        if (userData.tutoIndex < 3)
        {
            OnEggTutorial(0);
            guideManager.OnGuideAction += OnEggTutorial;
        }
    }

    private void Start()
    {
        AnimData.Initialize();
        isDoneGetPxm = true;
        getPixelmon = new WaitUntil(() => isDoneGetPxm == true);
        delayAutoTime = new WaitForSeconds(1f);
        HatchedPixelmonImg.gameObject.SetActive(false);

        PsvData = new PxmPsvData[4];
        for (int i = 0; i < PsvData.Length; i++)
            PsvData[i] = new PxmPsvData();

        if (!userData.isGetPxm)
        {
            IsOwnedPxm = userData.isOwnedPxm;
            HatchPxmData = userData.hatchPxmData;
            HatchMyPxmData = userData.hatchMyPxmData;
            PsvData = userData.psvData;
            PxmRank = (PxmRank)Enum.Parse(typeof(PxmRank), HatchPxmData.rank);
            StartCoroutine(SetPxmHatchAnim());
        }
    }

    private void OnEggTutorial(int guideNum)
    {
        if (guideNum == 0)
        {
            guideManager.GuideArrow.SetActive(true);
            guideManager.SetArrow(BreakAnim.gameObject);
        }
    }

    private bool Gacha()
    {
        if (userData.tutoIndex < 3)
        {
            HatchPxmData = DataManager.Instance.pixelmonData.data[0];
            PxmRank = (PxmRank)Enum.Parse(typeof(PxmRank), HatchPxmData.rank);
        }
        else
        {
            #region 확률에 따라 픽셀몬 등급 랜덤뽑기
            PxmRank = PerformPxmGacha(userData.eggLv.ToString());

            // 등급에 해당하는 픽셀몬 랜덤뽑기
            var pxmData = DataManager.Instance.pixelmonData.data;
            List<PixelmonData> randPxmData = new List<PixelmonData>(pxmData.Count);

            for (int i = 0; i < pxmData.Count; i++)
            {
                if (pxmData[i].rank == PxmRank.ToString())
                {
                    randPxmData.Add(pxmData[i]);
                }
            }
            HatchPxmData = randPxmData[UnityEngine.Random.Range(0, randPxmData.Count)];            
            #endregion
        }
        SaveManager.Instance.SetFieldData(nameof(userData.hatchPxmData), HatchPxmData);

        #region 픽셀몬 능력치 랜덤뽑기
        IsOwnedPxm = false;
        foreach (var data in userData.ownedPxms)
        {
            if (HatchPxmData.rcode == data.rcode)
            {
                IsOwnedPxm = true;
                HatchMyPxmData = data;
                SaveManager.Instance.SetFieldData(nameof(userData.hatchMyPxmData), HatchMyPxmData);
                break;
            }
        }
        if (IsOwnedPxm)
            SetNewPsvValue();
        else SetFirstPsvValue();
        SaveManager.Instance.SetFieldData(nameof(userData.psvData), PsvData);
        SaveManager.Instance.SetFieldData(nameof(userData.isOwnedPxm), IsOwnedPxm);
        #endregion
        return true;
    }

    private void SetNewPsvValue()
    {
        for (int i = 0; i < HatchMyPxmData.psvSkill.Count; i++)
        {
            var psvData = DataManager.Instance.GetData<BasePsvData>(HatchMyPxmData.psvSkill[i].psvName);
            var randAbility = RandAbilityUtil.PerformAbilityGacha(HatchMyPxmData.psvSkill[i].psvType, psvData.maxRate, HatchMyPxmData.psvSkill[i].psvValue);
            PsvData[i].NewPsvRank = randAbility.AbilityRank;
            PsvData[i].NewPsvValue = randAbility.AbilityValue;
            if (isAutoMode && ((PsvRank)Enum.Parse(typeof(PsvRank), randAbility.AbilityRank) >= autoPsvRank))
                isHighRankPsvSet = true;
        }
    }

    private void SetFirstPsvValue()
    {
        var basePsvData = RandAbilityUtil.RandAilityData();
        var randAbility = RandAbilityUtil.PerformAbilityGacha((AbilityType)basePsvData.psvEnum, basePsvData.maxRate);
        PsvData[0].PsvType = (AbilityType)basePsvData.psvEnum;
        PsvData[0].PsvName = basePsvData.rcode;
        PsvData[0].NewPsvRank = randAbility.AbilityRank;
        PsvData[0].NewPsvValue = randAbility.AbilityValue;
    }

    public void OnClickEgg(Button btn)
    {
        if (guideManager.guideNum == 0 && userData.tutoIndex == 2)
        {
            UIManager.Get<UIHatchResultPopup>().SetTutorialArrow();
        }
        else if (guideManager.guideNum == 1)
        {
            UIManager.Instance.ShowWarn("튜토리얼을 진행해주세요!");
            return;
        }

        if (!isAutoMode)
            StartCoroutine(ClickEgg(btn));
        else isWantStopAuto = true;
    }

    public IEnumerator ClickEgg(Button btn = null)
    {
        if (userData.eggCount <= 0)
        {
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20008").clip);
            UIManager.Instance.ShowWarn("알이 부족합니다!!");
            isAutoMode = false;
            yield break;
        }

        bool isConditionMet = false;

        if (btn != null)
            btn.interactable = false;

        if (userData.isGetPxm)
        {
            SaveManager.Instance.SetFieldData(nameof(userData.eggCount), -1, true);
            if (QuestManager.Instance.IsMyTurn(QuestType.Egg))
            {
                QuestManager.Instance.OnQuestEvent();
            }
            
            Gacha();
            yield return SetPxmHatchAnim();

            if (guideManager.guideNum == 0 && userData.tutoIndex == 1)
            {
                UIManager.Get<UIHatchResultPopup>().SetTutorialArrow();
                SaveManager.Instance.SetFieldData(nameof(userData.tutoIndex), 2);
            }
            if (isAutoMode && isHighRankPsvSet && (PxmRank >= autoPxmRank))
            {
                isConditionMet = true;
                isHighRankPsvSet = false;
                isAutoMode = false;
            }
        }

        isDoneGetPxm = false;

        if (!IsOwnedPxm)
        {
            isAutoMode = false;
            isHighRankPsvSet = false;
        }

        if (isWantStopAuto)
        {
            isAutoMode = false;
            isWantStopAuto = false;
            isHighRankPsvSet = false;
        }           

        if (!isAutoMode || isConditionMet)
        {
            HatchResultPopup.SetActive(true);
            HatchResultPopup.SetPopup(this);
        }
        else if(isAutoMode && !isConditionMet)
        {
            yield return delayAutoTime;
            GetPixelmon(false);
        }

        if (btn != null)
            btn.interactable = true;

        yield return getPixelmon;      
    }

    public void StartAutoEggHatch(PxmRank autoRank, PsvRank autoPsv)
    {
        StartCoroutine(AutoEggHatch(autoRank, autoPsv));
    }

    public IEnumerator AutoEggHatch(PxmRank autoRank, PsvRank autoPsv)
    {
        autoPxmRank = autoRank;
        autoPsvRank = autoPsv;
        autoBtnAnim.SetBool(AnimData.EggHatchAutoModeParameterHash, true);

        while (isAutoMode)
        {
            yield return ClickEgg();
        }
        autoBtnAnim.SetBool(AnimData.EggHatchAutoModeParameterHash, false);
    }

    private IEnumerator SetPxmHatchAnim()
    {
        HatchAnimGO.SetActive(true);

        // 애니메이션 실행
        BreakAnim.SetInteger(AnimData.EggBreakParameterHash, (int)PxmRank);
        HatchAnim.SetBool(AnimData.EggHatchParameterHash, true);

        // 애니메이션 끝난지 체크
        float startTime = Time.time;
        while (Time.time - startTime < BreakClip.length + 0.08f) yield return null;

        HatchedPixelmonImg.gameObject.SetActive(true);
        HatchedPixelmonImg.sprite = HatchPxmData.icon;
    }

    public PxmRank PerformPxmGacha(string rcode)
    {
        var data = DataManager.Instance.GetData<EggRateData>(rcode);

        float[] probs = { data.common, data.advanced, data.rare, data.epic, data.legendary };

        #region 확률 합이 100인지 체크
        float totalProb = 0;
        foreach (float prob in probs)
        {
            totalProb += prob;
        }
        if (totalProb != 100)
        {
            //Debug.LogError("확률 합 != 100");
        }
        #endregion

        int randProb = UnityEngine.Random.Range(100, 10001);

        float cumProb = 0;
        for (int i = 0; i < probs.Length; i++)
        {
            cumProb += probs[i] * 100;
            if (randProb <= cumProb)
            {
                return (PxmRank)i;
            }
        }

        throw new System.Exception("확률 합 != 100");
    }

    #region HatchResultPopup
    public void GetPixelmon(bool isReplaceBtn)
    {
        if (isReplaceBtn && IsOwnedPxm == true) //교체하기(교체 및 수집)
        {
            ReplacePsv();
        }
        else //수집하기
        {
            if (!IsOwnedPxm)
                GetFirst();
            else
                GetRepetition();
        }

        BreakAnim.SetInteger(AnimData.EggBreakParameterHash, -1);
        HatchAnim.SetBool(AnimData.EggHatchParameterHash, false);

        HatchedPixelmonImg.gameObject.SetActive(false);
        HatchAnimGO.SetActive(false);

        SaveManager.Instance.SetFieldData(nameof(userData.isGetPxm), true);
        isDoneGetPxm = true;

        if (userData.tutoIndex == 2)
        {
            SaveManager.Instance.SetFieldData(nameof(userData.tutoIndex), 3);
            guideManager.guideNum++;
            guideManager.SetBottomLock();
            guideManager.GuideNumTrigger(guideManager.guideNum);
        }
        
        HatchResultPopup.SetActive(false);
    }

    private void GetFirst()
    {
        #region Init Value
        List<PsvSkill> firstPsv = new List<PsvSkill>();
        firstPsv.Add(new PsvSkill
        {
            psvType = PsvData[0].PsvType,
            psvName = PsvData[0].PsvName,
            psvRank = PsvData[0].NewPsvRank,
            psvValue = PsvData[0].NewPsvValue
        });

        float[] ownEffectValue = { HatchPxmData.basePerHp, HatchPxmData.basePerDef };
        #endregion

        #region Update Value
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "rcode", HatchPxmData.rcode);
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "id", HatchPxmData.id);
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "isOwned", true);
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "atkValue", HatchPxmData.basePerAtk);
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "psvSkill", firstPsv);
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "ownEffectValue", ownEffectValue);
        PixelmonManager.Instance.UnLockedPixelmon(HatchPxmData.id);
        #endregion
    }

    private void GetRepetition()
    {
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "evolvedCount", ++userData.ownedPxms[HatchPxmData.id].evolvedCount);
        PixelmonManager.Instance.UnLockedPixelmon(HatchPxmData.id);
    }

    private void ReplacePsv()
    {
        List<PsvSkill> newPsvs = new List<PsvSkill>();
        for (int i = 0; i < HatchMyPxmData.psvSkill.Count; i++)
        {
            newPsvs.Add(new PsvSkill
            {
                psvType = HatchMyPxmData.psvSkill[i].psvType,
                psvName = HatchMyPxmData.psvSkill[i].psvName,
                psvRank = PsvData[i].NewPsvRank,
                psvValue = PsvData[i].NewPsvValue
            });
        }
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "psvSkill", newPsvs);
        SaveManager.Instance.UpdatePixelmonData(HatchPxmData.id, "evolvedCount", ++userData.ownedPxms[HatchPxmData.id].evolvedCount);
        PixelmonManager.Instance.ApplyStatus(PixelmonManager.Instance.pxmTab.allData[HatchPxmData.id].pxmData, 
            PixelmonManager.Instance.pxmTab.allData[HatchPxmData.id].myPxmData);
    }
    #endregion
}
