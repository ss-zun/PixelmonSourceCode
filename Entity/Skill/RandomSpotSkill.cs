using System.Collections;
using UnityEngine;

public class RandomSpotSkill : BaseSkill
{
    public PrefabSkill[] skillobj;
    float waitTime = 0.5f;
    WaitForSeconds spawnTime;
    WaitForSeconds duration;

    public override void InitInfo(Pixelmon pxm, GameObject target, MyAtvData myAtvData)
    {
        base.InitInfo(pxm, target, myAtvData);
        if (spawnTime == null)
        {
            spawnTime = new WaitForSeconds(waitTime);
            duration = new WaitForSeconds(2f);
        }
    }

    protected override void ExecuteSkill()
    {
        base.ExecuteSkill();
        transform.position = target.transform.position;
        StartCoroutine(SkillSpawner());
    }

    IEnumerator SkillSpawner()
    {
        for (int i = 0; i < skillobj.Length; i++)
        {
            skillobj[i].gameObject.SetActive(true);
            skillobj[i].SetSkill(this, soundData);
            yield return spawnTime;
        }
        yield return duration;
        CloseSkill();
    }
    
    protected override void CloseSkill()
    {
        for (int i = 0; i < skillobj.Length; i++)
        {
            skillobj[i].gameObject.SetActive(false);
        }
        base.CloseSkill();
    }
}
