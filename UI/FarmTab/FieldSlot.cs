using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

public class FieldSlot : MonoBehaviour
{
    private SaveManager saveManager;
    private UserData userData;

    public FarmTab farmTab;
    public int slotIndex;
    public FieldData fieldData;

    [SerializeField] private int price;
    [SerializeField] private float passTime;

    #region properties
    public FieldState CurrentFieldState 
    {
        get => fieldData.currentFieldState;
        set
        {
            if (fieldData.currentFieldState != value)
            {
                fieldData.currentFieldState = value;
                CurrentFieldAction(value);
            }
        }
    }
    #endregion

    #region UI
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button seedBtn;
    [SerializeField] private Button harvestBtn;
    
    [SerializeField] private Sprite[] plantImgs;
    [SerializeField] private Image curSprite;

    [SerializeField] private Slider timeSldr;
    [SerializeField] private TextMeshProUGUI timeTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;

    public GameObject GetFoodObj;
    [SerializeField] private TextMeshProUGUI harvestTxt;
    #endregion

    Coroutine growingCoroutine;

    private void Awake()
    {
        saveManager = SaveManager.Instance;
        userData = saveManager.userData;
    }

    public void CurrentFieldAction(FieldState state)
    {
        switch (state)
        {
            case FieldState.Locked: //잠김.
                break;

            case FieldState.Buyable: //구매가능
                /*Btn Settings*/
                buyBtn.gameObject.SetActive(true);
                /*UI Settings*/
                break;

            case FieldState.Empty: //빈 밭.
                /*Btn Settings*/
                buyBtn.transform.parent.gameObject.SetActive(false);
                seedBtn.gameObject.SetActive(true);
                harvestBtn.gameObject.SetActive(false);
                /*UI Settings*/
                curSprite.gameObject.SetActive(false);
                break;

            case FieldState.Seeded: //작물이 심긴 밭.
                /*Btn Settings*/
                buyBtn.transform.parent.gameObject.SetActive(false);
                seedBtn.gameObject.SetActive(false);
                /*UI Settings*/
                curSprite.gameObject.SetActive(true);
                curSprite.sprite = plantImgs[0];
                timeSldr.gameObject.SetActive(true);
                /*TMP Settings*/
                if (growingCoroutine != null)
                {
                    StopCoroutine(growingCoroutine);
                }
                growingCoroutine = StartCoroutine(plantGrowing()); //남은 시간
                break;

            case FieldState.Harvest: //수확 준비된 밭.
                /*Btn Settings*/
                buyBtn.transform.parent.gameObject.SetActive(false);
                harvestBtn.gameObject.SetActive(true);
                /*UI Settings*/
                curSprite.gameObject.SetActive(true);
                curSprite.sprite = plantImgs[fieldData.cropClass];
                timeSldr.gameObject.SetActive(false);
                /*TMP Settings*/
                if (growingCoroutine != null)
                {
                    StopCoroutine(growingCoroutine);
                }
                break;
        }
    }

    public void OnBuyBtn()
    {
        if (slotIndex < 4)
        {
            if (price <= userData.gold)
            {
                saveManager.SetFieldData(nameof(userData.gold), -price, true);
            }
            else
            {
                AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20008").clip);
                UIManager.Instance.ShowWarn("골드가 부족합니다!!");
                return;
            }
        }
        else
        {
            if (price <= userData.diamond)
            {
                saveManager.SetFieldData(nameof(userData.diamond), -price, true);
            }
            else
            {
                AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20008").clip);
                UIManager.Instance.ShowWarn("다이아가 부족합니다!!");
                return;
            }
        }
        CurrentFieldState = FieldState.Empty;
        buyBtn.transform.parent.gameObject.SetActive(false);
        farmTab.SaveFarmData();
        farmTab.UnlockNextField(slotIndex);
        farmTab.SetAllPriceTxts();
    }

    public void OnSeedBtn()
    {
        if (farmTab.PlantSeed())
        {
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20003").clip);
            RandomCrop();
            fieldData.leftTime = 900f;
            passTime = fieldData.leftTime;
            fieldData.startTime = DateTime.Now.ToString();
            CurrentFieldState = FieldState.Seeded;
            if (QuestManager.Instance.IsMyTurn(QuestType.Seed))
            {
                QuestManager.Instance.OnQuestEvent();
            }
        }
        farmTab.SaveFarmData();
    }

    public void OnHarvestBtn()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20004").clip);
        HarvestYield(fieldData.cropClass);
        GetFoodObj.SetActive(true);
        CurrentFieldState = FieldState.Empty;
        farmTab.SaveFarmData();

        if (QuestManager.Instance.IsMyTurn(QuestType.Harvest))
        {
            QuestManager.Instance.OnQuestEvent();
        }
    }

    public void SetPriceTxt()
    {
        if (CurrentFieldState == FieldState.Locked)
        {
            priceTxt.color = Color.red;
            return;
        }

        if (slotIndex < 4 && price > userData.gold)
        {
            priceTxt.color = Color.red;
            return;
        }

        if (slotIndex > 3 &&  price > userData.diamond)
        {
            priceTxt.color = Color.red;
            return;
        }

        priceTxt.color = Color.white;
    }

    private void RandomCrop()
    {
        int randNum = UnityEngine.Random.Range(0, 100);
        if (randNum < 69)
        {
            fieldData.cropClass = 1;
        }
        else if (randNum < 92)
        {
            fieldData.cropClass = 2;
        }
        else if (randNum < 99)
        {
            fieldData.cropClass = 3;
        }
        else
        {
            fieldData.cropClass = 4;
        }
    }

    private IEnumerator plantGrowing()
    {
        timeTxt.gameObject.SetActive(true);
        while (passTime > 0)
        {
            passTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt((passTime % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(passTime % 60f);
            timeTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            timeSldr.value = passTime / fieldData.leftTime;
            yield return null;
        }
        timeTxt.gameObject.SetActive(false);
        CurrentFieldState = FieldState.Harvest;
    }

    public void CalculateRemainingTime()
    {
        DateTime time = DateTime.Parse(fieldData.startTime);
        TimeSpan elapsed = DateTime.Now - time;
        passTime = fieldData.leftTime - (float)elapsed.TotalSeconds;

        if (passTime <= 0)
        {
            fieldData.currentFieldState = FieldState.Harvest;
        }
    }

    public void HarvestYield(int yield)
    {
        if (yield == 4)
        {
            saveManager.SetFieldData(nameof(saveManager.userData.food), 100, true);
            harvestTxt.text = "x100";
            return;
        }

        yield = yield switch
        {
            1 => 2,
            2 => 4,
            3 => 7,
            _ => 0
        };

        int randNum = UnityEngine.Random.Range(0, 100);
        if (randNum < 40)
        {
            yield *= 3;
            saveManager.SetFieldData(nameof(saveManager.userData.food), yield, true);
        }
        else if (randNum < 75)
        {
            yield *= 5;
            saveManager.SetFieldData(nameof(saveManager.userData.food), yield, true);
        }
        else
        {
            yield *= 8;
            saveManager.SetFieldData(nameof(saveManager.userData.food), yield, true);
        }
        harvestTxt.text = "x" + yield;
    }
}