using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillGachaBtn : MonoBehaviour
{
    public Button Btn;
    public GameObject Ticket;
    public GameObject Dia;

    public void SetTicket()
    {
        Dia.SetActive(false);
        Ticket.SetActive(true);
    }

    public void SetDia()
    {
        Ticket.SetActive(false);
        Dia.SetActive(true);
    }
}
