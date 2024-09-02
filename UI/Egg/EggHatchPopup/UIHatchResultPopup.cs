using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RateBg : BaseBg
{
}

[System.Serializable]
public class PixelmonBg : BaseBg
{
}

public class UIHatchResultPopup : UIBase
{
    #region 등급 UI
    [Header("등급 UI")]
    [SerializeField] private List<RateBg> rateBgs;
    [SerializeField] private List<PixelmonBg> pixelmonBgs;
    [SerializeField] private TextMeshProUGUI pxmName;
    [SerializeField] private TextMeshProUGUI rateTxt;
    [SerializeField] private Image rateBg;
    [SerializeField] private Image pixelmonBg;
    [SerializeField] private Image pixelmonImg;
    #endregion
    #region 능력치 UI
    [Header("능력치 UI")]
    [SerializeField] private TextMeshProUGUI AtkValueTxt;
    [SerializeField] private TextMeshProUGUI TraitTypeTxt;
    [SerializeField] private TextMeshProUGUI TraitValueTxt;

    [SerializeField] private UIPxmPsv[] UIPsv = new UIPxmPsv[4];

    [SerializeField] private TextMeshProUGUI OwnHpValueTxt;
    [SerializeField] private TextMeshProUGUI OwnDefenseValueTxt;
    #endregion
    [SerializeField] private GameObject CollectBtn;
    [SerializeField] private GameObject OwnedBtn;
    [SerializeField] private Button rePlaceBtn;

    private int lowPsvCnt;
    private EggHatch eggHatch;
    private UserData userData => SaveManager.Instance.userData;

    public UIHatchWarnPopup hatchWarnPopup;

    private async void Start()
    {
        hatchWarnPopup = await UIManager.Show<UIHatchWarnPopup>();
    }

    private void OnDisable()
    {
        if (userData.tutoIndex == 2)
        {
            GuideManager.Instance.GuideNumTrigger(GuideManager.Instance.guideNum);
        }
    }

    public void SetPopup(EggHatch eggHatch)
    {
        SaveManager.Instance.SetFieldData(nameof(userData.isGetPxm), false);
        this.eggHatch = eggHatch;
        #region 소환된 픽셀몬 정보 UI        
        pxmName.text = eggHatch.HatchPxmData.name;
        rateTxt.text = UIUtils.TranslateRank(eggHatch.PxmRank);
        rateBg.sprite = PxmRankImgUtil.GetRankImage(eggHatch.PxmRank, rateBgs.ConvertAll<BaseBg>(bg => (BaseBg)bg));
        pixelmonBg.sprite = PxmRankImgUtil.GetRankImage(eggHatch.PxmRank, pixelmonBgs.ConvertAll<BaseBg>(bg => (BaseBg)bg));
        pixelmonImg.sprite = eggHatch.HatchedPixelmonImg.sprite;
        #endregion

        #region 이미 가지고 있는 픽셀몬인지 체크 & 픽셀몬 능력치 UI 셋팅
        AtkValueTxt.text = eggHatch.HatchPxmData.basePerAtk.ToString("F2");
        TraitTypeTxt.text = eggHatch.HatchPxmData.trait.TranslateTraitString();
        TraitValueTxt.text = eggHatch.HatchPxmData.traitValue.ToString("F2");
        OwnHpValueTxt.text = eggHatch.HatchPxmData.basePerHp.ToString("F2");
        OwnDefenseValueTxt.text = eggHatch.HatchPxmData.basePerDef.ToString("F2");

        if (eggHatch.IsOwnedPxm)
        {
            rePlaceBtn.gameObject.SetActive(true);
            OwnedPxmUI();
        }
        else
        {
            rePlaceBtn.gameObject.SetActive(false);
            FirstPxmUI();
        }      
        #endregion
    }

    public void SetTutorialArrow()
    {
        GuideManager.Instance.SetArrow(CollectBtn, 40f);
    }

    private void OwnedPxmUI()
    {
        lowPsvCnt = 0;

        CollectBtn.SetActive(false);
        OwnedBtn.SetActive(true);

        UIPsv[0].NewPsvRankTxt.gameObject.SetActive(true);
        UIPsv[0].OldPsvValueTxt.gameObject.SetActive(true);
        UIPsv[0].ArrowImg.gameObject.SetActive(true);
        for (int i = 0; i < eggHatch.HatchMyPxmData.psvSkill.Count; i++)
        {
            UIPsv[i].gameObject.SetActive(true);

            UIPsv[i].OldPsvRankTxt.text = eggHatch.HatchMyPxmData.psvSkill[i].psvRank;
            UIPsv[i].PsvNameTxt.text = eggHatch.HatchMyPxmData.psvSkill[i].psvName;
            UIPsv[i].OldPsvValueTxt.text = new StringBuilder().Append(eggHatch.HatchMyPxmData.psvSkill[i].psvValue.ToString("F2")).Append('%').ToString();

            UIPsv[i].NewPsvRankTxt.text = eggHatch.PsvData[i].NewPsvRank.ToString();
            UIPsv[i].NewPsvValueTxt.text = new StringBuilder().Append(eggHatch.PsvData[i].NewPsvValue.ToString("F2")).Append('%').ToString();
            if (eggHatch.PsvData[i].NewPsvValue > eggHatch.HatchMyPxmData.psvSkill[i].psvValue)
                UIPsv[i].NewPsvValueTxt.HexColor("#78FF1E");
            else if(eggHatch.PsvData[i].NewPsvValue < eggHatch.HatchMyPxmData.psvSkill[i].psvValue)
            {
                UIPsv[i].NewPsvValueTxt.HexColor("#FF0A0A"); 
                lowPsvCnt++;
            }             
        }
        for (int i = 3; i >= eggHatch.HatchMyPxmData.psvSkill.Count; i--)
        {
            UIPsv[i].gameObject.SetActive(false);
        }
    }

    private void FirstPxmUI()
    {
        CollectBtn.SetActive(true);
        OwnedBtn.SetActive(false);

        UIPsv[0].PsvNameTxt.text = eggHatch.PsvData[0].PsvName;
        UIPsv[0].OldPsvRankTxt.text = eggHatch.PsvData[0].NewPsvRank.ToString();
        UIPsv[0].NewPsvValueTxt.text = new StringBuilder().Append(eggHatch.PsvData[0].NewPsvValue.ToString("F2")).Append('%').ToString();
        UIPsv[0].NewPsvValueTxt.HexColor("#78FF1E");

        UIPsv[0].NewPsvRankTxt.gameObject.SetActive(false);
        UIPsv[0].OldPsvValueTxt.gameObject.SetActive(false);
        UIPsv[0].ArrowImg.gameObject.SetActive(false);
        for (int i = 1; i <= 3; i++)
        {
            UIPsv[i].gameObject.SetActive(false);
        }
    }

    public void OnClickGetPixelmon(bool isReplaceBtn)
    {
        
        if ((eggHatch.HatchMyPxmData.psvSkill.Count == lowPsvCnt) 
            && isReplaceBtn)
        {
            hatchWarnPopup.SetActive(true);
            hatchWarnPopup.SetPopup(eggHatch);
        }           
        else eggHatch.GetPixelmon(isReplaceBtn);
    }
}
