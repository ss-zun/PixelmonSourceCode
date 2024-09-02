using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DungeonTab : UIBase
{
    SaveManager saveManager;
    UserData userData;

    public int[] dgLv => userData.bestDgLvs;
    public int[] keys = new int[3];

    #region UI
    [SerializeField] private TextMeshProUGUI keyChargeTime;
    [SerializeField] private DungeonSlot[] dungeonSlots;
    public UIDungeonEnterPopup dgPopup;
    
    #endregion

    private bool isInit = false;
    private Coroutine chargeTimeCoroutine;

    protected override async void Awake()
    {
        saveManager = SaveManager.Instance;
        userData = saveManager.userData;

        dgPopup = await UIManager.Show<UIDungeonEnterPopup>();
        dgPopup.dungeonTab = this;

        keys = new int[3];
        isInit = true;
    }

    private void OnEnable()
    {
        if (isInit)
        {
            keys[0] = userData.key0;
            keys[1] = userData.key1;
            keys[2] = userData.key2;

            if (chargeTimeCoroutine != null)
            {
                StopCoroutine(chargeTimeCoroutine);
            }
            chargeTimeCoroutine = StartCoroutine(UpdateChargeTime());

            for (int i = 0; i < 3; i++)
            {
                dungeonSlots[i].KeyTxt();
            }
        }
    }

    private void OnDisable()
    {
        if (chargeTimeCoroutine != null)
        {
            StopCoroutine(chargeTimeCoroutine);
            chargeTimeCoroutine = null;
        }
    }

    private IEnumerator UpdateChargeTime()
    {
        DateTime midnight = DateTime.Now.Date.AddDays(1);
        while (true)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeUntilMidnight = midnight - now;

            keyChargeTime.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                                timeUntilMidnight.Hours,
                                                timeUntilMidnight.Minutes,
                                                timeUntilMidnight.Seconds);
            yield return new WaitForSeconds(1f);
        }
    }

    public string GetKeyString(int type)
    {
        string result = $"{keys[type]}/3";
        return result;
    }

    public void LockDungeon()
    {
        UIManager.Instance.ShowWarn("업데이트 예정입니다.");
    }
}
