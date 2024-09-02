using System;
using System.Collections.Generic;
using System.Numerics;

[Serializable]
public class UserData
{
    public string version;

    #region Private User
    public string userId;
    public string userName = "아그니스";
    public int userLv = 1;
    public string _exp = "0";
    public BigInteger userExp = 0;
    #endregion

    #region Goods
    public string _gold = "0";
    public BigInteger gold = 0;
    public int diamond = 0;
    public int seed = 3;
    public int food = 0;
    public int skillTicket = 0;
    #endregion

    #region Settings
    public float BGMVolume = 0.8f;
    public float SFXVolume = 0.8f;
    #endregion

    #region Tutorial
    public int tutoIndex = 0; //0:first, 1:egg, 2:getPxm, 3:pxmTab, 4:end.
    #endregion

    #region Stage Info
    public string stageRcode = "STG_N1";
    public string curStage = "000101";
    public int curHuntCount = 0;
    public bool isInfinite;
    #endregion

    #region Main Quest
    public string questIndex = "0_Q1";
    public int questProgress = 0;
    #endregion

    #region EggLv
    public int eggLv = 1;
    public int fullGaugeCnt = 0;
    public bool isLvUpMode = false;
    public string startLvUpTime = null;
    public float skipTime = 0;
    #endregion

    #region Hatch Egg
    public int eggCount = 10;
    public bool isGetPxm = true;
    public bool isOwnedPxm = false;
    public PixelmonData hatchPxmData = null;
    public MyPixelmonData hatchMyPxmData = null;
    public PxmPsvData[] psvData = new PxmPsvData[4];
    #endregion

    #region Upgrade Tab
    public int[] UpgradeLvs = { 1, 1, 1, 1, 1, 1, 1 };
    public bool[] isLockedSlot = { false, false, true, true, true };
    #endregion

    #region Pixelmon Tab
    public MyPixelmonData[] equippedPxms = new MyPixelmonData[5];
    public List<MyPixelmonData> ownedPxms = new List<MyPixelmonData>();
    #endregion 

    #region Skill Tab
    public int[] equippedSkills = {-1, -1, -1, -1, -1};
    public List<MyAtvData> ownedSkills = new List<MyAtvData>();
    #endregion

    #region Farm Tab
    public FieldData[] fieldDatas = new FieldData[6];
    #endregion

    #region Dungeon Tab
    public int[] bestDgLvs = { 1, 1, 1 };
    public int key0 = 3;
    public int key1 = 3;
    public int key2 = 3;
    public string lastConnectTime;
    #endregion

    #region Ads
    public int adsCount = 4;
    #endregion
}
