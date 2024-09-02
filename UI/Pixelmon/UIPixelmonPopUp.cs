using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UIPixelmonPopUp : UIBase
{
    #region Data
    [SerializeField]
    private UIPixelmonTab pxmTab;
    [SerializeField]
    private PixelmonData data;
    [SerializeField]
    private MyPixelmonData myData;
    private SaveManager saveManager => SaveManager.Instance;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private GameObject overlay;
    [SerializeField] private TextMeshProUGUI rankTxt;
    [SerializeField] private Image rankIcon;
    [SerializeField] private Sprite[] rankSprites;
    [SerializeField] private TextMeshProUGUI pxmNameTxt;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private Image pxmIcon;
    [SerializeField] private TextMeshProUGUI lvTxt;

    [SerializeField] private Button skillBtn;
    [SerializeField] private GameObject[] objSkill;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillName;

    [SerializeField] private TextMeshProUGUI atkValue;
    [SerializeField] private TextMeshProUGUI traitType;
    [SerializeField] private TextMeshProUGUI traitValue;

    [SerializeField] private GameObject[] psvObjects;
    [SerializeField] private TextMeshProUGUI[] psvRank;
    [SerializeField] private TextMeshProUGUI[] psvName;
    [SerializeField] private TextMeshProUGUI[] psvValue;
    [SerializeField] private TextMeshProUGUI[] ownEffectValue;

    [SerializeField] private Button equipBtn;
    [SerializeField] private Image equipImg;
    [SerializeField] private Sprite[] equipSprites;
    [SerializeField] private TextMeshProUGUI equipTxt;


    [SerializeField] private Image feedingBtn;
    [SerializeField] private TextMeshProUGUI curFoodCount;
    [SerializeField] private TextMeshProUGUI demandFoodCount;

    [SerializeField] private Image evolvedBtn;
    [SerializeField] private TextMeshProUGUI curCardCount;
    [SerializeField] private TextMeshProUGUI demandCardCount;

    [SerializeField] private Sprite[] activativeColor;
    [SerializeField] private Button[] infoBtns;
    #endregion

    private string equip = "장착하기";
    private string unEquip = "해제하기";
    private string noneData = "??.??%";
    public int infoIndex = 0;
    public int foodPerExp = 10;
    public int maxExp = 50;
    private string maxLv = "-";
    public void ShowPopUp(int dataIndex, UIPixelmonTab tab)
    {
        pxmTab = tab;
        infoIndex = dataIndex;
        SetData();
        if((dataIndex == 0) && GuideManager.Instance.guideNum == 1)
        {
            GuideManager.Instance.SetArrow(equipBtn.gameObject, 40f);
        }
    }

    public void OnClickOverlay()
    {
        gameObject.SetActive(false);
        pxmTab.InvokePixelmonTabGuide();
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20010").clip);
    }

    public void OnPreviousInfo()
    {
        infoIndex--;
        SetData();
    }
    public void OnNextInfo()
    {
        infoIndex++;
        SetData();
    }

    private void SetData()
    {
        data = pxmTab.allData[infoIndex].pxmData;
        myData = pxmTab.allData[infoIndex].myPxmData;
        pxmTab.choiceId = myData.id;
        equipTxt.text = myData.isEquipped ? unEquip : equip;
        SetChangeBtn();
        InitInfo();
    }

    private void InitInfo()
    {
        var rank = (PxmRank)SetRankSprite(data.rank);
        rankTxt.text = rank.TranslateRank();
        rankIcon.sprite = rankSprites[SetRankSprite(data.rank)];
        pxmNameTxt.text = data.name;
        pxmIcon.sprite = data.icon;
        lvTxt.text = string.Format($"{myData.lv}/50");
        SetStars();
        SetStartEffect(myData.isOwned);
        SetPsvEffect();
        SetOwnedEffect(myData.isOwned);
        SetFoodCount();
        SetEvolvedCount();
        SetPxmSkill();
        CheckedOwn();
    }

    public void CheckedOwn()
    {
        if (myData.isOwned)
        {
            equipBtn.gameObject.SetActive(true);
            if(myData.isEquipped)
                equipImg.sprite = equipSprites[1];
            else
                equipImg.sprite = equipSprites[0];
            evolvedBtn.gameObject.SetActive(true);
            feedingBtn.gameObject.SetActive(true);
            //skillBtn.gameObject.SetActive(true);
        }
        else
        {
            equipBtn.gameObject.SetActive(false);
            evolvedBtn.gameObject.SetActive(false);
            feedingBtn.gameObject.SetActive(false);
            //skillBtn.gameObject.SetActive(false);
        }
    }

    public void OnEquip()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20002").clip);
        if (!myData.isOwned)
        {
            return;
        }
        if (!myData.isEquipped)
        {
            equipImg.sprite = equipSprites[0];
            pxmTab.tabState = TabState.Equip;
            pxmTab.OnEquip();     
        }
        else
        {
            equipImg.sprite = equipSprites[1];
            pxmTab.tabState = TabState.UnEquip;
            pxmTab.OnEquip();
        }
        gameObject.SetActive(false);
    }

    public void OnFeeding()
    {
        if (!myData.isOwned)
            return;
        else if (myData.lv >= 50)
        {
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20008").clip);
            UIManager.Instance.ShowWarn("이미 최대레벨 입니다.");
            return;
        }

        if (saveManager.userData.food >= myData.maxExp)
        {
            saveManager.SetDeltaData("food", -myData.maxExp);
            pxmTab.SetfoodCount();
            SetFoodCount();
            saveManager.UpdatePixelmonData(myData.id, "lv", ++myData.lv);
            lvTxt.text = string.Format($"{myData.lv}/50");
            pxmTab.allData[myData.id].SetPxmLv();
            myData.PxmLvUp();
            pxmTab.allData[infoIndex].SetPxmLv();
            SetStartEffect();
            PixelmonManager.Instance.ApplyStatus(data, myData);

            myData.ownEffectValue[0] += data.lvHpRate;
            myData.ownEffectValue[1] += data.lvDefRate;
            saveManager.UpdatePixelmonData(myData.id, "ownEffectValue", myData.ownEffectValue);
            PixelmonManager.Instance.UpdatePlayerStat(data.lvHpRate, data.lvDefRate);
            SetOwnedEffect();
            pxmTab.SetfoodCount();
            SetFoodCount();
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20005").clip);
        }
        else
        {
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20008").clip);
            UIManager.Instance.ShowWarn("먹이의 개수가 부족합니다.");
        }

        if (QuestManager.Instance.IsMyTurn(QuestType.Feed))
        {
            QuestManager.Instance.OnQuestEvent();
        }
    }

    public void SetChangeBtn()
    {
        if (infoIndex == 0)
        {
            infoBtns[0].gameObject.SetActive(false);
            infoBtns[1].gameObject.SetActive(true);
        }
        else if (infoIndex == pxmTab.allData.Count - 1)
        {
            infoBtns[0].gameObject.SetActive(true);
            infoBtns[1].gameObject.SetActive(false);
        }
        else
        {
            infoBtns[0].gameObject.SetActive(true);
            infoBtns[1].gameObject.SetActive(true);
        }
    }

    public void SetFoodCount()
    {
        if (myData.lv >= 50)
        {
            curFoodCount.text = maxLv;
            demandFoodCount.text = maxLv;
        }
        else
        {
            curFoodCount.text = saveManager.userData.food.ToString();
            demandFoodCount.text = myData.maxExp.ToString();
            if (saveManager.userData.food >= myData.maxExp && myData.isOwned)
            {
                feedingBtn.sprite = activativeColor[0];
                demandFoodCount.color = Color.white;
            }
            else
            {
                feedingBtn.sprite = activativeColor[1];
                demandFoodCount.color = Color.red;
            }
        }
    }

    public void OnEvovled()
    {
        if (!myData.isOwned || !myData.isAdvancable || myData.star >= 5) return;
        pxmTab.allData[myData.id].OnEvolved();
        SetStars();
        SetEvolvedCount();
        SetPsvEffect();
        SetOwnedEffect();
        pxmTab.CheckedEvolve();
        //Debug.Log("합성완료(팝업)");
    }

    public void SetEvolvedCount()
    {
        curCardCount.text = myData.evolvedCount.ToString();
        if (myData.star >= 5)
        {
            demandCardCount.text = maxLv;
            return;
        }

        demandCardCount.text = UIUtils.GetEvolveValue(myData, data).ToString();
        if (myData.evolvedCount >= UIUtils.GetEvolveValue(myData, data))
        {
            evolvedBtn.sprite = activativeColor[0];
            demandCardCount.color = Color.white;
        }
        else
        {
            evolvedBtn.sprite = activativeColor[1];
            demandCardCount.color = Color.red;
        }
    }

    public void SetStars()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < myData.star)
                stars[i].SetActive(true);
            else
                stars[i].SetActive(false);
        }
    }

    public int SetRankSprite(string rank)
    {
        switch (rank)
        {
            case "Common":
                return 0;
            case "Advanced":
                return 1;
            case "Rare":
                return 2;
            case "Epic":
                return 3;
            case "Legendary":
                return 4;
            case "Unique":
                return 5;
            default:
                return 0;
        }
    }

    public void SetStartEffect(bool isOwned = true)
    {
        traitType.text = UIUtils.TranslateTraitString(data.trait);
        if (isOwned)
        {
            atkValue.text = string.Format("{0:F2}%", myData.atkValue + myData.lv * data.lvAtkRate);
            traitValue.text = string.Format("{0:F2}%", data.traitValue + myData.lv * data.lvAtkRate);
        }
        else
        {
            atkValue.text = noneData;
            traitValue.text = noneData;
        }
    }

    public void SetPxmSkill()
    {
        if (myData.atvSkillId != -1)
        {
            objSkill[0].SetActive(true);
            objSkill[1].SetActive(false);
            skillIcon.sprite = SkillManager.Instance.skillTab.allData[myData.atvSkillId].atvData.icon;
            skillName.text = SkillManager.Instance.skillTab.allData[myData.atvSkillId].atvData.name;
        }
        else
        {
            objSkill[0].SetActive(false);
            objSkill[1].SetActive(true);
        }
    }


    public void SetPsvEffect()
    {
        //패시브 최대개수 4개
        for (int i = 0; i < 4; i++)
        {
            if (i < myData.psvSkill.Count && myData.psvSkill[i].psvName != null)
            {
                psvObjects[i].SetActive(true);
                psvRank[i].text = myData.psvSkill[i].psvRank;
                psvName[i].text = myData.psvSkill[i].psvName;
                psvValue[i].text = string.Format("{0:F2}%", myData.psvSkill[i].psvValue);
            }
            else
            {
                psvObjects[i].SetActive(false);
            }
        }
    }

    public void SetOwnedEffect(bool isOwned = true)
    {

        for (int i = 0; i < ownEffectValue.Length; i++)
        {
            if (isOwned)
                ownEffectValue[i].text = string.Format("{0:F2}%", myData.ownEffectValue[i]);
            else
                ownEffectValue[i].text = noneData;
        }
    }
}