using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum QuestType
{
    Default,
    UserLv,
    Mob,
    Boss,
    Stage,
    Egg,
    Nest,
    UpgradeAtk,
    UpgradeDmg,
    Skill,
    Feed,
    Seed,
    Harvest,
    Null
}

public class QuestManager : Singleton<QuestManager>
{
    public event Action<int> QuestEvent;

    private StageManager stageManager;
    private GuideManager guideManager;
    private SaveManager saveManager;
    private UserData userData;

    public QuestData data;
    private int questNum;
    private string curIndex;
    private int repeatCount;
    private readonly string maxMainQNum = "Q49";
    private readonly int maxRepeatNum = 3;

    private QuestType curType;
    private int curGoal;
    public int curProgress;
    public int curRwd;

    #region UI
    [SerializeField] private TextMeshProUGUI questNameTxt;
    [SerializeField] private TextMeshProUGUI countTxt;
    [SerializeField] public GameObject questClear;
    [SerializeField] private TextMeshProUGUI rewardTxt;
    [SerializeField] private Image rwdIcon;
    [SerializeField] private Sprite[] rwdSprite;
    #endregion

    public bool isSet = false;

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        stageManager = StageManager.Instance;
        guideManager = GuideManager.Instance;
        saveManager = SaveManager.Instance;
        userData = SaveManager.Instance.userData;

