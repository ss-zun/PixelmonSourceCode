
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum eAddressableType
{
    prefab,
    thumbnail,
    ui,
    animator,
    font,
    sound,
    data
}

public enum eAssetType
{
    sprite = 0,
    jsondata,
    prefab
}

[Serializable]
public class AddressableMapData
{
    public List<AddressableMap> list = new List<AddressableMap>();

    public void AddRange(List<AddressableMap> list)
    {
        this.list.AddRange(list);
    }

    public void Add(AddressableMap data)
    {
        list.Add(data);
    }
}

[Serializable]
public class AddressableMap
{
    public eAddressableType addressableType;
    public eAssetType assetType;
    public string key;
    public string path;
}