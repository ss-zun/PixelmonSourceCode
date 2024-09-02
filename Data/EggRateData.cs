using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EggRateData : IData
{
    #region json keys
    public string rcode;
    public float common;
    public float advanced;
    public float rare;
    public float epic;
    public float legendary;
    public float lvUpTime;
    #endregion

    string IData.Rcode => rcode;
}
