using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIGame : UIBase
{
    //public RawImage render;
    //public DamageIndicator damageIndicator;
    //public UICondition condition;
    //public PromptText promptText;
    //public List<QuickSlot> quickSlots;
    //public TMPro.TMP_Text nickname;
    //public TMPro.TMP_Text level;
    //public RawImage thumb;


    public override void Opened(object[] param)
    {
        base.Opened(param);
        //CameraContainer.instance.camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 0);
        //render.texture = CameraContainer.instance.camera.targetTexture;

        //nickname.text = UserInfo.myInfo.basicData.character_name;
        //level.text = $"lv.{UserInfo.myInfo.basicData.character_level}";
        //thumb.texture = await NetworkManager.instance.GetTexture(UserInfo.myInfo.basicData.character_image);
    }

    public override void Closed(object[] param)
    {
        base.Closed(param);
    }

    public void OnClickInventory()
    {
        //UIManager.Show<UIInventory>();
    }

    public void OnClickCraft()
    {
        //UIManager.Show<UICraft>();
    }
}
