using System;

[Serializable]
public class QuestData : IData
{
    public string rcode;
    public string strType;
    public string description;
    public int goal;
    public string rewardType;
    public int rewardValue;

    public QuestType type
    {
        get
        {
            if (Enum.TryParse(strType, out QuestType result)) return result;
            else return QuestType.Default;
        }
    }
    string IData.Rcode => rcode;
}