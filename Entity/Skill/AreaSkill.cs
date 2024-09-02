using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSkill : BaseSkill
{
    protected override void ExecuteSkill()
    {
        base.ExecuteSkill();
        AudioManager.Instance.PlayClip(soundData[Random.Range(0, soundData.Count)].clip);
        gameObject.transform.position = target.transform.position;
    }
}
