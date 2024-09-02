using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 확률에 따라 픽셀몬 패시브 등급 랜덤뽑기
/// </summary>
public static class RandAbilityUtil
{
    public static BasePsvData RandAilityData()
    {
        int randomIndex = UnityEngine.Random.Range(0, DataManager.Instance.basePsvData.data.Count);       
        return DataManager.Instance.basePsvData.data[randomIndex];
    }

    /// <summary>
    /// abilityType = (AbilityType)RandAilityData.psvEnum; (랜덤일 때)
    /// maxRate = RandAilityData.maxRate;
    /// </summary>
    /// <param name="abilityType"></param>
    /// <param name="maxRate"></param>
    /// <returns></returns>
    public static (string AbilityRank, float AbilityValue) PerformAbilityGacha(AbilityType abilityType, float maxRate, float oldValue = 0)
    {
        var data = DataManager.Instance.abilityRateData.data;

        #region 확률 합이 100인지 체크
        float totalProb = 0;
        foreach (var prob in data)
        {
            totalProb += prob.dropRate;
        }
        if (totalProb != 100)
        {
            //Debug.LogError("확률 합 != 100");
            return (null, 0); // 확률 합이 100이 아닐 때 null과 0 반환
        }
        #endregion

        #region 능력치 등급 랜덤(rocde)
        int randProb = UnityEngine.Random.Range(100, 10001);
        float cumProb = 0;
        string dropRcode = null;

        foreach (var prob in data)
        {
            cumProb += prob.dropRate * 100;
            if (randProb <= cumProb)
            {
                dropRcode = prob.rcode;
                break;
            }
        }
        #endregion

        #region 능력치값 랜덤(min~max)
        float abilityValue;
        int randValue;
        var dropData = DataManager.Instance.GetData<AbilityRateData>(dropRcode);     
        do
        {
            randValue = UnityEngine.Random.Range(dropData.min, dropData.max + 1);
            abilityValue = (randValue / 100f) * maxRate;
        } while (oldValue != 0 && abilityValue == oldValue);
        #endregion

        return (dropRcode, abilityValue);
    }
}
