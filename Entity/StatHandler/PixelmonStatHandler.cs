using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public static class PixelmonStatHandler
{
    private static string criDmg = "AddCriDmg";
    private static string dmg = "AddDmg";
    private static string sDmg = "AddSDmg";
    private static string sCriDmg = "AddSCriDmg";
    public static void InitStatus(this PixelmonStatus status, PixelmonData data, MyPixelmonData myData)
    {
        status.perAtk = SetStatus(data.basePerAtk, myData.lv, data.lvAtkRate);
        status.Atk = SetPlusStatus(1, myData.FindType(AbilityType.PSVAtk));
        status.Cri = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.Cri, myData.FindType(AbilityType.PSVCri));
        status.CriDmg = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.CriDmg, myData.FindType(AbilityType.PSVCriDmg), data.FindTraitType(criDmg, myData.lv));
        status.Dmg = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.Dmg, myData.FindType(AbilityType.PSVDmg), data.FindTraitType(dmg, myData.lv));
        status.SDmg = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.SDmg, myData.FindType(AbilityType.PSVSDmg), data.FindTraitType(sDmg, myData.lv));
        status.SCri = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.SCri, myData.FindType(AbilityType.PSVSCri));
        status.SCriDmg = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.SCriDmg, myData.FindType(AbilityType.PSVSCriDmg), data.FindTraitType(sCriDmg, myData.lv));
        status.AtkSpd = SetAtkSpdStatus(myData.FindType(AbilityType.PSVAtkSpd, true));
    }

    public static void ApplyMyPixelmon(UpgradeIndex type)
    {
        for (int i = 0; i < 5; i++)
        {
            if (PixelmonManager.Instance.player.pixelmons[i] != null)
            {
                PixelmonManager.Instance.player.pixelmons[i].ApplyStatus(type, PixelmonManager.Instance.player.pixelmons[i].data, PixelmonManager.Instance.player.pixelmons[i].myData);
            }
        }
    }

    public static void ApplyStatus(this Pixelmon pixelmon, UpgradeIndex type, PixelmonData data, MyPixelmonData myData)
    {
        switch (type)
        {
            case UpgradeIndex.Atk:
                pixelmon.status.Atk = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.Atk, myData.FindType(AbilityType.Attack));
                break;
            case UpgradeIndex.Cri:
                pixelmon.status.Cri = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.Cri, myData.FindType(AbilityType.PSVCri));
                break;
            case UpgradeIndex.CriDmg:
                pixelmon.status.CriDmg = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.CriDmg, myData.FindType(AbilityType.PSVCriDmg), data.FindTraitType(criDmg, myData.lv));
                break;
            case UpgradeIndex.Dmg:
                pixelmon.status.Dmg = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.Dmg, myData.FindType(AbilityType.PSVDmg), data.FindTraitType(dmg, myData.lv));
                break;
            case UpgradeIndex.SDmg:
                pixelmon.status.SDmg = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.SDmg, myData.FindType(AbilityType.PSVSDmg), data.FindTraitType(sDmg, myData.lv));
                break;
            case UpgradeIndex.SCri:
                pixelmon.status.SCri = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.SCri, myData.FindType(AbilityType.PSVSCri));
                break;
            case UpgradeIndex.SCriDmg:
                pixelmon.status.SCriDmg = SetPlusStatus(PixelmonManager.Instance.upgradeStatus.SCri, myData.FindType(AbilityType.PSVSCriDmg), data.FindTraitType(sCriDmg, myData.lv));
                break;
            default:
                break;
        }
    }

    public static float SetStatus(float perAtk, int lv, float lvAtkRate)
    {
        return (perAtk + lv * lvAtkRate + 100) * 0.01f;
    }

    public static float SetMultiStatus(float upgradeStat, float psvAtk, float traitValue = 1)
    {
        return upgradeStat * psvAtk * traitValue;
    }

    public static float SetPlusStatus(float upgradeStat, float psvAtk, float traitValue = 0)
    {
        return upgradeStat + psvAtk + traitValue;
    }

    public static float SetAtkSpdStatus(float psvValue)
    {
        return 1 * (1 - psvValue / 100);
    }

    public static void StatusUp(this PixelmonStatus status, string field, float stat, StatusType type = StatusType.Add)
    {
        var fieldInfo = status.GetType().GetField(field);
        if (fieldInfo != null)
        {
            var value = (float)fieldInfo.GetValue(status);
            switch(type)
            {
                case StatusType.Add:
                    fieldInfo.SetValue(status, value + stat);
                    break;
                case StatusType.Multiple:
                    fieldInfo.SetValue(status, value * stat);
                    break;
                case StatusType.Override:
                    fieldInfo.SetValue(status, value);
                    break;
            };
        }
    }

    public static void PxmLvUp(this MyPixelmonData myData)
    {
        if (myData.lv % 10 == 0)
        {
            SaveManager.Instance.UpdatePixelmonData(myData.id, "maxExp", (int)(myData.maxExp * 1.5f));
        }
        else
        {
            SaveManager.Instance.UpdatePixelmonData(myData.id, "maxExp", (int)(myData.maxExp * 1.1f));
        }
    }

    public static void PxmStarUp(this MyPixelmonData myData)
    {
        if ((myData.star & 1) == 0)
        {
            if (myData.star / 2 == 1)
            {
                //보유수치 상승
                for (int i = 0; i < myData.ownEffectValue.Length; i++)
                    myData.ownEffectValue[i] += 10;
                PixelmonManager.Instance.UpdatePlayerStat(10, 10);
            }
            else
            {
                for (int i = 0; i < myData.ownEffectValue.Length; i++)
                    myData.ownEffectValue[i] += 30;
                PixelmonManager.Instance.UpdatePlayerStat(30, 30);
            }
            SaveManager.Instance.UpdatePixelmonData(myData.id, nameof(myData.ownEffectValue), myData.ownEffectValue);
        }
        else
        {
            myData.psvSkill.Add(RandomPsv(myData.psvSkill));
            foreach (var pxm in Player.Instance.pixelmons)
            {
                if (pxm != null && pxm.myData != null && pxm.myData.id == myData.id)
                {
                    pxm.InitPxm();
                    break;
                }
            }
            SaveManager.Instance.UpdatePixelmonData(myData.id, nameof(myData.psvSkill), myData.psvSkill);
        }
    }

    public static void PsvRoulette(this MyPixelmonData myData, bool[] isLocked)
    {
        List<PsvSkill> originPsv = new List<PsvSkill>();
        for (int i = 0; i < myData.psvSkill.Count; i++)
        {
            if (!isLocked[i])
            {
                originPsv.Add(myData.psvSkill[i]);
            }
        }
        List<PsvSkill> newSkill = new List<PsvSkill>();
        newSkill.Capacity = 5;
        for (int i = 0; i < myData.psvSkill.Count; i++) 
        {
            if (!isLocked[i])
            {
                newSkill[i] = RandomPsv(originPsv);
            }
            else
            {
                newSkill[i] = myData.psvSkill[i];
            }
        }
        myData.psvSkill = newSkill.ToList();
        SaveManager.Instance.UpdatePixelmonData(myData.id, nameof(myData.psvSkill), myData.psvSkill);
    }


    public static PsvSkill RandomPsv(List<PsvSkill> psvList)
    {
        PsvSkill newSkill = new PsvSkill();
        BasePsvData basePsvData;
        do
        {
            basePsvData = RandAbilityUtil.RandAilityData();
        }
        while (PsvOverlapCheck(psvList, basePsvData));

        var randAbility = RandAbilityUtil.PerformAbilityGacha((AbilityType)basePsvData.psvEnum, basePsvData.maxRate);
        newSkill.psvType = (AbilityType)basePsvData.psvEnum;
        newSkill.psvName = basePsvData.rcode;
        newSkill.psvRank = randAbility.AbilityRank;
        newSkill.psvValue = randAbility.AbilityValue;
        return newSkill;
    }

    public static bool PsvOverlapCheck(List<PsvSkill> psvList, BasePsvData psvData)
    {
        for (int i = 0; i < psvList.Count; i++)
        {
            if (psvData.rcode == psvList[i].psvName)
            {
                return true;
            }
        }
        return false;
    }


    public static List<PsvSkill> PxmPsvEffect(this PixelmonStatus status, MyPixelmonData myData, bool[] isLocked)
    {

        List<PsvSkill> newSkills = new List<PsvSkill>();
        for (int i = 0; i < myData.psvSkill.Count; i++)
        {
            if (!isLocked[i])
            {
                PsvSkill newSkill = new PsvSkill();
                int randType = Random.Range(0, 7);
                newSkill.psvType = (AbilityType)randType;
                //TODO : 수치 값 랜덤 후 대입
                newSkills.Add(newSkill);
            }
            else
            {
                newSkills.Add(myData.psvSkill[i]);
            }
        }       
        return newSkills;
    }
    
    public static (BigInteger, bool) GetTotalDamage(this PixelmonStatus status, MyPixelmonData myData, bool isSkill = false, float perSkill = 1)
    {
        BigInteger dealDmg;
        bool isCri = false;
        if (isSkill)
        {
            if (IsCritical(status.Cri + status.SCri))
            {
                isCri = true;
                dealDmg = (BigInteger)((100 + PixelmonManager.Instance.upgradeStatus.Atk) * ((100 + perSkill + status.SDmg) / 100 + status.perAtk) * (100 + status.SCriDmg) / 100);
            }
            else
                dealDmg = (BigInteger)((100 + PixelmonManager.Instance.upgradeStatus.Atk) * ((100 + perSkill + status.SDmg) / 100 + status.perAtk));
        }
        else
        {
            if (IsCritical(status.Cri))
            {
                isCri= true;
                dealDmg = (BigInteger)((100 + PixelmonManager.Instance.upgradeStatus.Atk) * ((100 + status.Dmg) / 100 + status.perAtk) * (100 + status.CriDmg) / 100);
            }
            else
                dealDmg = (BigInteger)((100 + PixelmonManager.Instance.upgradeStatus.Atk) * ((100 + status.Dmg)/100 + status.perAtk));
        }
        //버프가 있다면 dealDmg *= 1;

        return (dealDmg, isCri);
    }


    public static bool IsCritical(float rate)
    {
        return Random.Range(0, 10000) <= rate * 100;
    }
}

public enum StatusType
{
    Add,
    Multiple,
    Override
}