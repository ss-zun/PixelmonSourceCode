using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTab : UIBase
{
    [SerializeField] private SkillGacha skillGachaTab;

    private void OnEnable()
    {
        skillGachaTab.SetSkillGacha();
    }
}
