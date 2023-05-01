using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDSystem : MonoBehaviour
{
    public GameObject hudSystemObj;
    FighterSystem fighter;
    WeaponsSystem weaponSystem;
    FighterSound fighterSound;
    AudioSource hudAudioSource;
    AudioSource lockedSource;
    AudioSource voiceSource;

    [Header("HUD信息")]
    [SerializeField] Text speedNum;
    [SerializeField] Text altNum;
    [SerializeField] Text yawNum;

    [Header("HUD运动轴")]
    [SerializeField] public RectTransform pitchLaddor;
    [SerializeField] RectTransform rollLever;
    [SerializeField] RectTransform pitchLever;
    [SerializeField] RectTransform yawLever;
    [SerializeField] public RectTransform compass;
    float yawScaleAngle;
    float pitchScaleAngle;

    [Header("锁定框")]
    [SerializeField] public RectTransform lockIndicator;
    [SerializeField] public Image lockedImg;

    [Header("准星")]
    [SerializeField] public RectTransform crosshair;

    [Header("边框")]
    [SerializeField] public RectTransform hudBox;

    [Header("锁定提示")] public Text lockedMsg;

    [Header("敌机指针")] public Transform enemyPosIndicator;
    Transform lockedTarget;
    public Text targetName;
    public Text targetDis;
    public Sprite[] lockedSprite;
    float LockOnTimer;
    float colorAlpha = 1;
    float targetDistance;

    enum LockState { LockedOff, LockedIn, LockedOn }
    enum AlphaState { FadeIn, FadeOut, Hold }
    LockState lockState;
    AlphaState alphaState;

    public void HudUpdate()
    {
        if (weaponSystem.angle >= 21f)
        {
            lockIndicator.GetComponent<RectMask2D>().enabled = true;
        }
        else
        {
            lockIndicator.GetComponent<RectMask2D>().enabled = false;
        }
        SynchronizeHUDInformation();
        CompassControl();
        PitchScaleControl();
        if (weaponSystem.targetArr.Count == 0)
        {
            lockIndicator.gameObject.SetActive(false);
            enemyPosIndicator.gameObject.SetActive(false);
            return;
        }
        LockIndicatorInfoUpdate();
        LockIndicatorStateUpdate();
        EnemyPosIndicatorUpdate();
        OnLockedPrompt();
    }
    public void InitBaseComponent()
    {
        fighter = GetComponent<FighterSystem>();
        weaponSystem = GetComponent<WeaponsSystem>();
        if (!rollLever) Debug.LogError("未能获取到滚筒控制");
        if (!pitchLever) Debug.LogError("未能获取到俯仰控制");
        if (!yawLever) Debug.LogError("未能获取到偏航控制");
        rollLever.localRotation = Quaternion.identity;
        pitchLever.localRotation = Quaternion.identity;
        yawLever.localRotation = Quaternion.identity;
        lockState = LockState.LockedOff;
        fighterSound = fighter.gameObject.GetComponent<FighterSound>();
        hudAudioSource = fighterSound.hudSource;
        lockedSource = fighterSound.lockedSource;
        voiceSource = fighterSound.cockpitLockedVoice;
        if (weaponSystem.target) targetName.text = weaponSystem.target.name;
        lockedImg.sprite = lockedSprite[0];
        lockIndicator.GetComponent<RectMask2D>().enabled = false;
    }
    void SynchronizeHUDInformation()
    {
        speedNum.text = (GetComponent<Rigidbody>().velocity.magnitude * 3.69f).ToString("0");
        altNum.text = transform.position.y.ToString("0");
        yawNum.text = transform.localEulerAngles.y.ToString("0");
    }
    void CompassControl()
    {
        yawScaleAngle = transform.localEulerAngles.y;
        if (Vector3.Cross(Vector3.forward, transform.forward).y < 0) yawScaleAngle = 360 - yawScaleAngle;
        Vector2 _yawVal = yawLever.anchoredPosition;
        _yawVal.x = (1154f / 90f) * yawScaleAngle; //(移动的像素/角度) 这里1154图片宽度，一张图是一个方向所以1154除以90°
        yawLever.anchoredPosition = _yawVal;
    }
    void PitchScaleControl()
    {
        rollLever.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z * -1);
        pitchScaleAngle = transform.localEulerAngles.x;
        if (transform.localEulerAngles.x >= 270) pitchScaleAngle -= 360f;
        Vector2 _pitchVal = pitchLever.anchoredPosition;
        _pitchVal.y = (50f / 5f) * pitchScaleAngle;
        pitchLever.anchoredPosition = _pitchVal;
    }

    void LockIndicatorInfoUpdate()      //更新锁定框上的信息
    {
        if (!weaponSystem.target) return;
        targetDistance = Vector3.Distance(fighter.gameObject.transform.position, weaponSystem.target.position);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(weaponSystem.target.position);
        lockIndicator.position = screenPos;
        targetDis.text = targetDistance.ToString("0");
        targetName.text = weaponSystem.target.name;
    }

    void LockIndicatorStateUpdate()     //更新锁定框的状态
    {
        if (!weaponSystem.target)
        {
            return;
        }
        if (!weaponSystem.SectorDetection(weaponSystem.target))
        {
            weaponSystem.SetLockedMsg(weaponSystem.target, false);
            lockState = LockState.LockedOff;
        }
        if (weaponSystem.SectorDetection(weaponSystem.target) && lockState != LockState.LockedOn)
        {
            lockState = LockState.LockedIn;
        }

        switch (lockState)
        {
            case LockState.LockedOff:
                if (hudAudioSource.isPlaying == true) hudAudioSource.Stop();
                weaponSystem.lockedTarget = null;
                alphaState = AlphaState.Hold;
                lockedImg.sprite = lockedSprite[0];
                LockOnTimer = 0f;
                break;
            case LockState.LockedIn:
                hudAudioSource.clip = fighterSound.hudLockedSFX[0];
                if (hudAudioSource.isPlaying == false) hudAudioSource.Play();

                weaponSystem.lockedTarget = null;
                lockedImg.sprite = lockedSprite[0];
                if (alphaState == AlphaState.Hold) alphaState = AlphaState.FadeIn;
                LockOnTimer += Time.deltaTime;
                weaponSystem.SetLockedMsg(weaponSystem.target, true);
                if (LockOnTimer > 3) lockState = LockState.LockedOn;
                break;
            case LockState.LockedOn:
                hudAudioSource.clip = fighterSound.hudLockedSFX[1];
                if (hudAudioSource.isPlaying == false) hudAudioSource.Play();

                lockedImg.sprite = lockedSprite[1];
                alphaState = AlphaState.Hold;
                lockedTarget = weaponSystem.target;
                weaponSystem.lockedTarget = lockedTarget;
                break;
            default:
                break;
        }
        LockBoxUpdate();
    }
    void LockBoxUpdate()            //随锁定框状态改变视觉效果
    {
        switch (alphaState)
        {
            case AlphaState.FadeIn:
                colorAlpha += Time.deltaTime * 4f;
                if (colorAlpha >= 1) alphaState = AlphaState.FadeOut;
                break;
            case AlphaState.FadeOut:
                colorAlpha -= Time.deltaTime * 4f;
                if (colorAlpha <= 0) alphaState = AlphaState.FadeIn;
                break;
            case AlphaState.Hold:
                colorAlpha = 1;
                break;
            default:
                break;
        }
        lockedImg.color = new Color(1, 1, 1, colorAlpha);
    }

    public void EnemyPosIndicatorUpdate()
    {
        if (weaponSystem.currentTarget)
        {
            enemyPosIndicator.gameObject.SetActive(true);
            enemyPosIndicator.LookAt(weaponSystem.currentTarget);
        }
        else
        {
            enemyPosIndicator.gameObject.SetActive(false);
        }
    }

    public void OnLockedPrompt()
    {
        if (weaponSystem.isEnemyleLocked && !weaponSystem.isMissileLocked) 
        {
            lockedMsg.text = "LOCKED";
            lockedSource.clip = fighterSound.onLockedSFX;
            if (!lockedSource.isPlaying) lockedSource.Play();
            voiceSource.clip = fighterSound.warningVoiceSFX;
            if (!voiceSource.isPlaying) voiceSource.Play();
        }
        else if (weaponSystem.isMissileLocked)
        {
            lockedMsg.text = "MISSILE";
            lockedSource.clip = fighterSound.missileComingSFX;
            if (!lockedSource.isPlaying) lockedSource.Play();
            voiceSource.clip = fighterSound.missileVoiceSFX;
            if (!voiceSource.isPlaying) voiceSource.Play();
        }
        if (!weaponSystem.isEnemyleLocked && !weaponSystem.isMissileLocked)
        {
            lockedMsg.text = "";
            if (lockedSource.isPlaying) lockedSource.Stop();
            if (voiceSource.isPlaying) voiceSource.Stop();
        }
    }
}