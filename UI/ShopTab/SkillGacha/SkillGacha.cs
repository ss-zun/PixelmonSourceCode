using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillGacha : MonoBehaviour
{
    [SerializeField] private SkillGachaBtn FiveBtn;
    [SerializeField] private SkillGachaBtn FifteenBtn;
    [SerializeField] private SkillGachaBtn ThirtyBtn;
    [SerializeField] private TextMeshProUGUI TicketCostTxt;

    private UserData userData => SaveManager.Instance.userData;
    private UISkillGachaPopup skillGachaPopup;
    private MyAtvData randSkill;
    private int oneCostTicket = 1; // 1티켓 == 1회 뽑기
    private int oneCostDia = 200;  // 200다이아 == 1회 뽑기

    private Dictionary<string, List<ActiveData>> RankDatas = new Dictionary<string, List<ActiveData>>();
    private Dictionary<string, float> rankProb = new Dictionary<string, float>
    {
        { "SS", 0.5f },
        { "S", 1.2f },
        { "A", 8.3f },
        { "B", 25.5f },
        { "C", 64.5f }
    };

    public float slotDurationTime = 0.5f;
    private WaitForSeconds waitSlotDuration;
    private bool isDoneGacha;

    private async void Awake()
    {
        skillGachaPopup = await UIManager.Show<UISkillGachaPopup>();
    }

    private void Start()
    {
        UIManager.Instance.UpdateUI += UpdateCostUI;

        waitSlotDuration = new WaitForSeconds(slotDurationTime);

        var datas = DataManager.Instance.activeData.data;
        for (int i = 0; i < datas.Count; i++)
        {
            if (RankDatas.ContainsKey(datas[i].rank))
                RankDatas[datas[i].rank].Add(datas[i]);
            else
                RankDatas.Add(datas[i].rank, new List<ActiveData> { datas[i] });
        }
    }

    public void SetSkillGacha()
    {
        SetBtnInteractable();
        TicketCostTxt.text = userData.skillTicket.ToString();
        isDoneGacha = true;
    }

    #region UI
    private void SetBtnInteractable()
    {
        UpdateButnInteractable(FiveBtn, 5);
        UpdateButnInteractable(FifteenBtn, 15);
        UpdateButnInteractable(ThirtyBtn, 30);
    }

    public void UpdateButnInteractable(SkillGachaBtn skillGachaBtn, int requiredAmount)
    {
        skillGachaBtn.Btn.interactable = true;
        if (userData.skillTicket >= requiredAmount * oneCostTicket)
        {
            skillGachaBtn.SetTicket();
        }
        else if (userData.diamond >= requiredAmount * oneCostDia)
        {
            skillGachaBtn.SetDia();
        }          
        else skillGachaBtn.Btn.interactable = false;
    }

    private void UpdateCostUI(DirtyUI dirtyUI)
    {
        if (dirtyUI == DirtyUI.Diamond || dirtyUI == DirtyUI.SkillTicket)
        {
            SetBtnInteractable();
            TicketCostTxt.text = userData.skillTicket.ToString();
        }            
    }
    #endregion

    public void OnClickBtn(int multiplier)
    {
        if (!isDoneGacha) return;
        int totalCostTicket = oneCostTicket * multiplier;
        int totalCostDia = oneCostDia * multiplier;

        if (userData.skillTicket >= totalCostTicket) // 티켓
        {
            SaveManager.Instance.SetFieldData(nameof(userData.skillTicket), -totalCostTicket, true);
        }
        else // 다이아
        {
            SaveManager.Instance.SetFieldData(nameof(userData.diamond), -totalCostDia, true);
        }

        if (QuestManager.Instance.IsMyTurn(QuestType.Skill))
        {
            QuestManager.Instance.OnQuestEvent(multiplier);
        }

        StartCoroutine(Gacha(multiplier));       
    }

    private string RandRank()
    {
        int randProb = UnityEngine.Random.Range(100, 10001);

        float cumProb = 0;
        foreach (var prob in rankProb)
        {
            cumProb += prob.Value * 100;
            if (randProb <= cumProb)
            {
                return prob.Key;
            }
        }

        return null;
    }
    private IEnumerator Gacha(int count)
    {
        isDoneGacha = false;
        ActiveData[] resultDatas = new ActiveData[count];       
        var ownedSkills = userData.ownedSkills;

        for (int i = 0; i < count; i++)
        {
            randSkill = new MyAtvData();
            resultDatas[i] = new ActiveData();

            var rankList = RankDatas[RandRank()];
            int randIndex = UnityEngine.Random.Range(0, rankList.Count);
            resultDatas[i] = rankList[randIndex];

            if (SkillManager.Instance.skillTab.allData[resultDatas[i].id].myAtvData.isOwned) // 이미 있는 스킬
            {
                int index = SkillManager.Instance.skillTab.allData[resultDatas[i].id].atvData.dataIndex;
                if(index != -1)
                    SaveManager.Instance.UpdateSkillData(index, "evolvedCount", ownedSkills[index].evolvedCount + 1);
            }
            else // 새롭게 뽑은 스킬
            {
                randSkill.rcode = resultDatas[i].rcode;
                randSkill.id = resultDatas[i].id;
                randSkill.isOwned = true;
                ownedSkills.Add(randSkill);
                SaveManager.Instance.SetFieldData(nameof(userData.ownedSkills), ownedSkills);
            }
            SkillManager.Instance.AddSkill(resultDatas[i].id);
        }

        skillGachaPopup.SetActive(true);
        bool isHigh = skillGachaPopup.SetPopup(count, resultDatas, this);
        if (isHigh) AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20000").clip);
        yield return waitSlotDuration;

        isDoneGacha = true;
    }
}
