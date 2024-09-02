using System.Numerics;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    private UserData userData;
    private PoolManager poolManager;

    protected override void Awake()
    {
        isDontDestroyOnLoad = true;
        base.Awake();

        userData = SaveManager.Instance.userData;
        poolManager = PoolManager.Instance;
    }

    public void SpawnRewards(GameObject go, string[] rcodes, int[] amounts, float[] rates = null)
    {
        for (int i = 0; i < rcodes.Length; i++)
        {
            if (rates == null || CheckDropRate(rates[i]))
            {
                if (poolManager.PoolDictionary.ContainsKey(rcodes[i]))
                {
                    poolManager.SpawnFromPool<DropItem>(rcodes[i]).ExeCuteSequence(go, amounts[i]);
                }
                else
                {
                    string itemName = DataManager.Instance.GetData<RewardData>(rcodes[i]).name;
                    GetReward(itemName, amounts[i]);
                }
            }
        }
    }

    public void SpawnRewards(string rcode, int amount)
    {
        string itemName = DataManager.Instance.GetData<RewardData>(rcode).name;
        GetReward(itemName, amount);
    }

    public void GetReward(string itemName, int _amount)
    {
        int stageCount = StageManager.Instance.stageNum
                    + StageManager.Instance.worldNum * 15
                    + StageManager.Instance.diffNum * 15 * 10;

        switch (itemName)
        {
            case nameof(userData.gold):
                BigInteger amount1 = _amount;
                amount1 *= ((stageCount - 1) * 235 + 100) / 100;
                SaveManager.Instance.SetFieldData(itemName, amount1, true);
                break;
            case nameof(userData.userExp):
                amount1 = _amount;
                amount1 *= ((stageCount - 1) * 100 + 100) / 100;
                SaveManager.Instance.SetFieldData(itemName, amount1, true);
                
                break;
            default:
                int amount3 = _amount;
                SaveManager.Instance.SetFieldData(itemName, amount3, true);
                break;
        }
    }

    private bool CheckDropRate(float rate)
    {
        return UnityEngine.Random.Range(0, 100) <= rate;
    }
}