using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAutoEggHatch : UIBase
{
    [SerializeField] private TMP_Dropdown pxmRank;
    [SerializeField] private TMP_Dropdown psvRank;
    private EggHatch eggHatch;

    public override void Opened(object[] param)
    {
        eggHatch = param[0] as EggHatch;
    }

    public void OnClickStartBtn()
    {
        eggHatch.isAutoMode = true;
        eggHatch.StartAutoEggHatch((PxmRank)pxmRank.value, (PsvRank)psvRank.value);
        gameObject.SetActive(false);
    }
}