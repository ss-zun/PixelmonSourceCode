using System;
using UnityEngine;

[Serializable]
public class SoundData : IData
{
    public string rcode;
    public string name;
    public string description;
    public bool isLoop;
    public float volume;
    public AudioClip clip;
    public string Rcode => rcode;
}