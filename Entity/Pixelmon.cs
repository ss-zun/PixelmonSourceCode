using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pixelmon : MonoBehaviour
{
    public PixelmonData data;
    public MyPixelmonData myData;
    public PixelmonFSM fsm;
    public PixelmonStatus status = new PixelmonStatus();
    public SpriteRenderer spriteRenderer;
    private void Start()
    {
        fsm.InitStates();
    }
    private void Update()
    {
        fsm.Update();
    }

    public void InitPxm()
    {
        status.InitStatus(data, myData);
        fsm.ChangeState(Player.Instance.fsm.currentState);
        fsm.attackSpeed = status.AtkSpd;
    }
}
