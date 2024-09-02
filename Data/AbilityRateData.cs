using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityRateData : IData
{
    public string rcode;
    public int min;
    public int max;
    public float dropRate;

    string IData.Rcode => rcode;
}
