using System.Collections.Generic;
using UnityEngine;

public class UpgradeTab : UIBase
{
    [SerializeField] private UpgradeSlot[] upgradeSlots;
    private int[] upgradeLvs => SaveManager.Instance.userData.UpgradeLvs;
    private int lastToggleIndex = 1;

    public Dictionary<UpgradeIndex, QuestType> questMapping = new Dictionary<UpgradeIndex, QuestType>
    {   {UpgradeIndex.Atk, QuestType.UpgradeAtk },
        {UpgradeIndex.Dmg, QuestType.UpgradeDmg},
        {UpgradeIndex.SDmg, QuestType.Null },
        {UpgradeIndex.Cri, QuestType.Null },
        {UpgradeIndex.CriDmg, QuestType.Null },
        {UpgradeIndex.SCri, QuestType.Null },
        {UpgradeIndex.SCriDmg, QuestType.Null }
    };

    protected override void Awake()
    {
        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            upgradeSlots[i].upgradeTab = this;
            upgradeSlots[i].CurLv = upgradeLvs[i];
        }

        UIManager.Instance.UpdateUI += Recalculate;
    }

    private void OnEnable()
    {
        Recalculate(DirtyUI.Gold);
    }

    private void Recalculate(DirtyUI dirty)
    {

        CurrentToggle(lastToggleIndex);
    }

    public void CurrentToggle(int toggleIndex)
    {
        lastToggleIndex = toggleIndex;  
        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            upgradeSlots[i].CalculatePrice(toggleIndex);
        }
    }

    public void PlayBtnSound()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20002").clip);
    }

    public void SaveUpgradeLvs(int index, int curLv)
    {
        int[] newUpgradeLvs = upgradeLvs;
        newUpgradeLvs[index] = curLv;
        SaveManager.Instance.SetData(nameof(UserData.UpgradeLvs), newUpgradeLvs);
    }
}