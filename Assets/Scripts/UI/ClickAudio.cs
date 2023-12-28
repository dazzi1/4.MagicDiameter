using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* @description:µã»÷ÒôÐ§
* @author: dazzi
* @time: 2023.07
*/
public class ClickAudio : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private ManagerVars vars;
    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.PlayClikAudio, PlayAudio);
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.PlayClikAudio, PlayAudio);
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }
    private void PlayAudio() {
        m_AudioSource.PlayOneShot(vars.buttonClip);
    }
    private void IsMusicOn(bool value) {
        m_AudioSource.mute = !value;
    }
}
