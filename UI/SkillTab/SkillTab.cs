using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTab : UIBase, IPointerDownHandler
{
    #region UI
    [SerializeField] private Toggle ownToggle;
    [SerializeField] private GameObject equipOverlay;
    public Image evolveIcon;
    public Sprite[] btnColor;
    #endregion

    #region 슬롯, 팝업
    [SerializeField] private UISkillPopUp infoPopUp;
    //픽셀몬 슬롯 프리팹
    [SerializeField]private SkillSlot slotPrefab;
    //전체 슬롯 부모 오브젝트 위치
    [SerializeField]private Transform contentTr;
    #endregion

    #region 매니저
    public SaveManager saveManager;
    [SerializeField]
    private SkillManager skillManager;
    [SerializeField]
    private DataManager dataManager;
    #endregion

    #region Info
    [Header("Info")]
    public UserData userData;
    //전체 스킬 정보
    public List<SkillSlot> allData = new List<SkillSlot>();
    //미보유 스킬 정보
    public List<SkillSlot> noneData = new List<SkillSlot>();
    //보유한 스킬 정보
    public List<SkillSlot> ownedData = new List<SkillSlot>();
    //편성된 스킬 정보
    public List<SkillEquipSlot> equipData = new List<SkillEquipSlot>();
    [SerializeField]
    private PixelmonLayout pxmLayout;
    #endregion

    public UnityAction<int> AddSkillAction;
    public UnityAction<int, int> InsertSlotAcion;
    public UnityAction<int> ClearSlotAcion;
    public TabState tabState = TabState.Normal;
    public PixelmonLayout skiltabLayout;
    public Color[] bgIconColor;
    public TMP_ColorGradient[] txtColors;
    public SkillSlot choiceSlot;
    public int choiceId;
    public bool isAdvancable;
 


    protected override async void Awake()
    {
        saveManager = SaveManager.Instance;
        userData = saveManager.userData;
        dataManager = DataManager.Instance;
        skillManager = SkillManager.Instance;
        skillManager.skillTab = this;
        AddSkillAction += AddSkill;
        InsertSlotAcion += InsertEquipSlot;
        ClearSlotAcion += ClearEquipSlot;
        InitTab();
        infoPopUp = await UIManager.Show<UISkillPopUp>();
    }

    public void InitTab()
    {
        for (int i = 0; i < dataManager.activeData.data.Count; i++)
        {
            SkillSlot slot = Instantiate(slotPrefab, contentTr);
            slot.InitSlot(this, dataManager.activeData.data[i]);
            if (userData.ownedSkills.Count != 0 && slot.atvData.dataIndex != -1)
            {
                slot.myAtvData = userData.ownedSkills[slot.atvData.dataIndex];
                slot.UpdateSlot();
                ownedData.Add(slot);
            }
            else
            {
                noneData.Add(slot);
            }
            allData.Add(slot);
        }

        for (int i = 0; i < equipData.Count; i++)
        {
            if (userData.equippedPxms.Length > i && userData.equippedSkills[i] != -1)
            {
                equipData[i].myPxmData = userData.equippedPxms[i];
                equipData[i].myAtvData = allData[userData.equippedSkills[i]].myAtvData;
            }
            equipData[i].skillTab = this;
        }

        OnOwnedToggle();
        CheckedLvUp();
    }

    public void OnOwnedToggle()
    {
        OnClosePopUp();
        foreach (var data in noneData)
        {
            data.gameObject.SetActive(!ownToggle.isOn);
        }
    }

    public void OnSkillPopUp(int id)
    {
        choiceId = id;
        infoPopUp.gameObject.SetActive(true);
        infoPopUp.ShowPopUp(allData[id], this);
    }

    public void OnEquip(int id)
    {
        if (!allData[choiceId].myAtvData.isOwned)
        {
            //미보유 안내문구
            UIManager.Instance.ShowWarn("미 보유 상태입니다.");
            return;
        }

        if (allData[choiceId].myAtvData.isEquipped)
        {
            CheckedOverlap(allData[id].myAtvData.id);
        }
        else if (allData[choiceId].myAtvData.isAttached)
            UnAttachedAction(allData[id].myAtvData.pxmId, id);
        else
        {
            tabState = TabState.Equip;
            infoPopUp.gameObject.SetActive(false);
            equipOverlay.SetActive(true);
        }
            
    }

    public void InsertEquipSlot(int index, int id)
    {
        equipData[index].EquipAction(allData[id].atvData, allData[id].myAtvData, index);
        allData[id].SetEquipTxt();
    }

    public void ClearEquipSlot(int index)
    {
        equipData[index].UnEquipAction();
    }

    public void UnAttachedAction(int pxmId, int skillId)
    {
        var pxmData = PixelmonManager.Instance.pxmTab.allData[pxmId];
        allData[skillId].myAtvData.isEquipped = false;
        saveManager.UpdateSkillData(allData[skillId].atvData.dataIndex, "isEquipped", allData[skillId].myAtvData.isEquipped);
        pxmData.myPxmData.atvSkillId = -1;
        saveManager.UpdatePixelmonData(pxmId, nameof(pxmData.myPxmData.atvSkillId), pxmData.myPxmData.atvSkillId);
        allData[skillId].myAtvData.isAttached = false;
        saveManager.UpdateSkillData(allData[skillId].atvData.dataIndex, "isAttached", allData[skillId].myAtvData.isAttached);
        allData[skillId].myAtvData.pxmId = -1;
        saveManager.UpdateSkillData(allData[skillId].atvData.dataIndex, "pxmId", allData[skillId].myAtvData.pxmId);
        allData[skillId].SetEquipTxt();
    }

    public void CheckedOverlap(int id)
    {
        foreach (var data in equipData)
        {
            if (data.myAtvData != null && data.myAtvData.id == id)
            {
                skillManager.UnEquipSkill(data.slotIndex);
                break;
            }
        }
    }

    public void OnCancelEquip()
    {
        tabState = TabState.Normal;
        infoPopUp.gameObject.SetActive(false);
        equipOverlay.SetActive(false);
    }

    public void AddSkill(int index)
    {
        if (!ownedData.Contains(allData[index]))
        {
            allData[index].gameObject.SetActive(true);
            allData[index].atvData.dataIndex = userData.ownedSkills.Count - 1;
            allData[index].myAtvData.isOwned = true;
            ownedData.Add(allData[index]);
            noneData.Remove(allData[index]);
        }
        allData[index].UpdateSlot();
    }

    public void CheckedLvUp()
    {
        if (isAdvancable)
        {
            evolveIcon.sprite = btnColor[1];
        }
        else
        {
            foreach (var data in ownedData)
            {
                if (data.myAtvData.isAdvancable)
                {
                    isAdvancable = true;
                    evolveIcon.sprite = btnColor[1];
                    return;
                }
            }
            if (!isAdvancable)
                evolveIcon.sprite = btnColor[0];
        }
    }


    public void OnAllLvUp()
    {
        OnClosePopUp();
        if (!isAdvancable) return;
        foreach (var data in ownedData)
        {
            if (data.myAtvData.isAdvancable)
            {
                data.OnEvolved();
                //Debug.Log($"{data.myAtvData.id} 합성완료");
            }
        }
        isAdvancable = false;
        evolveIcon.sprite = btnColor[0];
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20005").clip);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
#if UNITY_EDITOR
        pointer.position = Input.mousePosition;
#else
        pointer.position = Input.touches[0].position;
#endif
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            if (!raycastResults[0].gameObject.TryGetComponent<SkillSlot>(out choiceSlot))
            {
                OnClosePopUp();
            }
        }
    }

    private void OnDisable()
    {
        OnClosePopUp();
    }

    public void OnClosePopUp()
    {
        if (infoPopUp != null && infoPopUp.gameObject.activeSelf)
            infoPopUp.SetActive(false);
    }
}
