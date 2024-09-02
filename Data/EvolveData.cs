using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EvolveData : IData
{
    public string rcode;

    public int star1;
    public int star2;
    public int star3;
    public int star4;
    public int star5;

    string IData.Rcode => rcode;
    public string Rank => rcode;
}