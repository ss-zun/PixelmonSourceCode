using Sirenix.OdinInspector;
using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class DamageText : SerializedMonoBehaviour
{
    Camera cam;
    Coroutine OnDamage;
    [SerializeField] Transform parent;
    [SerializeField] TextMeshProUGUI damageTxt;
    [SerializeField] RectTransform rectTr;
    [SerializeField] TMP_ColorGradient[] textGradients; // 0번 일반, 1번 크리, 2번 플레이어
    Color textColor = Color.white;
    string missTxt = "Miss!";
    private void Awake()
    {
        cam = Camera.main;
    }

    public void ShowDamageText(BigInteger damage, Vector3 pos, bool isCri = false, bool isPlayer = false)
    {
        if (OnDamage != null)
            StopCoroutine(ShowText(pos));

        if (damage > 0)
            damageTxt.text = Calculater.NumFormatter(damage);
                //string.Format("{0:#,###}", damage);
        else if (damage <= 0 && isPlayer)
        {
            gameObject.SetActive(false);
            return;
        }
        else
            damageTxt.text = missTxt;

        damageTxt.color = Color.white;
        damageTxt.colorGradientPreset = null;

        if (isPlayer)
            damageTxt.colorGradientPreset = textGradients[2];
        else 
        {
            if (isCri)
                damageTxt.colorGradientPreset = textGradients[1];
            else
                damageTxt.colorGradientPreset = textGradients[0];
        }
        OnDamage = StartCoroutine(ShowText(pos));
    }

    IEnumerator ShowText(Vector3 pos)
    {
        rectTr.anchoredPosition = RectTransformUtility.WorldToScreenPoint(cam, pos);
        yield return null;
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            rectTr.anchoredPosition = RectTransformUtility.WorldToScreenPoint(cam, Vector3.Lerp(pos, pos + Vector3.up* 5, time / 3));
            textColor.a = Mathf.Lerp(damageTxt.color.a, 0, time / 3);
            damageTxt.color = textColor;
            yield return null;
        }
        damageTxt.text = null;
        gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        if (OnDamage != null)
            StopCoroutine(OnDamage);
    }
}
