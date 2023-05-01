using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour
{
    [HideInInspector] public enum controllerType { Player, AI }
    [HideInInspector] public controllerType Type;
    Transform originPoint;
    public enum AIstates
    {
        directionCorrection,    //方向修正
        circling,       //盘旋
        lockEvading,    //规避锁定
        missleEvading,  //规避导弹
        trackingTarget, //追击目标
        attack          //攻击
    }
    public AIstates aiStates;

    FighterSystem fighterSystem;
    WeaponsSystem weaponsSystem;
    HUDSystem hudSystem;

    float circlingTimer;
    float lockInTimer;
    float lockOnTimer;

    float heightLimit;
    float minimumLimit;
    float regionalLimit;

    bool aiCanFire = true;

    private void Awake()
    {
        originPoint = GameManager.GMins.originPoint;
        heightLimit = GameManager.heightLimit;
        minimumLimit = GameManager.minimumLimit;
        regionalLimit = GameManager.regionalLimit;
        switch (Type)
        {
            case controllerType.Player:
                InitFighterControllerPlayer();
                break;
            case controllerType.AI:
                InitFighterControllerAI();
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        lockInTimer += Time.deltaTime;
        lockOnTimer += Time.deltaTime;
        switch (Type)
        {
            case controllerType.Player:
                UpdateFighterControllerPlayer();
                break;
            case controllerType.AI:
                UpdateFighterControllerAI();
                break;
            default:
                break;
        }
    }
    public void InitFighterControllerPlayer()
    {
        fighterSystem = GetComponent<FighterSystem>();
        weaponsSystem = GetComponent<WeaponsSystem>();
        hudSystem = GetComponent<HUDSystem>();
        if (!fighterSystem) { Debug.LogWarning("获取飞行系统失败"); return; }
        if (!weaponsSystem) { Debug.LogWarning("获取武器系统失败"); return; }
        if (!hudSystem) { Debug.LogWarning("获取HUD系统失败"); return; }
        fighterSystem.InitBaseComponent();
        weaponsSystem.InitBaseComponent();
        hudSystem.InitBaseComponent();
    }

    public void InitFighterControllerAI()
    {
        fighterSystem = GetComponent<FighterSystem>();
        weaponsSystem = GetComponent<WeaponsSystem>();
        hudSystem = GetComponent<HUDSystem>();
        if (!fighterSystem) { Debug.LogWarning("获取飞行系统失败"); return; }
        if (!weaponsSystem) { Debug.LogWarning("获取武器系统失败"); return; }
        if (!hudSystem) { Debug.LogWarning("获取HUD系统失败"); return; }
        fighterSystem.InitBaseComponent();
        weaponsSystem.InitBaseComponent();
        hudSystem.InitBaseComponent();
        hudSystem.hudSystemObj.SetActive(false);
    }

    bool DirectionCorrection(float minimumLimit, float heightLimit, float regionalLimit)
    {
        var isIn = false;
        if (transform.position.y < minimumLimit)
        {
            isIn = true;
            TrackingTarget(originPoint);
        }
        if (transform.position.y > heightLimit)
        {
            isIn = true;
            TrackingTarget(originPoint);
        }
        if (transform.position.x < -regionalLimit)
        {
            isIn = true;
            TrackingTarget(originPoint);
        }
        if (transform.position.x > regionalLimit)
        {
            isIn = true;
            TrackingTarget(originPoint);
        }
        if (transform.position.z < -regionalLimit)
        {
            isIn = true;
            TrackingTarget(originPoint);
        }
        if (transform.position.z > regionalLimit)
        {
            isIn = true;
            TrackingTarget(originPoint);
        }
        return isIn;
    }

    public void UpdateFighterControllerAI()
    {
        var num = 0;
        weaponsSystem.FindClosestEnemy();
        if (DirectionCorrection(minimumLimit, heightLimit, regionalLimit)) aiStates = AIstates.directionCorrection;
        switch (aiStates)
        {
            case AIstates.directionCorrection:
                if (!DirectionCorrection(minimumLimit, heightLimit, regionalLimit)) aiStates = AIstates.circling;
                break;
            case AIstates.circling:
                circlingTimer += Time.deltaTime;
                num = Random.Range(1, 3);
                if (weaponsSystem.isEnemyleLocked) aiStates = AIstates.lockEvading;
                if (weaponsSystem.isMissileLocked) aiStates = AIstates.missleEvading;
                if (weaponsSystem.target && circlingTimer >= 10)
                {
                    if (!aiCanFire) aiCanFire = true;
                    aiStates = AIstates.trackingTarget;
                }
                break;
            case AIstates.lockEvading:
                var pith = fighterSystem.GetComponent<FighterParameter>().pitchPerformance * -1f * Time.deltaTime ;
                var roll = fighterSystem.GetComponent<FighterParameter>().pitchPerformance * -1f * Time.deltaTime ;
                var yaw = fighterSystem.GetComponent<FighterParameter>().pitchPerformance * -1f * Time.deltaTime ;
                //fighterSystem.AttitudeControl(0, 0, 1);
                if (weaponsSystem.target)
                {
                    TrackingTarget(weaponsSystem.target);
                }
                fighterSystem.Slowdown();
                if (weaponsSystem.isMissileLocked) aiStates = AIstates.missleEvading;
                if (GameManager.GMins.fighter && GameManager.GMins.fighter.GetComponent<WeaponsSystem>().lockedTarget != transform)
                {
                    aiStates = AIstates.circling;
                }
                break;
            case AIstates.missleEvading:
                if (weaponsSystem.target)
                {
                    TrackingTarget(weaponsSystem.target);
                }
                fighterSystem.Accelerator();
                if (!weaponsSystem.isMissileLocked) aiStates = AIstates.circling;
                break;
            case AIstates.trackingTarget:
                if (weaponsSystem.target)
                {
                    TrackingTarget(weaponsSystem.target);
                    if (Vector3.Distance(transform.position, weaponsSystem.target.position) >= 1500)
                    {
                        fighterSystem.Accelerator();
                    }
                    else if (Vector3.Distance(transform.position, weaponsSystem.target.position) <= 550)
                    {
                        fighterSystem.Slowdown();
                    }
                    else
                    {
                        fighterSystem.engineOutput = weaponsSystem.target.GetComponent<FighterSystem>().engineOutput * 1.2f;
                    }
                }
                if (weaponsSystem.SectorDetection(weaponsSystem.target))
                {
                    weaponsSystem.SetLockedMsg(weaponsSystem.target, true);
                    if (lockInTimer >= 5) aiStates = AIstates.attack;
                }
                else
                {
                    weaponsSystem.SetLockedMsg(weaponsSystem.target, false);
                    lockInTimer = 0;
                }
                if (GameManager.GMins.fighter && GameManager.GMins.fighter.GetComponent<WeaponsSystem>().lockedTarget == transform)
                {
                    aiStates = AIstates.lockEvading;
                }
                break;
            case AIstates.attack:
                if (weaponsSystem.target)
                {
                    TrackingTarget(weaponsSystem.target);
                    if (aiCanFire)
                    {
                        weaponsSystem.SetMissileMsg(weaponsSystem.target, true);
                        weaponsSystem.MissileLaunch(weaponsSystem.MisslesLaunchPoint[0], weaponsSystem.target);
                        aiCanFire = false;
                        circlingTimer = 0;
                        aiStates = AIstates.circling;
                    }
                }
                break;
            default:
                break;
        }
        fighterSystem.EngineControl();
    }
    public void UpdateFighterControllerPlayer()
    {
        fighterSystem.InputControl();
        fighterSystem.AttitudeControl(fighterSystem.inputPitch, fighterSystem.inputYaw, fighterSystem.inputRoll);
        fighterSystem.EngineControl();
        weaponsSystem.FindClosestEnemy();
        if (GetComponent<InputManager>().MainWeaponFire())
        {
            weaponsSystem.MissileLaunch(weaponsSystem.MisslesLaunchPoint[weaponsSystem.switchLaunchPoint()], weaponsSystem.lockedTarget);
            weaponsSystem.SetMissileMsg(weaponsSystem.lockedTarget, true);
        }
        hudSystem.HudUpdate();
    }

    void TrackingTarget(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        float dotF_B = Vector3.Dot(transform.up, dir.normalized);
        if (dotF_B != 0)
        {
            transform.Rotate(new Vector3(-dotF_B, 0, 0));
        }
        float dotL_R = Vector3.Dot(transform.right, dir.normalized);
        if (dotL_R != 0)
        {
            transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + Mathf.Clamp(-dotL_R, fighterSystem.rollPerformance * Time.deltaTime * 0.5f, fighterSystem.rollPerformance * Time.deltaTime));
        }
    }
}