using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening.Core.Easing;

public class SkillSlot : MonoBehaviour
{
    public SkillTab skillTab;
    public ActiveData atvData;
    public MyAtvData myAtvData;

    #region 슬롯정보
    [SerializeField] protected int slotIndex;
    [SerializeField]
    protected RectTransform rectTr;
    [SerializeField]
    protected Button slotBtn;
    [SerializeField]
    protected Image slotIcon;
    [SerializeField]
    protected Image slotIconBg;

    #endregion

    #region UI
    public TextMeshProUGUI skillRankTxt;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private TextMeshProUGUI equipIcon;
    [SerializeField] private TMP_ColorGradient[] equipGradient;
    [SerializeField] protected TextMeshProUGUI skillLv;
    [SerializeField] private Slider evolveSldr;
    [SerializeField] private Image evolveBarImg;
    [SerializeField] private TextMeshProUGUI evolvedCount;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if(slotBtn != null )
            slotBtn.onClick.AddListener(() => { OnClick(); });
    }

    public void OnClick()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20012").clip);
        skillTab.OnSkillPopUp(atvData.id);
    }

    public void EquipAction()
    {
        equipIcon.gameObject.SetActive(true);
        myAtvData.isEquipped = true;
    }

    public void UnEquipAction()
    {
        myAtvData.isEquipped = false;
        equipIcon.gameObject.SetActive(false);
    }

    public void InitSlot(SkillTab tab, ActiveData data)
    {
        skillTab = tab;
        atvData = data;        
        slotIndex = atvData.id;
        SetRankTxt();
        SetSkillIcon();
        atvData.dataIndex = SaveManager.Instance.userData.ownedSkills.FindIndex((obj) => obj.id == atvData.id);
    }

    public void UpdateSlot()
    {
        myAtvData = SaveManager.Instance.userData.ownedSkills[atvData.dataIndex];
        SetSkillLv();
        SetEquipTxt();
        SetEvolveSldr();
        lockIcon.SetActive(false);
        SetBgColor(atvData.rank);
    }

    public void OnEvolved()
    {
        while (myAtvData.isAdvancable)
        {
            skillTab.saveManager.UpdateSkillData(atvData.dataIndex, "isAdvancable", false);
            skillTab.saveManager.UpdateSkillData(atvData.dataIndex, "evolvedCount", myAtvData.evolvedCount - UIUtils.GetEvolveValue(myAtvData, atvData));
            skillTab.saveManager.UpdateSkillData(atvData.dataIndex, "lv", ++myAtvData.lv);
            SetEvolveSldr();
        }
        SetSkillLv();
    }

    public void SetRankTxt()
    {
        skillRankTxt.colorGradientPreset = GetRankColor(atvData.rank);
        skillRankTxt.text = atvData.rank;
    }
    public void SetSkillLv()
    {
        skillLv.text = myAtvData.lv.ToString();
    }

    public void SetEquipTxt()
    {
        if (myAtvData.isEquipped)
        {
            equipIcon.gameObject.SetActive(true);
            equipIcon.colorGradientPreset = equipGradient[0];
        }
        else if (myAtvData.isAttached)
        {
            equipIcon.gameObject.SetActive(true);
            equipIcon.colorGradientPreset = equipGradient[1];
        }
        else equipIcon.gameObject.SetActive(false);
    }

    public void SetSkillIcon()
    {
        slotIcon.sprite = atvData.icon;
        slotIconBg.sprite = atvData.bgIcon;
    }

    public void SetEvolveSldr()
    {
        int maxNum = UIUtils.GetEvolveValue(myAtvData, atvData);
        evolveSldr.maxValue = maxNum;
        evolveSldr.value = myAtvData.evolvedCount;
        evolvedCount.text = string.Format("{0}/{1}", myAtvData.evolvedCount, maxNum);
        if (myAtvData.evolvedCount >= maxNum)
        {
            skillTab.isAdvancable = true;
            skillTab.saveManager.UpdateSkillData(atvData.dataIndex, "isAdvancable", true);
            skillTab.evolveIcon.sprite = skillTab.btnColor[1];
            evolveBarImg.sprite = skillTab.btnColor[1];
        }
        else
        {
            evolveBarImg.sprite = skillTab.btnColor[2];
        }
    }

    public TMP_ColorGradient GetRankColor(string rank)
    {
        switch (rank)
        {
            case "C":
                return skillTab.txtColors[0];
            case "B":
                return skillTab.txtColors[1];
            case "A":
                return skillTab.txtColors[2];
            case "S":
                return skillTab.txtColors[3];
            case "SS":
                return skillTab.txtColors[4];
            default:
                return skillTab.txtColors[0];
        }
    }

    public void SetBgColor(string rank)
    {
        switch (rank)
        {
            case "C":
                slotIconBg.color = skillTab.bgIconColor[0];
                break;
            case "B":
                slotIconBg.color = skillTab.bgIconColor[1];
                break;
            case "A":
                slotIconBg.color = skillTab.bgIconColor[2];
                break;
            case "S":
                slotIconBg.color = skillTab.bgIconColor[3];
                break;
            case "SS":
                slotIconBg.color = skillTab.bgIconColor[4];
                break;
            default:
                break;
        }
    }

}
