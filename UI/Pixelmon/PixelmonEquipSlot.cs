using UnityEngine.UI;

public class PixelmonEquipSlot : PixelmonSlot
{
    public Player player => Player.Instance;
    public PixelmonManager pxmManager => PixelmonManager.Instance;
    public SkillManager skillManager => SkillManager.Instance;
    public UIManager uiManager => UIManager.Instance;
    public SaveManager saveManager;
    public bool isLocked = true;
    public Image stateIcon;

    private void Awake()
    {
        saveManager = SaveManager.Instance;
    }

    private void Start()
    {
        slotBtn.onClick.AddListener(OnClick);
        ChangedInfo();      
    }

    public void ChangedInfo()
    {
        slotIndex = gameObject.transform.GetSiblingIndex();
        isLocked = pxmtab.userData.isLockedSlot[slotIndex];
        if (isLocked) return;
        stateIcon.sprite = pxmManager.plusIcon;
        MyPixelmonData datas = pxmtab.userData.equippedPxms[slotIndex];

        if (datas != null && datas.isEquipped)
        {
            myPxmData = datas;
            Equip(myPxmData);
        }
        else
            myPxmData = null;
    }

    public override void InitSlot(UIPixelmonTab tab, PixelmonData data)
    {
        base.InitSlot(tab, data);
    }

    public void Equip(MyPixelmonData myData)
    { 
        myData.isEquipped = true;
        myPxmData = myData;
        pxmData = pxmManager.FindPixelmonData(myData.id);
        slotIcon.gameObject.SetActive(true);
        slotIcon.sprite = pxmData.icon;
        slotIconBg.sprite = pxmData.bgIcon;
        stateIcon.gameObject.SetActive(false);
        lvTxt.gameObject.SetActive(true);
        lvTxt.text = string.Format("Lv.{0}", myData.lv);

        saveManager.userData.equippedPxms[slotIndex] = myData;
        saveManager.SetData("equippedPxms", saveManager.userData.equippedPxms);
    }

    public void UnEquip()
    {
        if (myPxmData.atvSkillId != -1)
        {
            skillManager.skillTab.equipData[slotIndex].myAtvData.isEquipped = false;
            saveManager.UpdateSkillData(
                skillManager.skillTab.equipData[slotIndex].atvData.dataIndex,
                "isEquipped", skillManager.skillTab.equipData[slotIndex].myAtvData.isEquipped);
            skillManager.skillTab.equipData[slotIndex].SetEquipSlot();
            skillManager.skillTab.allData[myPxmData.atvSkillId].SetEquipTxt();
        }
        pxmData = null;
        myPxmData = null;
        slotIcon.gameObject.SetActive(false);
        slotIconBg.sprite = pxmManager.defaultBg;
        stateIcon.gameObject.SetActive(true);
        lvTxt.gameObject.SetActive(false);


        saveManager.userData.equippedPxms[slotIndex] = null;
        saveManager.SetData("equippedPxms", saveManager.userData.equippedPxms);
    }

    public override void OnClick()
    {
        if (isLocked)
        {
            ShowUnlockInfo();
            return;
        }

        if (myPxmData == null && pxmtab.tabState != TabState.Equip)
        {
            uiManager.ShowWarn("슬롯이 비어있습니다!");
            return;
        }
        else if (myPxmData == null && pxmtab.tabState == TabState.Equip)
        {
            if (GuideManager.Instance.guideNum == GuideManager.Instance.equipPixelmon)
            {
                QuestManager.Instance.OnQuestEvent();
                GuideManager.Instance.GuideArrow.SetActive(false);
                saveManager.SetFieldData(nameof(saveManager.userData.tutoIndex), 4);
                UIManager.Get<UIPixelmonTab>().isGuideOn = false;
            }
            pxmtab.EquipedPixelmon(slotIndex);
        }
        else if (pxmtab.tabState == TabState.Equip)
        {
            if (myPxmData.isEquipped)
                pxmtab.UnEquipSlot(slotIndex, myPxmData.id);
            pxmtab.EquipedPixelmon(slotIndex);
        }
        else if(pxmtab.tabState != TabState.Equip)
        {
            base.OnClick();
        }
    }

    public void ShowUnlockInfo()
    {
        switch (slotIndex)
        {
            case 2:
                uiManager.ShowWarn("유저 레벨 10Lv에 해금");
                return;
            case 3:
                uiManager.ShowWarn("유저 레벨 50Lv에 해금");
                return;
            case 4:
                uiManager.ShowWarn("유저 레벨 100Lv에 해금");
                return;
            default:
                return;
        }
    }

    public void OnPlaySound()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20012").clip);
    }
}
