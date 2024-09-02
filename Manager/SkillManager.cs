using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SkillManager : Singleton<SkillManager>
{
    public SkillTab skillTab;

    public Transform skillStorage;
    public Dictionary<int, List<BaseSkill>> dicSkill = new Dictionary<int, List<BaseSkill>>();
    public List<BaseSkill> prefablst = new List<BaseSkill>();

    public List<UnityAction<Pixelmon, ActiveData, MyAtvData>> actionStorage = new List<UnityAction<Pixelmon, ActiveData, MyAtvData>>();
    private UnityAction<Pixelmon, ActiveData, MyAtvData> skillAction;
    public PixelmonLayout layout;
    public float[] skillCoolTime;
    public Pixelmon[] reStartInfo;
    public Coroutine[] skillCoroutine;
    public DataManager dataManager;
    public SaveManager saveManager;
    protected override void Awake()
    {
        base.Awake();
        dataManager = DataManager.Instance;
        saveManager = SaveManager.Instance;
        skillCoolTime = new float[dataManager.activeData.data.Count];
        skillCoroutine = new Coroutine[dataManager.activeData.data.Count];
    }

    // Start is called before the first frame update
    void Start()
    {
        skillAction += OnSkillAction;
        InitStorage();
    }

    public void InitStorage()
    {
        for(int i = 0; i < dataManager.activeData.data.Count; i++)
        {
            actionStorage.Add(skillAction);
            if (prefablst.Count > i && prefablst[i] != null)
                SpawnSkill(i);
        }
        
    }

    public void ExecuteSkill(Pixelmon pxm, int index)
    {
        if (pxm.myData.atvSkillId != -1 && skillCoroutine[pxm.myData.atvSkillId] == null)
        {
            skillCoroutine[pxm.myData.atvSkillId] = StartCoroutine(SkillAction(pxm, index, pxm.myData.atvSkillId));
        }
    }

    public IEnumerator SkillAction(Pixelmon pxm, int index, int id)
    {
        bool isExit = false;
        yield return new WaitUntil(() => pxm.fsm.target != null);
        ActiveData data = skillTab.allData[pxm.myData.atvSkillId].atvData;
        MyAtvData myData = skillTab.allData[pxm.myData.atvSkillId].myAtvData;
        while (myData.isEquipped && Player.Instance.pixelmons[index] != null)
        {
            if (skillCoolTime[id] == 0)
            {
                Debug.Log(id);
                yield return new WaitUntil(
                    () => pxm.fsm.target != null &&
                    pxm.fsm.currentState == pxm.fsm.AttackState ||
                    !myData.isEquipped);
                if (!myData.isEquipped || Player.Instance.pixelmons[index] == null)
                {
                    layout.timer[index].fillAmount = 0;
                    skillCoroutine[pxm.myData.atvSkillId] = null;
                    break;
                }
                actionStorage[data.id]?.Invoke(pxm, data, myData);
                data.isCT = true;
                skillCoolTime[id] = data.coolTime;
            }
            while (0 <= skillCoolTime[id])
            {
                skillCoolTime[id] -= Time.deltaTime;  
                layout.timer[index].fillAmount = skillCoolTime[id] / data.coolTime;
                if (Player.Instance.pixelmons[index] != pxm || Player.Instance.pixelmons[index] != null && Player.Instance.pixelmons[index].myData.atvSkillId != id)
                {
                    layout.timer[index].fillAmount = 0;
                    skillCoroutine[id] = null;
                    isExit = true;
                }
                yield return null;
            }
            skillCoolTime[id] = 0;
            data.isCT = false;
            if (isExit)
                break;
        }
        data.isCT = false;
        skillCoroutine[id] = null;
    }

    public void OnSkillAction(Pixelmon pxm, ActiveData atvData, MyAtvData myAtvData)
    {
        var targets = pxm.fsm.Search(1);
        if(targets.Count == 0 || pxm.myData.atvSkillId == -1) return;
        for (int i = 0; i < 1; i++)
        {
            //프리팹 생성
            var skill = dicSkill[pxm.myData.atvSkillId][i];
            skill.gameObject.SetActive(true);
            //조건 설정
            skill.InitInfo(pxm, targets[i].gameObject, myAtvData);
        }
    }

    public void UnEquipSkill(int slotIndex)
    {
        skillTab.equipData[slotIndex].UnEquipAction();
    }

    public void AddSkill(int id)
    {
        skillTab.AddSkillAction?.Invoke(id);
    }

    public void SpawnSkill(int id)
    {
        if (!dicSkill.ContainsKey(id))
        {
            dicSkill.Add(id , new List<BaseSkill>());
            for (int i = 0; i < dataManager.activeData.data[id].count; i++)
            {
                var skill = Instantiate(prefablst[id], skillStorage);
                skill.data = dataManager.activeData.data[id];
                skill.SetSound();
                dicSkill[id].Add(skill);
                skill.gameObject.SetActive(false);
            }
        }
    }
}
