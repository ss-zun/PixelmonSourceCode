using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;

public class UIDgResultPopup : UIBase
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI rwdTxt;

    private float duration = 0.9f;

    public override void Opened(object[] param)
    {
        canvasGroup.alpha = 1;
        if (BigInteger.TryParse(param[0].ToString(), out BigInteger rwd))
        {
            rwdTxt.text = Calculater.NumFormatter(rwd);
        }
        else
        {
            rwdTxt.text = param[0].ToString();
        }
        
        StartCoroutine(FadeRwdPanel());
    }

    private IEnumerator FadeRwdPanel()
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;
        yield return new WaitForSeconds(1f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        UIManager.Hide<UIDgResultPopup>();
    }
}