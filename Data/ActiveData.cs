using System;
using UnityEngine;

[Serializable]
public class ActiveData : IData
{
    public string rcode;
    public int id;
    public string rank;
    public string name;
    public string description;
    public int type;
    public AtvSkillType Type => (AtvSkillType)type;
    public bool isCT;
    public float coolTime;
    public float maxRate;
    public string sound;

    public string prefabrcode;
    public int count = 1;
    public float range;
    public float scale;

    public int dataIndex;
    public Sprite icon;
    public Sprite bgIcon;

    public string Rcode => rcode;
    public string[] Soundrcode => sound.Split(' ');

}
