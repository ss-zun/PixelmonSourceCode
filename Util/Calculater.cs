using System;
using System.Numerics;

public static class Calculater
{
    #region Price Calculator
    /// <param name="lv"></param>
    /// <param name="baseNum">lv 1일 때 value</param>
    /// <param name="d1">lv1 -> lv2 증가량</param>
    /// <param name="d2">diff의 증가량</param>
    /// <returns></returns>
    public static BigInteger CalPrice(BigInteger lv, BigInteger baseNum, BigInteger d1, BigInteger d2)
    {
        if (lv <= 0) return 0;
        if (lv == 1) return baseNum;

        BigInteger lvMinus1 = lv - 1;
        BigInteger lvMinus2 = lv - 2;

        BigInteger result = baseNum + d1 * lvMinus1 + (d2 * lvMinus1 * lvMinus2) / 2;
        return result;
    }

    /// <param name="lv"></param>
    /// <param name="baseNum">lv 1일 때 value</param>
    /// <param name="d1">lv1 -> lv2 증가량</param>
    /// <param name="d2">diff의 증가량</param>
    /// <returns></returns>
    public static BigInteger CalPriceSum(BigInteger lv, BigInteger baseNum, BigInteger d1, BigInteger d2)
    {
        if (lv <= 0) return 0;

        BigInteger sumBaseNum = baseNum * lv;
        BigInteger sumD1 = d1 * (lv * (lv - 1) / 2);
        BigInteger sumD2 = d2 * ((lv - 1) * (lv - 2) * lv / 6);
        BigInteger result = sumBaseNum + sumD1 + sumD2;

        return result;
    }
    #endregion

    #region Translator
    /// <param name="number">float 값을 넣을 때는 Mathf.RoundToInt를 이용할 것.</param>
    public static string NumFormatter(BigInteger number)
    {
        if (number < 1000)
        {
            return number.ToString();
        }

        int alphabetIndex = 0;

        while (number >= 1000000 && alphabetIndex < 26)
        {
            number /= 1000;
            alphabetIndex++;
        }

        float distanceNum = (float)number / 1000;
        alphabetIndex++;

        char suffix = (char)('A' + alphabetIndex - 1);
        return distanceNum.ToString("0.##") + suffix;
    }
    #endregion
}