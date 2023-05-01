using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSound : MonoBehaviour
{
    public GameObject engine;
    public GameObject cockpit;
    public GameObject cockpitLockedMsg;
    public GameObject cockpitVoice;

    [HideInInspector] public AudioSource engineSource;
    [HideInInspector] public AudioSource hudSource;
    [HideInInspector] public AudioSource lockedSource;
    [HideInInspector] public AudioSource cockpitLockedVoice;

    public AudioClip fighterEngineSFX;
    public AudioClip[] hudLockedSFX;
    public AudioClip onLockedSFX;
    public AudioClip missileComingSFX;
    public AudioClip warningVoiceSFX;
    public AudioClip missileVoiceSFX;

    public void InitBaseComponent()
    {
        engineSource = engine.GetComponent<AudioSource>();
        hudSource = cockpit.GetComponent<AudioSource>();
        lockedSource = cockpitLockedMsg.GetComponent<AudioSource>();
        cockpitLockedVoice = cockpitVoice.GetComponent<AudioSource>();
    }
}
