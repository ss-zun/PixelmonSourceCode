using System;
using UnityEngine;

[Serializable]
public class BasePsvData : IData
{
    public string rcode;
    public int psvEnum;
    public float maxRate;
    public float weight;

    string IData.Rcode => rcode;
}
