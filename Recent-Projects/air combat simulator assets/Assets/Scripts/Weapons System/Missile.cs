using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("锁定目标")] public Transform target;
    [Header("导弹伤害")] public float damege;
    WeaponsSystem weaponsSystem;
    GameObject explosiveVFX;
    float radian;
    float speed;
    bool canTrack = true;
    //float damge=255;

    private void Start()
    {
        if (!weaponsSystem) return;
        speed = weaponsSystem.gameObject.GetComponent<Rigidbody>().velocity.magnitude * 2f;
        damege = 300;
    }

    private void FixedUpdate()
    {
        Track();
    }

    public void SetSelfWeaponsSystem(Transform selfFighter)
    {
        weaponsSystem = selfFighter.GetComponent<WeaponsSystem>();
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Track()
    {
        if (target && canTrack)
        {
            target.GetComponent<WeaponsSystem>().SetMissileMsg(target, true);
            Vector3 targetPos = (target.position - transform.position).normalized;
            var dis = Vector3.Distance(target.position, transform.position);
            //if (dis <= 20f) canTrack = false;
            radian = Mathf.Clamp(dis * 0.2f, 10, 60);
            var angle = Vector3.Angle(transform.forward, targetPos) / radian;
            if (angle > 0.1f || angle < -0.1f)
            {
                transform.forward = Vector3.Slerp(transform.forward, targetPos, Time.deltaTime / angle).normalized;
            }
            else
            {
                speed += 25 * Time.deltaTime;
                transform.forward = Vector3.Slerp(transform.forward, targetPos, 1).normalized;
            }
            transform.position += transform.forward * speed * Time.deltaTime;
            if (dis <= 15f)
            {
                HitTheTarget();
            }
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
    void HitTheTarget()
    {
        target.GetComponent<FighterSystem>().DurabilityDown(damege);
        explosiveVFX = Resources.Load<GameObject>("Prefabs/VFX/explosiveVFX");
        if (explosiveVFX) Destroy(Instantiate<GameObject>(explosiveVFX, transform.position, transform.rotation), 10f);
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (target) target.GetComponent<WeaponsSystem>().SetMissileMsg(target, false);
    }
}