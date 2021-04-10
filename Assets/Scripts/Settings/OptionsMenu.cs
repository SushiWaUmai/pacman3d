using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private const string masterVolume = "MasterVolume";
    private const string musicVolume = "MusicVolume";
    private const string sfxVolume = "SfxVolume";

    private void ChangeVolume(string parameter, float val) => audioMixer.SetFloat(parameter, Mathf.Log10(val) * 20);

    public void ChangeMasterVolume(float val) => ChangeVolume(masterVolume, val);
    public void ChangeMusicVolume(float val) => ChangeVolume(musicVolume, val);
    public void ChangeSFXVolume(float val) => ChangeVolume(sfxVolume, val);
}
