using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : Singleton<StageManager>
{
    private SaveManager saveManager;
    private UserData userData;
    private QuestManager questManager;
    private GuideManager guideManager;

    #region Stage Info
    [Header("Stage Info")]
    public StageData data;
    private readonly string fixedRcode = "STG_";
    private string _currentRcode;
    public string CurrentRcode
    {
        get
        {
            if (_currentRcode != userData.stageRcode)
            {
                _currentRcode = userData.stageRcode;
            }
            return _currentRcode;
        }
        private set
        {
            _currentRcode = value;
            saveManager.SetFieldData(nameof(userData.stageRcode), _currentRcode);
            data = DataManager.Instance.GetData<StageData>(_currentRcode);
        }
    }

    [SerializeField] private readonly int themeKinds = 3;
    [SerializeField] private readonly int themeStgCount = 5;
    [SerializeField] private readonly int maxWorldNum = 10;
    public int diffNum;
    public int worldNum;
    public int stageNum;
    public int themeNum;

    public int curSpawnCount = 0;
    private float curInterval = 0; //현재 시간 간격
    private readonly float spawnInterval = 2f; //스폰 간격
    public int killCount = 0;

    private float bossLeftTime;
    private readonly float bossLimitTime = 30;

    public int dgIndex = 0;
    public DgMonster dgBoss;
    #endregion

    #region Coroutine & WaitUntil
    private Coroutine stageCoroutine;
    private Coroutine infiniteMode;
    private WaitUntil proceedNormalStg;
    private WaitUntil proceedBossStg;
    private WaitUntil proceedDgStg;
    private WaitUntil bossDieDone;
    #endregion

    #region Flags
    private bool isStgFade;
    private bool isBossStage;
    private bool isBossCleared;
    public bool isBossDieDone;
    public bool isDungeon;
    public bool isDungeonClear;
    private bool isPlayerDead;
    #endregion

    #region UI
    [Header("Stage UI")]
    [SerializeField] private Image StageIcon;
    [SerializeField] private Sprite[] iconSprite;
    [SerializeField] private TextMeshProUGUI stageTitleTxt;

    [SerializeField] private Slider progressSldr;
    [SerializeField] private TextMeshProUGUI progressTxt;
    private float prevProgress;
    private float curProgress;
    Coroutine progressCoroutine;

    [SerializeField] private Slider bossTimeSldr;
    [SerializeField] private TextMeshProUGUI bossTimeTxt;
    [SerializeField] private TextMeshProUGUI bossTxt;
    [SerializeField] private GameObject bossBtn;

    public Spawner spawner;
    public FadeInvoker stageFade;
    public FadeInvoker allFade;

    [SerializeField] private RectTransform canvasRect;
    GameObject middleBar;
    GameObject bottomBar;
    #endregion

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        saveManager = SaveManager.Instance;
        userData = saveManager.userData;
        guideManager = GuideManager.Instance;
        InitData();
    }

    private void InitData()
    {
        data = DataManager.Instance.GetData<StageData>(userData.stageRcode);

        themeNum = CurrentRcode[CurrentRcode.Length - 1] - 48;
        if (CurrentRcode[4] == 'B') isBossStage = true;
        else isBossStage = false;

        diffNum = int.Parse(userData.curStage.Substring(0, 2));
        worldNum = int.Parse(userData.curStage.Substring(2, 2));
        stageNum = int.Parse(userData.curStage.Substring(4, 2));
        ChangeMapByTheme();

        killCount = userData.curHuntCount;
    }

    private IEnumerator Start()
    {
        if (userData.tutoIndex < 4)
        {
            isStgFade = true;
            allFade.gameObject.SetActive(true);
            allFade.StartFadeOut();
        }
        while (userData.tutoIndex < 4) yield return null;

        GameManager.Instance.OnPlayerDie += OnPlayerDie;
        GameManager.Instance.OnEnemyDie += OnEnemyDie;

        proceedNormalStg = new WaitUntil(() => NormalStage());
        proceedBossStg = new WaitUntil(() => BossStage());
        proceedDgStg = new WaitUntil(() => DungeonStage());
        bossDieDone = new WaitUntil(() => isBossDieDone == true);

        while (middleBar == null)
        {
            var middleBarObj = UIManager.Get<UIMiddleBar>();
            if (middleBarObj != null)
            {
                middleBar = middleBarObj.gameObject;
            }
            yield return null;
        }
        bottomBar = UIManager.Instance.parents[2].GetChild(0).gameObject;
        questManager = QuestManager.Instance;
        InitStage();
    }

    #region Stage
    public void InitStage()
    {
        isPlayerDead = false;

        InitStageUI();
        GameManager.Instance.NotifyStageStart();

        if (!isStgFade)
        {
            isStgFade = true;
            allFade.gameObject.SetActive(true);
            allFade.StartFadeOut();
        }
        if (userData.isInfinite)
        {
            killCount = 0;
            infiniteMode = StartCoroutine(InfiniteStage());
        }
        else
        {
            killCount = userData.curHuntCount;
            stageCoroutine = StartCoroutine(StartStage());
        }
    }

    private IEnumerator StartStage()
    {
        if (!isStgFade)
        {
            isStgFade = true;
            allFade.gameObject.SetActive(true);
            yield return allFade.FadeOut();
        }
        else
        {
            stageFade.gameObject.SetActive(true);
            yield return stageFade.FadeOut();
        }

        if (isBossStage && !isDungeon)
        { 
            InitBossStage();
            yield return proceedBossStg;
            bossTimeSldr.gameObject.SetActive(false);

            if (isBossCleared)
            {
                isBossCleared = false;
                yield return bossDieDone;
                ResetSpawnedEnemy();

                if (questManager.IsMyTurn(QuestType.Stage))
                {
                    questManager.OnQuestEvent();
                }

                stageFade.gameObject.SetActive(true);
                yield return stageFade.FadeIn();
                NextStageData();  
            }
            else if (!isDungeon)
            {
                NextStageData(false);
                saveManager.SetFieldData(nameof(userData.isInfinite), true);
                yield break;
            }
        }
        else
        {
            yield return proceedNormalStg;

            if (!isDungeon)
            {
                stageFade.gameObject.SetActive(true);
                yield return stageFade.FadeIn();
                NextStageData();
            }
        }
        
        if (isDungeon)
        {
            allFade.gameObject.SetActive(true);
            yield return allFade.FadeIn();
            InitDgStage();
            yield return allFade.FadeOut();

            yield return proceedDgStg;

            allFade.gameObject.SetActive(true);
            if (isDungeonClear)
            {
                yield return new WaitForSeconds(3f);
            }
            yield return allFade.FadeIn();
            isStgFade = false;

            ChangeMapByTheme();

            middleBar.gameObject.SetActive(true);
            bottomBar.gameObject.SetActive(true);
            killCount = userData.curHuntCount;
        }

        InitStage();
    }

    private bool NormalStage() 
    {
        if (isDungeon)
        {
            ResetSpawnedEnemy();
            return true;
        }
        if (killCount >= data.nextStageCount)
        {
            ResetSpawnedEnemy();
            return true;
        }

        curInterval += Time.deltaTime;
        if (curInterval >= spawnInterval)
        {
            if (curSpawnCount < data.spawnCount)
            {
                spawner.SpawnMonsterTroop(data.monsterId, data.spawnCount);
                curInterval = 0;
            }
        }
        return false;
    }

    private IEnumerator InfiniteStage()
    {
        stageFade.gameObject.SetActive(true);
        yield return stageFade.FadeOut();

        while (true)
        {
            if (isDungeon)
            {
                ResetSpawnedEnemy();
                stageCoroutine = StartCoroutine(StartStage());
                yield break;
            }

            curInterval += Time.deltaTime;
            if (curInterval >= spawnInterval)
            {
                if (curSpawnCount < data.spawnCount)
                {
                    spawner.SpawnMonsterTroop(data.monsterId, data.spawnCount);
                    curInterval = 0;
                }
            }
            yield return null;
        }
    }

    private void InitBossStage()
    {
        if (userData.curHuntCount != 0)
        {
            saveManager.SetFieldData(nameof(userData.curHuntCount), 0);
            curSpawnCount = userData.curHuntCount;
        }
        spawner.SpawnMonsterTroop(data.monsterId, data.spawnCount);
        bossLeftTime = bossLimitTime;
    }

    private bool BossStage()
    {
        if (isDungeon)
        {
            ResetSpawnedEnemy();
            return true;
        }
        if (isBossCleared)
            return true;          

        StageTimer();

        if (bossLeftTime == 0)
        {
            ResetSpawnedEnemy();
            GameManager.Instance.NotifyStageTimeOut();
            return true;
        }
        return false;
    }
    
    private void InitDgStage()
    {
        bossLeftTime = bossLimitTime;
        dgBoss = spawner.GetDgMonster(dgIndex);
        dgBoss.InitDgMonster(dgIndex);
        MapManager.Instance.OnMapChanged(dgIndex);

        middleBar.gameObject.SetActive(false);
        bottomBar.gameObject.SetActive(false);
        InitStageUI();
    }

    private bool DungeonStage()
    {
        if (!isDungeon)
        {//중도포기 버튼.
            dgBoss.DisableDgMonster();
            return true;
        }

        stageTitleTxt.text = $"Dungeon Lv.{dgBoss.dgLv}";
        StageTimer();

        if (bossLeftTime == 0)
        {//TimeOver
            isDungeon = false;
            isDungeonClear = true;
            dgBoss.DisableDgMonster();
            if (dgIndex == 0 && guideManager.guideNum == guideManager.goldDg)
            {
                questManager.OnQuestEvent();
            }
            return true;
        }
        return false;
    }

    private void StageTimer()
    {
        bossLeftTime = Mathf.Max(0, bossLeftTime - Time.deltaTime);
        float percent = bossLeftTime / bossLimitTime;
        bossTimeSldr.value = percent;
        bossTimeTxt.text = string.Format("{0:F2}", bossLeftTime);
    }
    
    private void NextStageData(bool isClear = true)
    {
        string newRcode = fixedRcode;
        string newStageProgress;

        if (!isClear)
        {//실패: CurStage 변화X.
            newRcode += "N" + themeNum;
            isBossStage = false;
            CurrentRcode = newRcode;
            return;
        }
        else if (!isBossStage)
        {//Normal -> Boss: CurStage 변화X.
            newRcode += "B" + themeNum;
            isBossStage = true;
            CurrentRcode = newRcode;
            return;
        }
        
        if (stageNum == themeKinds * themeStgCount) //Next World
        {
            if (worldNum == maxWorldNum) //Next Diff
            {
                diffNum++;
                worldNum = 0;
            }
            worldNum++;
            stageNum = 0;
            themeNum = 1;
            MapManager.Instance.OnMapChanged((int)MapList.Theme1);
        }
        else if (stageNum % 5 == 0) //Next Theme
        {
            themeNum++;
            if (themeNum == 2)
            {
                MapManager.Instance.OnMapChanged((int)MapList.Theme2);
            }
            else
            {
                MapManager.Instance.OnMapChanged((int)MapList.Theme3);
            }
        }
        stageNum++;
        isBossStage = false;

        newRcode += "N";
        newRcode += themeNum;
        CurrentRcode = newRcode;
        newStageProgress = diffNum.ToString("D2") + worldNum.ToString("D2") + (stageNum).ToString("D2");

        saveManager.SetFieldData(nameof(userData.curStage), newStageProgress);
    }

    private void ResetSpawnedEnemy()
    {
        spawner.ResetSpawnedMonster();
        curSpawnCount = 0;
        killCount = 0;
        saveManager.SetFieldData(nameof(userData.curHuntCount), 0);
    }
    #endregion

    #region Death Events
    public void OnEnemyDie(Enemy enemy)
    {
        EnemyData enemyData = enemy.statHandler.data;
        if (enemyData.isBoss)
        {
            isBossCleared = true;
        }
        else
        {
            killCount++;
            curSpawnCount--;
            curProgress = Mathf.Min((float)killCount / data.nextStageCount, 1f);       
        }

        RewardManager.Instance.SpawnRewards(enemy.gameObject, enemyData.rewardType, enemyData.rewardValue, enemyData.rewardRate);
        
        if (questManager.IsMyTurn(QuestType.Mob) && !enemy.statHandler.data.isBoss)
        {
            questManager.OnQuestEvent();
        }
        else if (questManager.IsMyTurn(QuestType.Boss) && enemy.statHandler.data.isBoss)
        {
            questManager.OnQuestEvent();
        }
        saveManager.SetFieldData(nameof(userData.curHuntCount), 1, true);
    }

    public void OnPlayerDie()
    {
        if (isPlayerDead) return;
        isPlayerDead = true;
        
        if (stageCoroutine != null)
        {
            StopCoroutine(stageCoroutine); //Stop Stage
        }
        
        ResetSpawnedEnemy();

        if (isBossStage)
        {
            saveManager.SetFieldData(nameof(userData.isInfinite), true);
        }
        NextStageData(false);

        if (isDungeon)
        {
            Destroy(dgBoss.gameObject);
            isDungeon = false;
        }
    }
    #endregion

    #region UI
    private void InitStageUI()
    {
        Player.Instance.transform.position = Vector3.zero;
        if (progressCoroutine != null)
        {
            StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }

        bossTxt.gameObject.SetActive(false);

        if (isDungeon)
        {
            Player.Instance.transform.position += Vector3.down * 3;
            bossTimeSldr.gameObject.SetActive(true);
            bossBtn.SetActive(false);
            stageTitleTxt.text = $"Dungeon Lv.{dgBoss.dgLv}";
            StageIcon.gameObject.SetActive(false);
        }
        else if (userData.isInfinite)
        {
            bossTimeSldr.gameObject.SetActive(false);
            bossBtn.SetActive(true);
            stageTitleTxt.text = $"{SetDiffTxt(diffNum)} {worldNum}-{stageNum}";
            StageIcon.gameObject.SetActive(true);
            StageIcon.sprite = iconSprite[0];
            progressSldr.value = 1;
            progressTxt.text = "100%";
        }
        else if (isBossStage)
        {
            bossTimeSldr.gameObject.SetActive(true);
            bossTxt.gameObject.SetActive(true);
            bossBtn.SetActive(false);
            stageTitleTxt.text = $"{SetDiffTxt(diffNum)} {worldNum}-{stageNum}";
            StageIcon.gameObject.SetActive(true);
            StageIcon.sprite = iconSprite[1];
            progressSldr.value = 1;
            progressTxt.text = "100%";
        }
        else
        {
            bossTimeSldr.gameObject.SetActive(false);
            bossBtn.SetActive(false);
            stageTitleTxt.text = $"{SetDiffTxt(diffNum)} {worldNum}-{stageNum}";
            StageIcon.gameObject.SetActive(true);
            StageIcon.sprite = iconSprite[0];
            curProgress = Mathf.Min((float)killCount / data.nextStageCount, 1f);
            prevProgress = curProgress;
            progressSldr.value = prevProgress;
            progressTxt.text = string.Format($"{(int)(prevProgress * 100)}%");
            progressCoroutine = StartCoroutine(SetProgressBar());
        }
    }

    private void ChangeMapByTheme()
    {
        switch (themeNum)
        {
            case 2:
                MapManager.Instance.OnMapChanged((int)MapList.Theme2);
                break;
            case 3:
                MapManager.Instance.OnMapChanged((int)MapList.Theme3);
                break;
            default:
                MapManager.Instance.OnMapChanged((int)MapList.Theme1);
                break;
        }
    }

    private IEnumerator SetProgressBar()
    {
        while (true)
        {
            if (curProgress > prevProgress)
            {
                StartCoroutine(UIUtils.AnimateSliderChange(progressSldr, prevProgress, curProgress));
                progressTxt.text = string.Format($"{(int)(prevProgress * 100)}%");
                prevProgress = curProgress;
            }
            yield return null;
        }
    }

    public string SetDiffTxt(int num)
    {
        return num switch
        {
            0 => "쉬움",
            1 => "보통",
            2 => "어려움",
            3 => "매우어려움",
            4 => "악몽I",
            5 => "악몽II",
            6 => "악몽III",
            7 => "심연I",
            8 => "심연II",
            9 => "심연III",
            10 => "지옥I",
            11 => "지옥II",
            12 => "지옥III",
            13 => "지옥IV",
            14 => "지옥V",
            15 => "불지옥I",
            16 => "불지옥II",
            17 => "불지옥III",
            18 => "불지옥IV",
            19 => "불지옥V",
            _ => "공허"
        };
    }

    public Slider GetBossSlider()
    {
        return progressSldr;
    }

    public TextMeshProUGUI GetBossHpText()
    {
        return progressTxt;
    }

    public void OnBossBtn()
    {
        if (infiniteMode != null)
        {
            StopCoroutine(infiniteMode);
        }
        saveManager.SetFieldData(nameof(userData.isInfinite), false);

        ResetSpawnedEnemy();
        NextStageData();
        StartCoroutine(delayBossBtn());
    }

    private IEnumerator delayBossBtn()
    {
        stageFade.gameObject.SetActive(true);
        yield return stageFade.FadeIn();
        InitStage();
    }
    #endregion
}