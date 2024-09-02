using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PixelmonManager : Singleton<PixelmonManager>
{
    public UnityAction<int, MyPixelmonData> equipAction;
    public UnityAction<int> unEquipAction;
    public UnityAction<int> unlockSlotAction;

    private SaveManager saveManager => SaveManager.Instance;
    private UserData userData;
    public Player player => Player.Instance;
    public UIPixelmonTab pxmTab;
    public PixelmonLayout[] layouts;

    private List<PixelmonData> pxmData;
    public Pixelmon[] equippedPixelmon;
    public PixelmonStatus upgradeStatus = new PixelmonStatus();

    public Sprite plusIcon;
    public Sprite defaultBg;

    public float perHp = 0;
    public float perDef = 0;
    // Start is called before the first frame update
    void Start()
    {
        pxmData = DataManager.Instance.pixelmonData.data;
        userData = saveManager.userData;
        equipAction += Equipped;
        unEquipAction += UnEquipped;
        if(saveManager.userData.userLv < 100)
            unlockSlotAction += UnLockedSlots;
        InitUpgradeStatus();
        InitEquippedPixelmon();
        InitPlayerStat();
    }

    public void InitPlayerStat()
    {
        foreach (MyPixelmonData pxm in userData.ownedPxms)
        {
            if (!pxm.isOwned) continue;
            perHp += pxm.ownEffectValue[0];
            perDef += pxm.ownEffectValue[1];
        }
        player.statHandler.UpdateStats(perHp, perDef);
        player.healthSystem.currentHealth = player.statHandler.maxHp;
    }

    public void UpdatePlayerStat(float hp, float def)
    {
        perHp += hp;
        perDef += def;
        player.statHandler.UpdateStats(perHp, perDef, hp/100);
        pxmTab.InitInfo();
    }

    private void InitUpgradeStatus()
    {
        int[] upgradeArr = userData.UpgradeLvs;

        upgradeStatus.Atk = upgradeArr[0];
        upgradeStatus.Cri = upgradeArr[1] * 0.05f;
        upgradeStatus.CriDmg = upgradeArr[2] * 0.5f;
        upgradeStatus.Dmg = upgradeArr[3] * 0.1f;
        upgradeStatus.SDmg = upgradeArr[4] * 0.2f;
        upgradeStatus.SCri = upgradeArr[5] * 0.025f;
        upgradeStatus.SCriDmg = upgradeArr[6] * 0.3f;
    }

    private void InitEquippedPixelmon()
    {
        for (int i = 0; i < 5; i++)
        {
            if (userData.equippedPxms[i].isEquipped)
            {
                Equipped(i, userData.equippedPxms[i]);
                SkillManager.Instance.ExecuteSkill(Player.Instance.pixelmons[i], i);
            }
        }
        //player.LocatedPixelmon();
    }

    private async void Equipped(int index, MyPixelmonData myData)
    {
        player.pixelmons[index] = equippedPixelmon[index];
        player.pixelmons[index].gameObject.SetActive(true);
        player.pixelmons[index].myData = myData;
        player.pixelmons[index].data = pxmData[myData.id];
        player.pixelmons[index].fsm.anim.runtimeAnimatorController = await ResourceManager.Instance.LoadAsset<RuntimeAnimatorController>(myData.rcode, eAddressableType.animator);
        player.pixelmons[index].InitPxm();
        player.LocatedPixelmon();
    }

    private void UnEquipped(int index)
    {
        if (player.pixelmons[index] == null) return; 
        player.pixelmons[index].fsm.InvokeAttack(false);
        player.pixelmons[index].gameObject.SetActive(false);
        player.pixelmons[index] = null;
        player.LocatedPixelmon();
    }

    public void UnLockedPixelmon(int index)
    {
        pxmTab.unLockAction?.Invoke(index);
    }

    public void UnLockedSlots(int index)
    {
        switch (index) 
        {
            case 10:
                UnLockedSlot(2);
                UIManager.Instance.ShowWarn("3번째 픽셀몬 슬롯이 개방되었습니다.");
                break;
            case 50:
                UnLockedSlot(3);
                UIManager.Instance.ShowWarn("4번째 픽셀몬 슬롯이 개방되었습니다.");
                break;
            case 100:
                UnLockedSlot(4);
                unlockSlotAction -= UnLockedSlots;
                UIManager.Instance.ShowWarn("5번째 픽셀몬 슬롯이 개방되었습니다.");
                break;
            default:
                break;
        }
    }

    private void UnLockedSlot(int index)
    {
        pxmTab.equipData[index].isLocked = false;
        pxmTab.equipData[index].stateIcon.sprite = plusIcon;
        userData.isLockedSlot[index] = false;
        saveManager.SetData(nameof(saveManager.userData.isLockedSlot), userData.isLockedSlot);
        layouts[0].UnLockedIcon(index);
        layouts[1].UnLockedIcon(index);
    }

    public PixelmonData FindPixelmonData(int id)
    {
        return DataManager.Instance.pixelmonData.data[id];
    }

    public void ApplyStatus(PixelmonData data, MyPixelmonData myData)
    {
        if (myData.isEquipped)
        {
            foreach (var equipData in pxmTab.equipData)
            {
                if (equipData.myPxmData != null && equipData.myPxmData.id == myData.id)
                {
                    equipData.SetPxmLv();
                    Player.Instance.pixelmons[equipData.slotIndex].status.InitStatus(data, myData);
                    break;
                }
            }
        }
    }
}
