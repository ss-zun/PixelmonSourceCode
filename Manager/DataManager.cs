using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : GSpreadReader<DataManager>
{
    private readonly Dictionary<string, IData> dataDics = new Dictionary<string, IData>();

    public GameData<StageData> stageData;
    public GameData<PixelmonData> pixelmonData;
    public GameData<EnemyData> enemyData;
    public GameData<RewardData> rewardData;
    public GameData<EggRateData> eggRateData;
    public GameData<EvolveData> evolveData;
    public GameData<AbilityRateData> abilityRateData;
    public GameData<BasePsvData> basePsvData;
    public GameData<ActiveData> activeData;
    public GameData<QuestData> questData;
    public GameData<SoundData> soundData;
    public Sprite[] pxmBgIcons;
    public Sprite[] skillBgIcons;
 

    public bool isPxmInit;
    public async Task SetBaseData()
    {
        float progress = 1.0f;
        foreach (var data in soundData.data)
        {
            UILoading.Instance.SetProgress(progress++ / soundData.data.Count, "선율을 다듬는 중");
            data.clip = await ResourceManager.Instance.LoadAsset<AudioClip>(data.rcode, eAddressableType.sound);
        }

        progress = 1.0f;
        foreach (var data in pixelmonData.data)
        {
            UILoading.Instance.SetProgress(progress++ / pixelmonData.data.Count, "픽셀몬 부화중");
            data.icon = await ResourceManager.Instance.LoadAsset<Sprite>(data.rcode, eAddressableType.thumbnail);
            switch (data.rank)
            {
                case "Common":
                    data.bgIcon = pxmBgIcons[0];
                    data.rankIdx = 0;
                    break;
                case "Advanced":
                    data.bgIcon = pxmBgIcons[1];
                    data.rankIdx = 1;
                    break;
                case "Rare":
                    data.bgIcon = pxmBgIcons[2];
                    data.rankIdx = 2;
                    break;
                case "Epic":
                    data.bgIcon = pxmBgIcons[3];
                    data.rankIdx = 3;
                    break;
                case "Legendary":
                    data.bgIcon = pxmBgIcons[4];
                    data.rankIdx = 4;
                    break;
                case "Unique":
                    data.bgIcon = pxmBgIcons[5];
                    data.rankIdx = 5;
                    break;
                default:
                    break;
            }
        }

        progress = 1.0f;
        foreach (var data in activeData.data)
        {
            UILoading.Instance.SetProgress(progress++ / activeData.data.Count, "마법 배우는 중");
            data.icon = await ResourceManager.Instance.LoadAsset<Sprite>(data.rcode, eAddressableType.thumbnail);
            switch (data.rank)
            {
                case "C":
                    data.bgIcon = skillBgIcons[0]; 
                    break;
                case "B":
                    data.bgIcon = skillBgIcons[1];
                    break;
                case "A":
                    data.bgIcon = skillBgIcons[2];
                    break;
                case "S":
                    data.bgIcon = skillBgIcons[3];
                    break;
                case "SS":
                    data.bgIcon = skillBgIcons[4];
                    break;
                default:
                    break;
            }
        }

        pixelmonData.data.Sort((x, y) =>
        {
            return x.rankIdx.CompareTo(y.rankIdx);
        });

        for(int i = 0; i < pixelmonData.data.Count; i++) 
        {
            pixelmonData.data[i].id = i;
        }

        while (SaveManager.Instance.userData.ownedPxms.Count <= pixelmonData.data.Count)
        {
            SaveManager.Instance.userData.ownedPxms.Add(new MyPixelmonData());
        }
        var removeList = new List<string>();
        SaveManager.Instance.userData.ownedPxms.ForEach((obj) => {
            if (!string.IsNullOrEmpty(obj.rcode))
            {
                var data = GetData<PixelmonData>(obj.rcode);
                if (data != null)
                    obj.id = data.id;
                else
                    removeList.Add(obj.rcode);
            }
        });
        SaveManager.Instance.userData.ownedPxms.RemoveAll(obj => removeList.Contains(obj.rcode));
        SaveManager.Instance.SetData(nameof(SaveManager.Instance.userData.ownedPxms), SaveManager.Instance.userData.ownedPxms);

        isPxmInit = true;
    }

    public T GetData<T>(string rcode) where T : class, IData
    {
        return (T)dataDics[rcode];
    }

    public override void AddDataDics<T>(List<T> datas)
    {
        foreach (T data in datas)
        {
            if (!dataDics.ContainsKey(data.Rcode)) if (!dataDics.ContainsKey(data.Rcode))
                    dataDics.Add(data.Rcode, data);
        }
    }
}