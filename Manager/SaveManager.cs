using System.IO;
using System.Collections;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Numerics;

public class SaveManager : Singleton<SaveManager>
{
    public UserData userData = new UserData();
    [SerializeField] private DataManager dataManager;

    private string userPath;
    private string initPath;

    private static bool isDirty;
    private WaitUntil CheckDirty = new WaitUntil(() => isDirty);

    protected override void Awake()
    {
        base.Awake();
        initPath = "InitData";
        userPath = Path.Combine(Application.persistentDataPath, "userData.json");
        LoadData();
        
        if (userData.version == null)
        {
            File.Delete(userPath);
            LoadData();
            SetFieldData(nameof(userData.version), "v1.0.7");
        }

        SetFieldData(nameof(userData.gold), BigInteger.Parse(userData._gold));
        SetFieldData(nameof(userData.userExp), BigInteger.Parse(userData._exp));
    }

    void Start()
    {
        dataManager = DataManager.Instance;
        StartCoroutine(ChangedValue());
    }

    public void SaveToJson<T>(T data, string path = null)
    {
        path ??= userPath;

        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, jsonData);
    }

    public void LoadData()
    {
        if (File.Exists(userPath))
        {
            LoadFromJson(userPath);
        }
        else if (Resources.Load<TextAsset>(initPath) != null)
        {
            LoadFromResources(initPath);
            SaveToJson(userData, userPath);
        }
        else
        {
            userData = new UserData();
            SaveToJson(userData, userPath);
            LoadData();
        }
    }

    public void LoadFromJson(string path)
    {
        string jsonData = File.ReadAllText(path);
        userData = JsonUtility.FromJson<UserData>(jsonData);
    }

    public void LoadFromResources(string resourceName)
    {
        TextAsset jsonData = Resources.Load<TextAsset>(resourceName);
        if (jsonData != null)
        {
            userData = JsonUtility.FromJson<UserData>(jsonData.text);
        }
    }

    private IEnumerator ChangedValue()
    {
        while (true)
        {
            yield return CheckDirty;
            isDirty = false;
            SaveToJson(userData);
        }
    }

    private async void SaveDataAsync()
    {
        await Task.Run(() => SaveToJson(userData));
    }

    public void SetData(string field, object value)
    {
        var fieldInfo = userData.GetType().GetField(field);
        fieldInfo.SetValue(userData, value);
        isDirty = true;
    }

    public void SetDeltaData(string field, int value)
    {
        var fieldInfo = userData.GetType().GetField(field);
        int currentValue = (int)fieldInfo.GetValue(userData);
        fieldInfo.SetValue(userData, currentValue + value);
        isDirty = true;
    }

    public void SetFieldData(string field, object value, bool isDelta = false)
    {
        var fieldInfo = userData.GetType().GetField(field);
        var currentValue = fieldInfo.GetValue(userData);

        if (isDelta)
        {
            if (currentValue is int currentInt)
            {
                fieldInfo.SetValue(userData, currentInt + (int)value);
            }
            else if (currentValue is BigInteger currentBigInt)
            {
                BigInteger bigIntValue = value switch
                {
                    int intValue => new BigInteger(intValue),
                    BigInteger bigIntegerValue => bigIntegerValue,
                    _ => 0
                };

                fieldInfo.SetValue(userData, currentBigInt + bigIntValue);
            }
            else return;
        }
        else
        {
            fieldInfo.SetValue(userData, value);
        }

        if (field == nameof(userData.gold))
        {
            userData._gold = userData.gold.ToString();
        }
        else if (field == nameof(userData.userExp))
        {
            userData._exp = userData.userExp.ToString();
        }

        isDirty = true;

        if (Enum.TryParse(field, true, out DirtyUI dirtyUI))
        {
            UIManager.Instance.InvokeUIChange(dirtyUI);
        }
    }

    public void UpdatePixelmonData(int index, string field, object value)
    {
        userData.ownedPxms[index].UpdateField(field, value);
        isDirty = true;
    }

    public void UpdateSkillData(int index, string field, object value)
    {
        userData.ownedSkills[index].UpdateField(field, value);
        isDirty = true;
    }
}