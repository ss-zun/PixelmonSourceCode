using TMPro;
using UnityEngine;

public class UIUnlockPopup : UIBase
{
    [SerializeField] private TextMeshProUGUI unlockMsg;

    public override void Opened(object[] param)
    {
        unlockMsg.text = param[0].ToString();
    }

    public void HideUnlockPopup()
    {
        UIManager.Hide<UIUnlockPopup>();
    }
}

