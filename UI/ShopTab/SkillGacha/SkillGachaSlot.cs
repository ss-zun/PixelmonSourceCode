using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SkillGachaSlot : MonoBehaviour
{
    public Image Bg;
    public Image Icon;
    public GameObject SRank;
    public GameObject SSRank;

    public void InitSlot(Sprite bg, Sprite icon, float duration)
    {
        Bg.sprite = bg;
        Icon.sprite = icon;
        transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360);
    }
}
