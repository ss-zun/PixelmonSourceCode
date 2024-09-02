using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGO : MonoBehaviour
{
    public void OnAnimEnd()
    {
        gameObject.SetActive(false);
    }
}