        GetQuestIndex();
        guideManager.SetBottomLock();
    }

    private void Start()
    {
        QuestEvent += UpdateProgress;
        SetQuestUI();
        Firebase.Analytics.FirebaseAnalytics.LogEvent($"Start_QuestIndex_{questNum}");
    }
    
    #region UI
    public void SetQuestUI()
    {
        SetQuestNameTxt();
        SetQuestCountTxt();
        SetRwdUI();
    }

    private void SetQuestNameTxt()
    {
        string curDescription = data.description;
        curDescription = curDescription.Replace("N", curGoal.ToString());
        questNameTxt.text = $"{questNum}. " + curDescription;
    }

    public void SetQuestCountTxt()
    {
        int goal = curGoal;
        int progress = Mathf.Min(curProgress, goal);
        
        if (progress < goal)
        {
            countTxt.text = $"<color=#FF2525>({progress} / {goal})</color>";
        }
        else
        {
            countTxt.text = $"<color=#82FF55>({progress} / {goal})</color>";
            questClear.SetActive(true);
        }
    }

    private void SetRwdUI()
    {
        rwdIcon.gameObject.SetActive(true);
        switch (data.rewardType)
        {
            case "RWD_Gold":
                rwdIcon.sprite = rwdSprite[0];
                break;
            case "RWD_Dia":
                rwdIcon.sprite = rwdSprite[1];
                break;
            case "RWD_Egg":
                rwdIcon.sprite = rwdSprite[2];
                break;
            case "RWD_Seed":
                rwdIcon.sprite = rwdSprite[3];
                break;
            case "RWD_Food":
                rwdIcon.sprite = rwdSprite[4];
                break;
            case "RWD_KeyA":
                rwdIcon.sprite = rwdSprite[5];
                break;
            case "RWD_Skill":
                rwdIcon.sprite = rwdSprite[6];
                break;
            default:
                rwdIcon.gameObject.SetActive(false);
                rewardTxt.text = "";
                curRwd = 0;
                break;
        }

        if (data.rewardType != "")
        {
            if (data.rewardType == "RWD_Gold" && data.rcode[0] == 'R')
            {
                curRwd = data.rewardValue + ((questNum - 50) / 3 * 5000);
                rewardTxt.text = Calculater.NumFormatter(curRwd);
            }
            else
            {
                curRwd = data.rewardValue;
                rewardTxt.text = curRwd.ToString();
            }
        }
    }

    public void QuestClearBtn()
    {
        if (IsQuestClear())
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"QuestIndex_{questNum}");
            if (data.rewardType != "")
            {
                RewardManager.Instance.SpawnRewards(data.rewardType, curRwd);
            }
            questClear.SetActive(false);
            SetQuestIndex();
            ResetProgress();
            SetQuestUI();
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20009").clip);
        }
        else
        {
            UIManager.Instance.ShowWarn("퀘스트 조건이 충족되지 않았습니다!!");
        }
    }
    #endregion
    
    private void GetQuestIndex()
    {
        string[] splitId = userData.questIndex.Split('_');
        repeatCount = int.Parse(splitId[0]);
        curIndex = splitId[1];
        if (splitId[1][0] == 'R')
        {
            questNum = (repeatCount - 1) * maxRepeatNum + int.Parse(splitId[1][1..]) + 56;
        }
        else
        {
            questNum = int.Parse(curIndex[1..]);
            guideManager.guideNum = questNum;

            if (userData.tutoIndex < 3)
            {
                guideManager.guideNum = 0;
            }
            guideManager.GuideNumTrigger(guideManager.guideNum);
        }

        data = DataManager.Instance.GetData<QuestData>(curIndex);
        curType = data.type;
        curGoal = data.goal;
        curProgress = userData.questProgress;
    }

    public void SetQuestIndex()
    {
        string qNum;

        questNum++;
        if (repeatCount == 0)
        {
            if (curIndex == maxMainQNum)
            {
                repeatCount++;
                qNum = "R1";
            }
            else
            {
                int index = int.Parse(curIndex[1..]);
                qNum = "Q" + (++index).ToString();
                guideManager.guideNum = index;
                isSet = true;
                guideManager.GuideNumTrigger(index);
                guideManager.SetBottomLock();
            }
        }
        else if (int.Parse(curIndex[1..]) == maxRepeatNum)
        {
            repeatCount++;
            qNum = "R1";
        }
        else
        {//반복Q 진행중
            int index = int.Parse(curIndex[1..]);
            qNum = "R" + (index + 1).ToString();
        }
        curIndex = qNum;

        data = DataManager.Instance.GetData<QuestData>(qNum);
        string newId = repeatCount.ToString() + "_" + qNum;
        saveManager.SetData(nameof(userData.questIndex), newId);

        curType = data.type;
        curGoal = data.goal;
    }


    public bool IsMyTurn(QuestType type)
    {
        return type == curType;
    }

    #region Quest Progress
    public void ResetProgress()
    {
        int progress;

        switch (curType)
        {
            case QuestType.UserLv:
                progress = userData.userLv;
                break;
            case QuestType.Nest:
                progress = userData.eggLv;
                break;
            case QuestType.UpgradeAtk:
                progress = userData.UpgradeLvs[0];
                break;
            case QuestType.UpgradeDmg:
                progress = userData.UpgradeLvs[1];
                break;
            default:
                progress = 0;
                break;
        }
        saveManager.SetData(nameof(userData.questProgress), progress);
        curProgress = progress;
        SetQuestCountTxt();
    }

    private void UpdateProgress(int value)
    {
        int progress = curProgress;
        switch (curType)
        {
            case QuestType.Default:
                progress = 1;
                break;
            case QuestType.UserLv:
                progress = userData.userLv;
                break;
            case QuestType.Nest:
                progress = userData.eggLv;
                break;
            case QuestType.UpgradeAtk:
                progress = userData.UpgradeLvs[0];
                break;
            case QuestType.UpgradeDmg:
                progress = userData.UpgradeLvs[1];
                break;
            default:
                progress++;
                break;
        }
        saveManager.SetData(nameof(userData.questProgress), progress + value);
        curProgress = progress + value;
        SetQuestCountTxt();
    }

    private bool IsQuestClear()
    {
        return curProgress >= curGoal;
    }
    #endregion

    public void OnQuestEvent(int value = 0)
    {
        QuestEvent?.Invoke(value);
    }
}