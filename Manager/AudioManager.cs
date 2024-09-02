using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BgmIndex
{
    Intro,
    Main
}

public class AudioManager : Singleton<AudioManager>
{
    public AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [SerializeField] private AudioSource[] bgmAudioSource;
    [SerializeField] private AudioClip[] bgmClip;

    [SerializeField] private Queue<AudioSource> sfxAudioSource;
    
    public bool[] isMuted = new bool[3];
    private float[] preVolumes = new float[3];

    private void Start()
    {
        SetBGMVolume(SaveManager.Instance.userData.BGMVolume);
        SetSFXVolume(SaveManager.Instance.userData.SFXVolume);
    }

    public void ChangeBackGroundMusic(BgmIndex clip)
    {
        if (bgmAudioSource[0].isPlaying)
        {
            SetBgAudio(clip, 0, 1);
        }
        else if (bgmAudioSource[1].isPlaying)
        {
            SetBgAudio(clip, 1, 0);
        }
        else
        {
            bgmAudioSource[0].DOComplete();
            bgmAudioSource[0].volume = 0;
            SetBgAudio(clip, 1, 0);
        }
    }

    private void SetBgAudio(BgmIndex clip, int stopIndex, int playIndex)
    {
        bgmAudioSource[playIndex].clip = bgmClip[(int)clip];
        bgmAudioSource[playIndex].Play();
        bgmAudioSource[playIndex].DOFade(0.5f, 2);
        bgmAudioSource[stopIndex].DOFade(0, 2).OnComplete(() => bgmAudioSource[stopIndex].Stop());
    }

    public void PlayClip(AudioClip clip)
    {
        AudioSource audio = sfxAudioSource.Dequeue();
        sfxAudioSource.Enqueue(audio);
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(clip);
        }
        else
        {
            audio.Stop();
            audio.PlayOneShot(clip);
        }
    }

    public void SetBGMVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("BGMVolume", dB);
    }

    public void SetSFXVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
    }

    //public void ToggleMute(int index)
    //{
    //    string audioName;

    //    switch (index)
    //    {
    //        case 1:
    //            audioName = "BGMVolume";
    //            break;
    //        case 2:
    //            audioName = "SFXVolume";
    //            break;
    //        default:
    //            audioName = "MasterVolume";
    //            break;
    //    }

    //    isMuted[index] = !isMuted[index];

    //    if (isMuted[index])
    //    {
    //        audioMixer.GetFloat(audioName, out preVolumes[index]);
    //        audioMixer.SetFloat(audioName, -80);
    //    }
    //    else
    //    {
    //        audioMixer.SetFloat(audioName, preVolumes[index]);
    //    }
    //}
}