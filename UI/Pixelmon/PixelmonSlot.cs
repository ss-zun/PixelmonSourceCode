using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PixelmonSlot : MonoBehaviour
{
    private DataManager dataManager;

    #region 슬롯정보
    [SerializeField]
    protected RectTransform rectTr;
    [SerializeField]
    protected Button slotBtn;
    public Image slotIcon;
    public Image slotIconBg;
    #endregion

    #region UI
    public int slotIndex;
    [SerializeField] protected TextMeshProUGUI lvTxt;
    [SerializeField] private Slider evolveSldr;
    [SerializeField] private Image evolveBarImg;
    [SerializeField] private TextMeshProUGUI evolveTxt;
    [SerializeField] protected GameObject[] stars;
    public GameObject equipIcon;
    public TextMeshProUGUI propertyEffectTxt;
    #endregion

    #region 데이터
    public UIPixelmonTab pxmtab;
    public PixelmonData pxmData;
    public MyPixelmonData myPxmData = new MyPixelmonData();
    public bool isOwned => myPxmData.isOwned;
    #endregion

    public virtual void InitSlot(UIPixelmonTab tab, PixelmonData data)
    {
        dataManager = DataManager.Instance;
        pxmtab = tab;
        pxmData = data;
        slotIcon.sprite = pxmData.icon;
        slotIconBg.sprite = pxmData.bgIcon;
        slotBtn.onClick.AddListener(OnClick);
        //if (isLocked) lockIcon.SetActive(false);
    }

    public virtual void UpdateSlot()
    {
        myPxmData = SaveManager.Instance.userData.ownedPxms[pxmData.id];
        slotIcon.color = Color.white;
        slotIconBg.color = Color.white;
        lvTxt.gameObject.SetActive(true);
        evolveSldr.gameObject.SetActive(true);
        
        SetPxmLv();
        SetStars();
        SetEvolveSldr();
        if(myPxmData.isEquipped)
            equipIcon.SetActive(true);
        else equipIcon.SetActive(false);
    }

    public void SetPxmLv()
    {
        lvTxt.text = string.Format("Lv.{0}", myPxmData.lv);
    }

    public void SetStars()
    {
        if (stars.Length > 0)
        {
            for (int i = 0; i < myPxmData.star; i++)
            {
                stars[i].SetActive(true);
            }
        }
    }

    public void SetEvolveSldr()
    {
        int maxNum = UIUtils.GetEvolveValue(myPxmData, pxmData);
        evolveSldr.maxValue = maxNum;
        evolveSldr.value = myPxmData.evolvedCount;        
        evolveTxt.text = string.Format("{0}/{1}", myPxmData.evolvedCount, maxNum);
        if (myPxmData.evolvedCount >= maxNum)
        {
            evolveBarImg.sprite = pxmtab.btnColor[1];
            pxmtab.evolveIcon.sprite = pxmtab.btnColor[1];
            pxmtab.isAdvancable = true;
            pxmtab.saveManager.UpdatePixelmonData(myPxmData.id, "isAdvancable", true);
        }
        else
        {
            evolveBarImg.sprite = pxmtab.btnColor[2];
        }
    }



    public virtual void OnClick()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20012").clip);
        pxmtab.OnInfoPopUp(pxmData.id);
    }

    public void OnEvolved()
    {
        if (myPxmData.star >= 5) return;
        pxmtab.saveManager.UpdatePixelmonData(myPxmData.id, "isAdvancable", false);
        pxmtab.saveManager.UpdatePixelmonData(myPxmData.id, "evolvedCount", myPxmData.evolvedCount - UIUtils.GetEvolveValue(myPxmData, pxmData));
        pxmtab.saveManager.UpdatePixelmonData(myPxmData.id, "star", ++myPxmData.star);
        myPxmData.PxmStarUp();
        SetStars();
        SetEvolveSldr();
        PixelmonManager.Instance.ApplyStatus(pxmData, myPxmData);
    }
}
