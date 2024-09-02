using System;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : UIBase
{
    [SerializeField] private Slider bgmSldr;
    [SerializeField] private Slider sfxSldr;
    
    private readonly string email = "dodugbab31@gmail.com";

    private void Start()
    {
        bgmSldr.onValueChanged.AddListener(SetBgmVolume);
        sfxSldr.onValueChanged.AddListener(SetSfxVolume);

        bgmSldr.value = SaveManager.Instance.userData.BGMVolume;
        sfxSldr.value = SaveManager.Instance.userData.SFXVolume;

        SetBgmVolume(bgmSldr.value);
        SetSfxVolume(sfxSldr.value);
    }

    public override void HideDirect()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20002").clip);
        UIManager.Hide<UISettings>();
    }

    public void SetBgmVolume(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
        SaveManager.Instance.SetFieldData(nameof(SaveManager.Instance.userData.BGMVolume), value);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        SaveManager.Instance.SetFieldData(nameof(SaveManager.Instance.userData.SFXVolume), value);
    }

    public void SupportBtn()
    {
        AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20002").clip);
        string emailUrl = $"mailto:{email}";
        Application.OpenURL(emailUrl);
    }
}
