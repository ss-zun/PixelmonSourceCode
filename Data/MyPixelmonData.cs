using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MyPixelmonData
{
    //도감 넘버
    public string rcode;
    public int id;
    public int lv = 1;
    public int maxExp = 10;
    public int star = 0;
    //진화 가능 여부
    public bool isAdvancable;
    //같은 카드 중복개수
    public int evolvedCount = 0;
    
    //장착 여부
    public bool isEquipped;
    //보유 여부
    public bool isOwned;

    public int atvSkillId = -1;
    //공격수치
    public float atkValue;
    //패시브 능력
    public List<PsvSkill> psvSkill = new List<PsvSkill>();
    //보유효과
    public float[] ownEffectValue = new float[2];

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

    public float FindType(AbilityType type, bool isSpeed = false)
    {
        var myType = psvSkill.Find((obj) => obj.psvType == type);
        if (myType == null)
        {
            return isSpeed ? 0 : 1;
        }
        return myType.psvValue;
    }
}

[Serializable]
public class PsvSkill
{
    public AbilityType psvType;
    public string psvRank;
    public string psvName;
    public float psvValue;
}
