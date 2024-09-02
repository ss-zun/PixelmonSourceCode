using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PixelmonLayout : MonoBehaviour
{
    public Image[] backgrounds;
    public Image[] thumbnailIcon;
    public Image[] stateIcon;
    public Image[] timer;
    public bool isMiddleBar;
    public bool isSkillTab;
    PixelmonManager pxmManager => PixelmonManager.Instance;
    SkillManager skillManager => SkillManager.Instance;

    private void Awake()
    {
        if (isMiddleBar)
        {
            pxmManager.layouts[0] = this;
            skillManager.layout = this;
        }
        else if (isSkillTab)
            pxmManager.layouts[1] = this;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.isInit);
        pxmManager.equipAction += InsertIcon;
        pxmManager.unEquipAction += DeleteIcon;
        InitPxmLayout();
    }
    private void InitPxmLayout()
    {
        UserData userdata = SaveManager.Instance.userData;
        for(int i = 0; i < backgrounds.Length; i++) 
        {
            if (!userdata.isLockedSlot[i])
            {
                if (userdata.equippedPxms[i].isEquipped)
                    InsertIcon(i, userdata.equippedPxms[i]);

                UnLockedIcon(i);
            }
        }
    }

    public void InsertIcon(int index, MyPixelmonData data)
    {
        stateIcon[index].gameObject.SetActive(false);
        backgrounds[index].sprite = pxmManager.FindPixelmonData(data.id).bgIcon;
        thumbnailIcon[index].gameObject.SetActive(true);
        thumbnailIcon[index].sprite = pxmManager.FindPixelmonData(data.id).icon;
    }

    public void DeleteIcon(int index)
    {
        backgrounds[index].sprite = pxmManager.defaultBg;
        thumbnailIcon[index].gameObject.SetActive(false);
        stateIcon[index].gameObject.SetActive(true);
    }
    public void UnLockedIcon(int index)
    {
        stateIcon[index].sprite = PixelmonManager.Instance.plusIcon;
    }
}
