using System;

[Serializable]
public class RewardData : IData
{
    public string rcode;
    public string name;

    string IData.Rcode => rcode;  // 명시적 인터페이스 구현
}