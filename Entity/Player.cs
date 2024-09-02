using System.Collections;
using System.Linq;
using UnityEngine;

public class Player : Singleton<Player>
{
    public PlayerFSM fsm;
    public PlayerStatHandler statHandler;
    public PlayerHealthSystem healthSystem;
    public GameObject HitPosition;

    [Header("LocatedPixelmon")]
    public float radius = 2.0f;
    public Pixelmon[] pixelmons = new Pixelmon[5];

    private IEnumerator Start()
    {
        while(SaveManager.Instance.userData.tutoIndex < 4) yield return null;
        fsm.Init();
    }

    public void ChangePixelmonsState(PixelmonState newState)
    {
        for(int i = 0;  i < pixelmons.Length; i++)
        {
            if (pixelmons[i] == null) continue;
            switch (newState)
            {
                case PixelmonState.Attack:
                    pixelmons[i].fsm.ChangeState(pixelmons[i].fsm.AttackState);
                    break;
                case PixelmonState.Idle:
                    pixelmons[i].fsm.ChangeState(pixelmons[i].fsm.IdleState);
                    break;
                case PixelmonState.Move:
                    pixelmons[i].fsm.ChangeState(pixelmons[i].fsm.MoveState);
                    break;
            }
        }
    }

    public void SetPixelmonsTarget(GameObject target)
    {
        for(int i = 0;i < pixelmons.Length; i++)
        {
            if (pixelmons[i] == null) continue;
            pixelmons[i].fsm.target = target;
        }
    }

    public void LocatedPixelmon()
    {
        var pxmList = pixelmons.ToList().FindAll((obj) => obj != null);
        if(pxmList.Count == 0) return;
        int angle = 360 / pxmList.Count;
        int currentAngle = -90;

        switch (pxmList.Count)
        {
            case 2:
                currentAngle = 0;
                break;
            case 4:
                currentAngle = 45;
                break;
            default:
                break;
        }       

        
        for (int i = 0; i < pxmList.Count; i++)
        {
            if (pxmList[i] == null) continue;
            Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius - 0.1f, 0);
            pxmList[i].transform.position = transform.position + pos;
            currentAngle += angle;
        }
    }
}