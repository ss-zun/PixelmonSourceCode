using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIHatchWarnPopup : UIBase
{
    private EggHatch eggHatch;
    public void SetPopup(EggHatch eggHatch)
    {
        this.eggHatch = eggHatch;
    }

    public void OnClickNoBtn()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20010").clip);
        gameObject.SetActive(false);
    }

    public void OnClickYesBtn()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20010").clip);
        eggHatch.GetPixelmon(true);
        gameObject.SetActive(false);
    }
}
