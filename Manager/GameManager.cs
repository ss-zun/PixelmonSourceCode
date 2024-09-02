using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject DataLoadWarningPopup;
    [SerializeField] private GameObject exitPanel;
    public static bool isInit;

    public event Action<Enemy> OnEnemyDie;
    public event Action OnPlayerDie;
    public event Action OnStageTimeOut;
    public event Action OnStageStart;


    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 30;
    }

    public void OnInit()
    {
        StartCoroutine(OnManagerInit());
        StartCoroutine(OnBackButtonPressed());
    }

    public IEnumerator OnManagerInit()
    {
        UILoading.Show();

        StartDataLoading();
        yield return new WaitUntil(() => DataManager.Instance.isInit);

        ResourceManager.Instance.Init();
        yield return new WaitUntil(() => ResourceManager.Instance.isInit);

        InitData();
        yield return new WaitUntil(() => DataManager.Instance.isPxmInit);

        UILoading.Instance.HideProgress();
    }

    private async void StartDataLoading()
    {
        bool result = await DataManager.Instance.Init();

        if (!result) // 데이터 로드 실패
        {
            DataLoadWarningPopup.SetActive(true);
        }
    }

    private IEnumerator OnBackButtonPressed()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
            Task showPanelTask = UIManager.Show<GameExitPanel>();
            while (!showPanelTask.IsCompleted) yield return null;
        }
    }

    public void OnClickExitBtn()
    {
#if UNITY_EDITOR
        // 에디터 모드에서 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // 빌드된 애플리케이션에서는 애플리케이션 종료
    Application.Quit();
#endif
    }

    public async void InitData()
    {
        await DataManager.Instance.SetBaseData();
    }

    public void NotifyEnemyDie(Enemy enemy)
    {
        OnEnemyDie?.Invoke(enemy);
    }

    public void NotifyPlayerDie()
    {
        OnPlayerDie?.Invoke();
    }

    public void NotifyStageTimeOut()
    {
        OnStageTimeOut?.Invoke();
    }

    public void NotifyStageStart()
    {
        OnStageStart?.Invoke();
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        OnPlayerDie = null;
        OnStageTimeOut = null;
        OnStageStart = null;
    }
}
