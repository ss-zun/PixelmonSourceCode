using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DailyManager : MonoBehaviour
{
    private Coroutine chargeTimeCoroutine;
    private SaveManager saveManager => SaveManager.Instance;
    private UserData userData;
    // Start is called before the first frame update
    void Start()
    {
        userData = saveManager.userData;
        if (chargeTimeCoroutine != null)
            StopCoroutine(chargeTimeCoroutine);
        chargeTimeCoroutine = StartCoroutine(UpdateChargeTime());
    }

    private IEnumerator UpdateChargeTime()
    {
        DateTime midnight = DateTime.UtcNow.AddDays(1);
        string lastTime = userData.lastConnectTime;
        if (DateTime.TryParse(lastTime, out DateTime date))
        {
            if (date.Date < DateTime.UtcNow)
            {
                //하루 뒤.
                ResetKey();
                ResetAds();
            }
        }
        saveManager.SetFieldData(nameof(userData.lastConnectTime), DateTime.UtcNow.Date.ToString());
        while (true)
        {
            if (DateTime.UtcNow.Date >= midnight)
            {
                ResetKey();
                ResetAds();
                midnight = midnight.AddDays(1);
                saveManager.SetFieldData(nameof(userData.lastConnectTime), DateTime.UtcNow.ToString());
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void ResetAds()
    {
        userData.adsCount = 24;
        saveManager.SetFieldData(nameof(userData.adsCount), 24);
    }

    private void ResetKey()
    {
        saveManager.SetFieldData(nameof(userData.key0), 3);
        saveManager.SetFieldData(nameof(userData.key1), 3);
        saveManager.SetFieldData(nameof(userData.key2), 3);
    }
}
