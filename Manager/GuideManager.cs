using System;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : Singleton<GuideManager>
{
    public GameObject[] Locks;
    public GameObject GuideArrow;
    public GameObject PxmToggle;
    public int guideNum = 100;
    private UserData userData => SaveManager.Instance.userData;

    #region Tutorial Start Indexes
    private readonly HashSet<int> guideNumSet = new HashSet<int> { 0, 1, 3, 6, 11, 29, 40, 49 };
    public event Action<int> OnGuideAction;

    public readonly int equipPixelmon = 1;
    public readonly int setAllPixelmon = 3;
    public readonly int nestLvUp = 6;
    public readonly int upgrAtk = 11;
    public readonly int skillGatcha = 29;
    public readonly int seedFarm = 40;
    public readonly int goldDg = 49;
    #endregion

    #region Specific Quest 
    public readonly int skillEquip = 30;
    #endregion

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();
    }

    public void SetArrow(GameObject obj, float addYPos = 0)
    {
        Vector3 currentPosition = obj.transform.position;
        Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y + 20f + addYPos, currentPosition.z);
        GuideArrow.transform.position = newPosition;
    }

    public void GuideNumTrigger(int guideNum)
    {
        if (guideNumSet.Contains(guideNum))
        {
            OnGuideAction?.Invoke(guideNum);
        }
        else
        {
            GuideArrow.SetActive(false);
        }
    }

    public void SetBottomLock()
    {
        if (guideNum > 0)
        {
            Locks[0].SetActive(false);
        }
        
        if (guideNum >= upgrAtk)
        {
            Locks[1].SetActive(false);
        }

        if (guideNum >= skillGatcha)
        {
            Locks[2].SetActive(false);
            Locks[5].SetActive(false);
        }

        if (guideNum >= seedFarm)
        {
            Locks[3].SetActive(false);
        }

        if (guideNum >= goldDg)
        {
            Locks[4].SetActive(false);
        }
    }
}
