using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsSystem : MonoBehaviour
{
    enum LaunchPoint { main, left, right }
    LaunchPoint launchPoint;
    public Transform target;//目标
    public Transform currentTarget;
    public Transform lockedTarget;//当前锁定的目标
    public List<GameObject> targetArr;
    public bool isEnemyleLocked;
    public bool isMissileLocked;
    public Transform[] MisslesLaunchPoint;
    InputManager InputMg;
    FighterParameter fighterParameter;
    public float angle;

    public int switchLaunchPoint()
    {
        var pointNum = 0;
        switch (launchPoint)
        {
            case LaunchPoint.main:
                break;
            case LaunchPoint.left:
                pointNum = 2;
                launchPoint = LaunchPoint.right;
                break;
            case LaunchPoint.right:
                pointNum = 1;
                launchPoint = LaunchPoint.left;
                break;
            default:
                break;
        }
        return pointNum;
    }

    public void InitBaseComponent()
    {
        InputMg = GetComponent<InputManager>();
        fighterParameter = GetComponent<FighterParameter>();
        launchPoint = LaunchPoint.left;
    }

    public void MissileLaunch(Transform LaunchPoint,Transform lockedTarget)
    {
        var missile = Resources.Load<GameObject>("Prefabs/Weapons/Missile");
        if (!missile) { Debug.LogWarning("导弹没有加载成功"); return; }
        missile = Instantiate<GameObject>(missile, LaunchPoint.position, LaunchPoint.rotation);
        missile.GetComponent<Missile>().SetSelfWeaponsSystem(transform);
        if (lockedTarget) missile.GetComponent<Missile>().SetTarget(lockedTarget);
        Destroy(missile.gameObject, 15f);
    }

    public bool SectorDetection(Transform target)
    {
        if (!target) return false;
        float distance = Vector3.Distance(transform.position, target.position);//计算自机与敌机距离
        Vector3 norVec = transform.rotation * Vector3.forward;
        Vector3 temVec = target.position - transform.position;
        Debug.DrawLine(transform.position, target.position, Color.green);//画出与目标点的连线
        angle = Mathf.Acos(Vector3.Dot(norVec.normalized, temVec.normalized)) * Mathf.Rad2Deg;//计算两个向量间的夹角
        if (distance < fighterParameter.radarPerformanceDistance && angle <= fighterParameter.radarPerformanceAngle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void FindClosestEnemy()
    {
        switch (gameObject.GetComponent<FighterController>().Type)
        {
            case FighterController.controllerType.Player:
                if (targetArr.Count == 0)
                {
                    target = null;
                    currentTarget = null;
                    lockedTarget = null;
                    return;
                }
                target = targetArr[0].transform;
                var tmpAngle = Mathf.Infinity;
                Vector3 norVec = transform.rotation * Vector3.forward;
                    foreach (GameObject gameobject in targetArr)
                    {
                        Vector3 dis = gameobject.transform.position - transform.position;
                        var targetAngle = Mathf.Acos(Vector3.Dot(norVec.normalized, dis.normalized)) * Mathf.Rad2Deg;
                        if (targetAngle < tmpAngle)
                        {
                            target = gameobject.transform;
                            tmpAngle = targetAngle;
                            if (InputMg.SwitchTargets()) currentTarget = target.transform;
                        }
                    }
                break;
            case FighterController.controllerType.AI:
                if (GameManager.GMins.fighter) target = GameManager.GMins.fighter.transform;
                break;
            default:
                break;
        }
        if (!target)
        {
            target = null;
            currentTarget = null;
            lockedTarget = null;
        }
    }
    public void SetLockedMsg(Transform target, bool isLock)
    {
        if (target.GetComponent<WeaponsSystem>()) target.GetComponent<WeaponsSystem>().isEnemyleLocked = isLock;
    }
    public void SetMissileMsg(Transform target, bool isLock)
    {
        if (target != null && target.GetComponent<WeaponsSystem>()) target.GetComponent<WeaponsSystem>().isMissileLocked = isLock;
    }
}