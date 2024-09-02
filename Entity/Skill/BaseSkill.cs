using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    public ActiveData data;
    public MyAtvData myData;
    public Pixelmon owner;
    public Enemy enemy;
    public GameObject target;
    public List<SoundData> soundData = new List<SoundData>();

    public void SetSound()
    {
        foreach (var sound in DataManager.Instance.soundData.data)
        {
            for (int i = 0; i < data.Soundrcode.Length; i++)
            {
                if (data.Soundrcode[i] == sound.rcode)
                    soundData.Add(sound);
            }
        }
    }

    public virtual void InitInfo(Pixelmon pxm, GameObject _target, MyAtvData myAtvData)
    {
        owner = pxm;       
        myData = myAtvData;
        target = _target;
        Setprojectile();
        ExecuteSkill();
    }

    protected virtual void ExecuteSkill() { }
    protected virtual void Setprojectile() { }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemy>(out enemy))
        {
            //Debug.Log(target);
            var result = owner.status.GetTotalDamage(owner.myData, true, data.maxRate + myData.lv);
            enemy.healthSystem.TakeDamage(result.Item1, result.Item2);
        }
    }

    protected virtual void CloseSkill()
    {
        gameObject.SetActive(false);
    }
}
