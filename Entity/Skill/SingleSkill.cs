using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSkill : BaseSkill
{
    public Rigidbody2D rbody;
    public Vector2 movePos;
    public float speed = 1f;

    public override void InitInfo(Pixelmon pxm, GameObject target, MyAtvData myAtvData)
    {
        base.InitInfo(pxm, target, myAtvData);
    }
    
    protected override void Setprojectile()
    {
        base.Setprojectile();
        
        Vector2 newPos = target.transform.position - transform.position;
        float rotZ = Mathf.Atan2(newPos.y, newPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
        movePos = transform.up;
    }

    protected override void ExecuteSkill()
    {
        base.ExecuteSkill();
        transform.position = owner.transform.position;
        rbody.velocity = movePos * speed;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemy>(out enemy))
        {
            //TODO : 애니메이션 상태 변경
            base.OnTriggerEnter2D(collision);
        }

    }

    protected override void CloseSkill()
    {
        base.CloseSkill();
        rbody.velocity = Vector2.zero;
    }
}
