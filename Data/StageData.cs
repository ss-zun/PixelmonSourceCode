using System;

[Serializable]
public class StageData : IData
{
    public string rcode;

    #region json keys
    public int spawnCount;
    public int nextStageCount;
    public string monsterIds;
    public string offlineRewardTypes;
    public string offlineRewardValues;
    #endregion

    #region converted arrays
    public string[] monsterId => monsterIds.Split(' ');
    public string[] offlineRewardType => offlineRewardTypes.Split(' ');
    public int[] offlineRewardValue => Array.ConvertAll(offlineRewardValues.Split(' '), int.Parse);
    #endregion

    string IData.Rcode => rcode;
}