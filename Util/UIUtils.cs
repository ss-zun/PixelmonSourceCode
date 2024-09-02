using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtils
{
    public static IEnumerator AnimateTextChange(TextMeshProUGUI textElement, int startValue, int endValue, float duration = 0.5f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentValue = (int)Mathf.Lerp(startValue, endValue, elapsed / duration);
            textElement.text = currentValue.ToString();
            yield return null;
        }

        textElement.text = endValue.ToString();
    }

    public static IEnumerator AnimateSliderChange(Slider slider, float startValue, float endValue, float duration = 0.5f)
    {
        slider.value = startValue;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            slider.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        slider.value = endValue;
    }

    public static string TranslateRank(this PxmRank rank)
    {
        switch (rank)
        {
            case PxmRank.Common: return "일반";
            case PxmRank.Advanced: return "고급";
            case PxmRank.Rare: return "희귀";
            case PxmRank.Epic: return "영웅";
            case PxmRank.Legendary: return "전설";
        }
        return null;
    }

    public static string TranslateTraitEnum(this TraitType trait)
    {
        switch (trait)
        {
            case TraitType.AddDmg: return "일반 데미지";
            case TraitType.AddCriDmg: return "일반 치명타 데미지";
            case TraitType.AddSDmg: return "스킬 데미지";
            case TraitType.AddSCriDmg: return "스킬 치명타 데미지";
        }
        return null;
    }

    public static string TranslateTraitString(this string trait)
    {
        switch (trait)
        {
            case "AddDmg": return "일반 데미지";
            case "AddCriDmg": return "일반 치명타 데미지";
            case "AddSDmg": return "스킬 데미지";
            case "AddSCriDmg": return "스킬 치명타 데미지";
        }
        return null;
    }

    public static int GetEvolveValue(MyPixelmonData myData, PixelmonData data)
    {
        switch (myData.star)
        {
            case 0:
                return DataManager.Instance.GetData<EvolveData>(data.rank).star1;
            case 1:    
                return DataManager.Instance.GetData<EvolveData>(data.rank).star2;
            case 2:    
                return DataManager.Instance.GetData<EvolveData>(data.rank).star3;
            case 3:    
                return DataManager.Instance.GetData<EvolveData>(data.rank).star4;
            case 4:    
                return DataManager.Instance.GetData<EvolveData>(data.rank).star5;
            default:
                return 0;
        }
    }
  
    public static int GetEvolveValue(MyAtvData myData, ActiveData data)
    {
        string rankTxt = string.Format("{0}Rank", data.rank);
        switch (myData.lv / 10)
        {
            case 0:
                return DataManager.Instance.GetData<EvolveData>(rankTxt).star1;
            case 1:
                return DataManager.Instance.GetData<EvolveData>(rankTxt).star2;
            case 2:
                return DataManager.Instance.GetData<EvolveData>(rankTxt).star3;
            case 3:
                return DataManager.Instance.GetData<EvolveData>(rankTxt).star4;
            case 4:
            default:
                return DataManager.Instance.GetData<EvolveData>(rankTxt).star5;
        }
    }

    // TextMeshProUGUI의 색상을 색상 코드로 변경하는 확장 메서드
    public static void HexColor(this TextMeshProUGUI textElement, string hexColor)
    {
        Color newColor;
        if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
        {
            textElement.color = newColor;
        }
        else
        {
            //Debug.LogError("색상 코드 변환에 실패했습니다: " + hexColor);
        }
    }

    public static float GetPsvWeight(string rcode)
    {
        switch (rcode)
        {
            case "공격력":
                return DataManager.Instance.GetData<BasePsvData>(rcode).weight;
            case "치명타 확률":
                return DataManager.Instance.GetData<BasePsvData>(rcode).weight;
            case "치명타 데미지":
                return DataManager.Instance.GetData<BasePsvData>(rcode).weight;
            case "공격 속도":
                return DataManager.Instance.GetData<BasePsvData>(rcode).weight;
            case "일반 데미지":
                return DataManager.Instance.GetData<BasePsvData>(rcode).weight;
            case "스킬 데미지":
                return DataManager.Instance.GetData<BasePsvData>(rcode).weight;
            case "스킬 치명타 확률":
                return DataManager.Instance.GetData<BasePsvData>(rcode).weight;
            case "스킬 치명타 데미지":
                return DataManager.Instance.GetData<BasePsvData>(rcode).weight;
            default:
                return 0;
        }
    }
}
