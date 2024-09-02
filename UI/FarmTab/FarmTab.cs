using TMPro;
using UnityEngine;

public class FarmTab : UIBase
{
    private SaveManager saveManager;
    private UserData userData;

    [SerializeField] public FieldSlot[] fieldSlots;

    #region User Data
    private int seedCount => userData.seed;
    private int foodCount => userData.food;
    #endregion

    #region UI
    [SerializeField] private TextMeshProUGUI seedTxt;
    [SerializeField] private TextMeshProUGUI foodTxt;
    #endregion

    int i = 0;

    private bool isAwakeEnabled;
    protected override void Awake()
    {
        isAwakeEnabled = false;
        base.Awake();
        saveManager = SaveManager.Instance;
        userData = saveManager.userData;
        UIManager.Instance.UpdateUI += UpdateFieldUI;

        for (int i = 0; i < fieldSlots.Length; i++)
        {
            fieldSlots[i].farmTab = this;
            fieldSlots[i].slotIndex = i;
            fieldSlots[i].fieldData = userData.fieldDatas[i];
        }
        seedTxt.text = seedCount.ToString();
        foodTxt.text = foodCount.ToString();
        isAwakeEnabled = true;
    }

    private void OnEnable()
    {
        if (isAwakeEnabled && i != 0)
        {
            seedTxt.text = seedCount.ToString();
            foodTxt.text = foodCount.ToString();

            for (int i = 0; i < fieldSlots.Length; i++)
            {
                if (fieldSlots[i].fieldData.currentFieldState == FieldState.Seeded)
                {
                    fieldSlots[i].CalculateRemainingTime();
                }
                fieldSlots[i].CurrentFieldAction(fieldSlots[i].CurrentFieldState);
                SetAllPriceTxts();
            }
        }
        i++;
    }

    private void OnDisable()
    {
        for (int i = 0; i < fieldSlots.Length; i++)
        {
            if (fieldSlots[i].GetFoodObj.activeSelf)
            {
                fieldSlots[i].GetFoodObj.SetActive(false);
            }
        }
    }

    private void UpdateFieldUI(DirtyUI dirtyUI)
    {
        switch (dirtyUI)
        {
            case DirtyUI.Seed:
                seedTxt.text = seedCount.ToString();
                break;
            case DirtyUI.Food:
                foodTxt.text = foodCount.ToString();
                break;
        }
    }

    public void UnlockNextField(int buyIndex)
    {
        if (buyIndex > 4) return;
        fieldSlots[buyIndex + 1].CurrentFieldState = FieldState.Buyable;
        SaveFarmData();
    }

    public bool PlantSeed()
    {
        if (seedCount < 5)
        {
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20008").clip);
            UIManager.Instance.ShowWarn("씨앗이 부족합니다!!");
            return false;
        }
        else
        {
            saveManager.SetFieldData(nameof(saveManager.userData.seed), -5, true);
            return true;
        }
    }

    public void SaveFarmData()
    {
        FieldData[] temp = new FieldData[6];
        for (int i = 0; i < temp.Length; i++)
        {
            FieldData tempItem = fieldSlots[i].fieldData;
            temp[i] = tempItem;
        }
        saveManager.SetFieldData(nameof(userData.fieldDatas), temp);
    }

    public void SetAllPriceTxts()
    {
        for (int i = 0; i < 6; i++)
        {
            fieldSlots[i].SetPriceTxt();
        }
    }
}