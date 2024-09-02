using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillEquipSlot : MonoBehaviour
{
    private SaveManager saveManager => SaveManager.Instance;
    public SkillTab skillTab;
    public ActiveData atvData;
    public MyAtvData myAtvData;
    public MyPixelmonData myPxmData;
    public Image equipIcon;
    public int slotIndex;

    private void Start()
    {
        skillTab = SkillManager.Instance.skillTab;
        ChangedInfo();
    }


    public void ChangedInfo()
    {
        slotIndex = gameObject.transform.GetSiblingIndex();
        MyAtvData myData = skillTab.equipData[slotIndex].myAtvData;
        if (myData != null && myData.isEquipped)
        {
            equipIcon.sprite = skillTab.allData[myData.id].atvData.icon;
            myAtvData = myData;
        }
    }

    public void OnClick()
    {
        if (skillTab.tabState == TabState.Normal && myAtvData == null)
        {
            UIManager.Instance.ShowWarn("장착된 스킬이 없습니다.");
            return;
        }
        else if (skillTab.tabState == TabState.Normal && myAtvData.isEquipped)
        {
            skillTab.OnSkillPopUp(myAtvData.id);
        }
        else if (skillTab.tabState == TabState.Equip && SaveManager.Instance.userData.equippedPxms[slotIndex].isEquipped)
        {
            if (DataManager.Instance.activeData.data[skillTab.choiceId].isCT)
            {
                //스킬이 쿨타임 안내
                UIManager.Instance.ShowWarn("해당 스킬이 쿨타임 중입니다.");
                skillTab.OnCancelEquip();
                return;
            }

            if (myAtvData != null && myAtvData.isEquipped)
                skillTab.CheckedOverlap(myAtvData.id);
            skillTab.allData[skillTab.choiceId].EquipAction();
            EquipAction(skillTab.allData[skillTab.choiceId].atvData, skillTab.allData[skillTab.choiceId].myAtvData, slotIndex);
            skillTab.OnCancelEquip();
        }
        else
        {
            skillTab.OnCancelEquip();
        }
    }


    public void EquipAction(ActiveData data, MyAtvData myData, int idx)
    {
        atvData = data;
        myAtvData = myData;
        equipIcon.sprite = atvData.icon;
        
        saveManager.userData.equippedSkills[idx] = atvData.id;
        saveManager.SetData("equippedSkills", saveManager.userData.equippedSkills);

        myData.isEquipped = true;
        myData.isAttached = true;
        myData.pxmId = saveManager.userData.equippedPxms[idx].id;
        saveManager.UpdateSkillData(atvData.dataIndex, nameof(myAtvData.isEquipped), true);
        saveManager.UpdateSkillData(atvData.dataIndex, nameof(myAtvData.isAttached), true);
        saveManager.UpdateSkillData(atvData.dataIndex, nameof(myAtvData.pxmId), myData.pxmId);

        PixelmonManager.Instance.pxmTab.equipData[idx].myPxmData.atvSkillId = atvData.id;
        saveManager.userData.equippedPxms[idx].atvSkillId = atvData.id;
        saveManager.SetData("equippedPxms", saveManager.userData.equippedPxms);

        saveManager.UpdatePixelmonData(saveManager.userData.equippedPxms[idx].id, nameof(myPxmData.atvSkillId), atvData.id);
        SkillManager.Instance.ExecuteSkill(Player.Instance.pixelmons[idx], idx);

        if (GuideManager.Instance.guideNum == GuideManager.Instance.skillEquip)
        {
            QuestManager.Instance.OnQuestEvent();
        }
    }

    public void UnEquipAction()
    {
        skillTab.UnAttachedAction(myAtvData.pxmId, myAtvData.id);
        SetEquipSlot();
    }

    public void SetEquipSlot()
    {
        equipIcon.sprite = null;
        atvData = null;
        myAtvData = null;
        saveManager.userData.equippedSkills[slotIndex] = -1;
        saveManager.SetData(nameof(saveManager.userData.equippedSkills), saveManager.userData.equippedSkills);
    }

    public void OnPlaySound()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20012").clip);
    }
}
