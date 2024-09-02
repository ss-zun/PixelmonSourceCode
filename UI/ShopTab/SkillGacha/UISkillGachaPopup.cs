using DG.Tweening;
using TMPro;
using UnityEngine;

public class UISkillGachaPopup : UIBase
{
    [SerializeField] private SkillGachaSlot slotPrefab;
    [SerializeField] private Transform grid;

    [SerializeField] private SkillGachaBtn FiveBtn;
    [SerializeField] private SkillGachaBtn FifteenBtn;
    [SerializeField] private SkillGachaBtn ThirtyBtn;

    private const int maxSlotCnt = 30;

    private SkillGachaSlot[] slot = new SkillGachaSlot[maxSlotCnt];
    private bool isInitSlots;

    private SkillGacha skillGacha;

    private void Start()
    {
        UIManager.Instance.UpdateUI += UpdateCostUI;
    }

    private void UpdateCostUI(DirtyUI dirtyUI)
    {
        if (dirtyUI == DirtyUI.Diamond || dirtyUI == DirtyUI.SkillTicket)
        {
            SetBtnInteractable();
        }
    }

    private void SetBtnInteractable()
    {
        skillGacha.UpdateButnInteractable(FiveBtn, 5);
        skillGacha.UpdateButnInteractable(FifteenBtn, 15);
        skillGacha.UpdateButnInteractable(ThirtyBtn, 30);
    }

    private void InitSlots()
    {
        for (int i = 0; i < maxSlotCnt; i++)
        {
            slot[i] = Instantiate(slotPrefab, grid);
        }
        isInitSlots = true;
    }

    public bool SetPopup(int slotCnt, ActiveData[] datas, SkillGacha skillGacha)
    {
        bool isHigh = false;
        this.skillGacha = skillGacha;
        SetBtnInteractable();

        if (!isInitSlots)
            InitSlots();

        for (int i = 0;i < slotCnt; i++)
        {
            if (slot[i].SRank.gameObject.activeInHierarchy) slot[i].SRank.gameObject.SetActive(false);
            if (slot[i].SSRank.gameObject.activeInHierarchy) slot[i].SSRank.gameObject.SetActive(false);

            slot[i].gameObject.SetActive(true);
            slot[i].InitSlot(datas[i].bgIcon, datas[i].icon, skillGacha.slotDurationTime);

            if (datas[i].rank == "S")
            {
                isHigh = true;
                slot[i].SRank.gameObject.SetActive(true);
            }
            else if (datas[i].rank == "SS")
            {
                isHigh = true;
                slot[i].SSRank.gameObject.SetActive(true);
            }
        }
        for (int i = slotCnt; i < maxSlotCnt; i++)
            slot[i].gameObject.SetActive(false);

        return isHigh;
    }

    public void OnClickBtn(int multiplier)
    {
        skillGacha.OnClickBtn(multiplier);
    }
}
