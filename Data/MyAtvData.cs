using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MyAtvData
{
    public string rcode;
    public int id = -1;
    public int lv = 1;
    public int maxExp = 10;
    public bool isAdvancable;
    //같은 카드 중복개수
    public int evolvedCount = 0;

    //장착한 픽셀몬 ID
    public int pxmId = -1;
    //장착 여부
    public bool isEquipped;
    //등록 여부
    public bool isAttached;
    //보유 여부
    public bool isOwned;

    public void UpdateField(string fieldName, object value)
    {
        var fieldInfo = GetType().GetField(fieldName);
        if (fieldInfo != null)
        {
            fieldInfo.SetValue(this, value);
        }
        else
        {
            //Debug.LogWarning($"{fieldName}라는 변수를 MyPixelmonData에서 찾을 수 없습니다.");
        }
    }
}
