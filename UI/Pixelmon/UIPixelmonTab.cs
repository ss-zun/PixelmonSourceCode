using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPixelmonTab : UIBase
{
    [Header("UI")]
    [SerializeField] private Toggle prossessToggle;
    [SerializeField] private TextMeshProUGUI foodCountTxt;
    [SerializeField] private TextMeshProUGUI ownSkillEffectTxt;
    [SerializeField] private GameObject allSetBtn;
    public Image evolveIcon;
    public Sprite[] btnColor;

    public RectTransform equipOverlay;
    public int choiceId;
    public bool isAdvancable;
    public TabState tabState;

    public UnityAction<int> unLockAction;
    #region 슬롯, 팝업
    [SerializeField] private UIPixelmonPopUp infoPopUp;
    //픽셀몬 슬롯 프리팹
    [SerializeField] private PixelmonSlot slotPrefab;
    //전체 슬롯 부모 오브젝트 위치
    [SerializeField] private Transform contentTr;
    #endregion

    #region 매니저
    public SaveManager saveManager;
    private PixelmonManager pixelmonManager;
    private DataManager dataManager;
    private GuideManager guideManager;
    #endregion

    #region Info
    [Header("Info")]
    public UserData userData;
    //전체 픽셀몬 정보
    public List<PixelmonSlot> allData = new List<PixelmonSlot>();
    //미보유 픽셀몬 정보
    public List<PixelmonSlot> noneData = new List<PixelmonSlot>();
    //보유한 픽셀몬 정보
    public List<PixelmonSlot> ownedData = new List<PixelmonSlot>();
    //편성된 픽셀몬 정보
    public PixelmonEquipSlot[] equipData = new PixelmonEquipSlot[5];
    #endregion

    public bool isGuideOn = false;

    protected override async void Awake()
    {
        saveManager = SaveManager.Instance;
        userData = saveManager.userData;
        pixelmonManager = PixelmonManager.Instance;
        dataManager = DataManager.Instance;
        guideManager = GuideManager.Instance;
        pixelmonManager.pxmTab = this;
        unLockAction += GetPixelmon;
        equipOverlay.localScale = new Vector2(Screen.width, Screen.height);
        //equipOverlay.SetActive(false);
        InitTab();
        infoPopUp = await UIManager.Show<UIPixelmonPopUp>();
    }

    private void OnEnable()
    {
        SetfoodCount();
    }

    private void OnDisable()
    {
        if (isGuideOn)
        {
            guideManager.GuideNumTrigger(guideManager.guideNum);
        }
    }

    public void InvokePixelmonTabGuide()
    {
        isGuideOn = true;
        StartCoroutine(DelayPxmGuide());
    }

    private IEnumerator DelayPxmGuide()
    {
        yield return null;
        switch (guideManager.guideNum)
        {
            case 1:
                guideManager.SetArrow(allData[0].gameObject, 40f);
                break;
            case 3:
                guideManager.SetArrow(allSetBtn);
                break;
        }
    }

    public void InitTab()
    {
        int index = 0;
        for (int i = 0; i < dataManager.pixelmonData.data.Count; i++)
        {
            PixelmonSlot slot = Instantiate(slotPrefab, contentTr);
            slot.InitSlot(this, dataManager.pixelmonData.data[i]);
            if (userData.ownedPxms.Count > i && userData.ownedPxms[i].isOwned)
            {
                slot.myPxmData = userData.ownedPxms[index++];
                ownedData.Add(slot);
                slot.UpdateSlot();
            }
            else
            {
                noneData.Add(slot);
            }
            allData.Add(slot);
        }

        for (int i = 0; i < equipData.Length; i++)
        {
            if (userData.equippedPxms.Length > i)
            {
                equipData[i].myPxmData = userData.equippedPxms[i];
            }
            equipData[i].pxmtab = this;
        }

        OnProssessionToggle();
        SetfoodCount();
        CheckedEvolve();
        InitInfo();
    }

    public void InitInfo()
    {
        ownSkillEffectTxt.text = $"캐릭터의 HP {pixelmonManager.perHp:N0}%, 방어력 {pixelmonManager.perDef:N0}% 증가";
    }

    public void CheckedEvolve()
    {
        if (isAdvancable)
            evolveIcon.sprite = btnColor[1];
        else
        {
            foreach (var data in ownedData)
            {
                if (data.myPxmData.isAdvancable)
                {
                    isAdvancable = true;
                    evolveIcon.sprite = btnColor[1];
                }
            }
            if(!isAdvancable)
                evolveIcon.sprite = btnColor[0];
        }
    }

    public void SetfoodCount()
    {
        foodCountTxt.text = userData.food.ToString();
    }

    public void OnProssessionToggle()
    {
        foreach (PixelmonSlot data in noneData)
        {
            data.gameObject.SetActive(!prossessToggle.isOn);
        }
    }

    public void OnArrangeAll()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20002").clip);
        if (ownedData.Count == 0) return;

        int arrCount = 0;
        for (int i = 0; i < equipData.Length; i++)
        {
            if (!equipData[i].isLocked)
            {
                arrCount++;
                if (equipData[i].myPxmData != null && equipData[i].myPxmData.isEquipped)
                {
                    UnEquipSlot(equipData[i].slotIndex, equipData[i].myPxmData.id);
                }
            }
        }

        if(arrCount == 0) return;

        float[] weight = new float[ownedData.Count];
        float[] bestPxm = new float[arrCount];

        for(int i = 0; i < ownedData.Count; i++) 
        {
            weight[i] = (ownedData[i].myPxmData.atkValue + ownedData[i].myPxmData.lv * ownedData[i].pxmData.lvAtkRate) +
                ownedData[i].pxmData.traitValue;
            for (int j = 0; j < ownedData[i].myPxmData.psvSkill.Count; j++)
            {
                float aa = UIUtils.GetPsvWeight(ownedData[i].myPxmData.psvSkill[j].psvName);
                weight[i] = weight[i] + ownedData[i].myPxmData.psvSkill[j].psvValue * aa;
            }

            if (bestPxm[0] == 0 || bestPxm[0] <= weight[i])
            {
                bestPxm[0] = weight[i];
                Array.Sort(bestPxm);
            }
        }

        Array.Reverse(bestPxm);
        for(int i = 0; i < arrCount; i++) 
        {
            if (!equipData[i].isLocked && bestPxm[i] != 0)
            {
                tabState = TabState.Equip;
                choiceId = ownedData[weight.ToList().FindIndex((obj) => obj == bestPxm[i])].myPxmData.id;
                equipData[i].OnClick(); 
            }
        }
        
        if (guideManager.guideNum == guideManager.setAllPixelmon)
        {
            QuestManager.Instance.OnQuestEvent();

            if (isGuideOn)
            {
                guideManager.GuideArrow.SetActive(false);
                isGuideOn = false;
            }
        }
    }

    public void OnAutoEvolved()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20002").clip);
        if (!isAdvancable) return;
        foreach (var data in ownedData)
        {
            while (data.myPxmData.isAdvancable)
            {
                if (data.myPxmData.star >= 5) break;
                data.OnEvolved();
            }
            //Debug.Log($"{data.myPxmData.id} 합성완료");
        }
        isAdvancable = false;
        evolveIcon.sprite = btnColor[0];
    }

    public void GetPixelmon(int index)
    {
        if (!ownedData.Contains(allData[index]))
        {
            pixelmonManager.UpdatePlayerStat(
                allData[index].pxmData.basePerHp, allData[index].pxmData.basePerDef);
            allData[index].gameObject.SetActive(true);
            allData[index].myPxmData.isOwned = true;
            ownedData.Add(allData[index]);
            noneData.Remove(allData[index]);
        }
        allData[index].UpdateSlot();
    }

    public void OnHideOverlay()
    {
        tabState = TabState.Normal;
        equipOverlay.gameObject.SetActive(false);
        infoPopUp.gameObject.SetActive(false);

        if (isGuideOn)
        {
            switch (guideManager.guideNum)
            {
                case 1:
                    guideManager.SetArrow(allData[0].gameObject, 40f);
                    break;
            }
        }
    }
    
    public void OnEquip()
    {
        if (tabState == TabState.Equip) 
        {
            equipOverlay.gameObject.SetActive(true);
            if (isGuideOn && guideManager.guideNum == 1)
            {
                guideManager.SetArrow(equipData[0].gameObject, 40f);
            }
        }
        else if(tabState == TabState.UnEquip) 
        {
            for (int i = 0; i < equipData.Length; i++)
            {
                if (equipData[i].pxmData == allData[choiceId].pxmData)
                {
                    UnEquipSlot(i, choiceId);
                    break;
                }
            }
            tabState = TabState.Normal;
        }
    }

    public void EquipedPixelmon(int slotIndex)
    {//TODO: Equip Guide
        equipData[slotIndex].Equip(allData[choiceId].myPxmData);
        pixelmonManager.equipAction?.Invoke(slotIndex, equipData[slotIndex].myPxmData);

        if (allData[choiceId].myPxmData.atvSkillId != -1)
            SkillManager.Instance.skillTab.InsertSlotAcion(slotIndex, allData[choiceId].myPxmData.atvSkillId);
        allData[choiceId].equipIcon.SetActive(true);
        equipOverlay.gameObject.SetActive(false);        
        tabState = TabState.Normal;
    }

    public void UnEquipSlot(int slotIndex, int choiceId)
    {
        allData[choiceId].myPxmData.isEquipped = false;
        allData[choiceId].equipIcon.SetActive(false);
        pixelmonManager.unEquipAction?.Invoke(slotIndex);
        equipData[slotIndex].UnEquip();
    }

    public void OnInfoPopUp(int id)
    {
        choiceId = id;
        infoPopUp.gameObject.SetActive(true);
        infoPopUp.ShowPopUp(choiceId, this);
    }
}

