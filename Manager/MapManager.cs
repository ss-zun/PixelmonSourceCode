using UnityEngine;

public enum MapList
{
    DgGold,
    DgSeed,
    DgSkill,
    Theme1,
    Theme2, 
    Theme3
}

public class MapManager : Singleton<MapManager>
{
    public GameObject[] mapList;
    private int curMap = (int)MapList.Theme1;

    public void OnMapChanged(int map)
    {
        mapList[curMap].SetActive(false);
        mapList[map].SetActive(true);
        curMap = map;
    }
}
