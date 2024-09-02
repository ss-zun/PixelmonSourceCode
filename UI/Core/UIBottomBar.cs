using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum eTabs
{
    UIPixelmonTab,
    UIUpgradeTab,
    UISkilltab,
    UIFarmTab,
    UIDungeonTab,
    UIShopTab
}

public class UIBottomBar : MonoBehaviour
{
    GuideManager guideManager;

    private List<UIBase> uiTabs = new List<UIBase>();
    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private List<Toggle> toggles;

    private bool isGuideOn = false;
    private int guidingToggle = -1;

    int selectedIndex { get => toggles.FindIndex(obj  => obj.isOn); }

    private async void Awake()
    {
        guideManager = GuideManager.Instance;
        guideManager.OnGuideAction += SetGuideArrow;

        var names = Enum.GetNames(typeof(eTabs));

        for (int i = 0; i < names.Length; i++)
        {
            uiTabs.Add(await UIManager.Show(names[i]));
        }
    }

    public void OnvalueChanged()
    {
        GameObject nextObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        
        if (int.TryParse(nextObject.name, out int index))
        {
            if (guideManager.Locks[index].activeInHierarchy)
            {
                string msg = "";
                switch (index)
                {
                    case 0:
                        return;
                    case 1:
                        msg = "해금조건: 퀘스트11 클리어";
                        UIManager.Instance.ShowWarn(msg);
                        return;
                    case 2:
                        msg = "해금조건: 퀘스트29 클리어";
                        UIManager.Instance.ShowWarn(msg);
                        return;
                    case 3:
                        msg = "해금조건: 퀘스트40 클리어";
                        UIManager.Instance.ShowWarn(msg);
                        return;
                    case 4:
                        msg = "해금조건: 퀘스트49 클리어";
                        UIManager.Instance.ShowWarn(msg);
                        return;
                    case 5:
                        msg = "해금조건: 퀘스트29 클리어";
                        UIManager.Instance.ShowWarn(msg);
                        return;
                    default:
                        break;
                }
            }
        }
        
        overlayPanel.SetActive(selectedIndex >= 0);
        uiTabs.ForEach(obj => obj.gameObject.SetActive(false)); //모든 탭 끄기.
        if (selectedIndex >= 0)
        {
            uiTabs[selectedIndex].gameObject.SetActive(true);   
            if (isGuideOn && guidingToggle == selectedIndex)
            {
                isGuideOn = false;
                guidingToggle = -1;

                switch(selectedIndex)
                {
                    case 0:
                        UIPixelmonTab tab = uiTabs[selectedIndex].GetComponent<UIPixelmonTab>();
                        tab.InvokePixelmonTabGuide();
                        break;
                    default:
                        break;
                }
            }
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20006").clip);
        }
    }

    private async void SetGuideArrow(int guideIndex)
    {
        if (guideIndex == guideManager.equipPixelmon)
        {
            isGuideOn = true;
            guideManager.GuideArrow.SetActive(true);
            guideManager.SetArrow(toggles[0].gameObject);
            guidingToggle = 0;
        }
        else if (guideIndex == guideManager.setAllPixelmon)
        {
            isGuideOn = true;
            guideManager.GuideArrow.SetActive(true);
            guideManager.SetArrow(toggles[0].gameObject);
            guidingToggle = 0;
        }
        if (QuestManager.Instance.isSet)
        {
            if (guideIndex == guideManager.upgrAtk)
            {
                await UIManager.Show<UIUnlockPopup>("성장탭 해금!");
                //isGuideOn = true;
                //guideManager.GuideArrow.SetActive(true);
                //guideManager.SetArrow(toggles[1].gameObject);
                //guidingToggle = 1;
            }
            else if (guideIndex == guideManager.skillGatcha)
            {
                await UIManager.Show<UIUnlockPopup>("스킬,상점탭 해금!");
                //isGuideOn = true;
                //guideManager.GuideArrow.SetActive(true);
                //guideManager.SetArrow(toggles[5].gameObject);
                //guidingToggle = 5;
            }
            else if (guideIndex == guideManager.seedFarm)
            {
                await UIManager.Show<UIUnlockPopup>("농장탭 해금!");
                //isGuideOn = true;
                //guideManager.GuideArrow.SetActive(true);
                //guideManager.SetArrow(toggles[3].gameObject);
                //guidingToggle = 3;
            }
            else if (guideIndex == guideManager.goldDg)
            {
                guideIndex = 100;
                await UIManager.Show<UIUnlockPopup>("던전탭 해금!");
                //isGuideOn = true;
                //guideManager.GuideArrow.SetActive(true);
                //guideManager.SetArrow(toggles[4].gameObject);
                //guidingToggle = 4;
            }
        }
    }
}

