using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarnPopup : UIBase
{
    UIManager uiManager;

    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI warnTxt;


    WaitForSeconds wait = new WaitForSeconds(1f);
    Coroutine myCoroutine;

    protected override void Awake()
    {
        base.Awake();
        uiManager = UIManager.Instance;
    }

    public override void Opened(object[] param)
    {
        if (myCoroutine != null)
        {
            return;
        }
        warnTxt.text = param[0].ToString();
        myCoroutine = StartCoroutine(ShowWarning());
    }

    public IEnumerator ShowWarning()
    {
        yield return wait;

        Sequence seq = DOTween.Sequence();
        seq.Append(img.DOFade(0f, 1f));
        seq.Join(warnTxt.DOFade(0f, 1f));

        yield return wait;

        UIManager.Hide<WarnPopup>(false);
    }
}
