using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInvoker : MonoBehaviour
{
    [SerializeField] private Image image;
    public WaitForSeconds waitFadeTime = new WaitForSeconds(0.5f);
    private bool isUsing;

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        if (!isUsing)
        {
            isUsing = true;
            image.color = new Color(0, 0, 0, 0);
            yield return image.DOFade(1f, 0.5f).WaitForCompletion();
            isUsing = false;
        }
    }

    public IEnumerator FadeOut(WaitForSeconds time = null)
    {
        if (!isUsing)
        {
            isUsing = true;

            if (time == null)
                time = waitFadeTime;

            image.color = Color.black;
            yield return time;
            yield return image.DOFade(0f, 0.5f).WaitForCompletion();

            isUsing = false;
            gameObject.SetActive(false);
        }
    }
}
