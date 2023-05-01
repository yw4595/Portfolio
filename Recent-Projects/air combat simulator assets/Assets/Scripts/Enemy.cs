using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody enemyRg;
    public Transform target;
    AudioSource audioSource;
    public AudioClip audioClip;
    public float HP = 500;

    public float mixSpeed;
    public float maxSpeed;

    float eng = 10;
    void Start()
    {
        enemyRg = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.GMins.fighter) setTarget();
        if (GameManager.GMins.fighter.GetComponent<WeaponsSystem>().lockedTarget == transform)
        {
            eng -= Time.deltaTime;
            if (eng <= 0) eng = 0;
            huibi();
            return;
        }
        if (target) Move();//Track();
        if (eng<=10) eng += Time.deltaTime;
    }
    public void setTarget()
    {
        target = GameManager.GMins.fighter.transform;
    }
    void Track()
    {
        var speed = Random.Range(mixSpeed, maxSpeed);
        var ver = (target.position - transform.position);
        var dis = Vector3.Distance(target.position, transform.position);
        Quaternion ro = Quaternion.LookRotation(ver);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, ro, speed * 0.001f * Time.deltaTime);
        enemyRg.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }
    void huibi()
    {
        var speed = maxSpeed;
        Quaternion ro = Quaternion.identity;
        ro.eulerAngles += new Vector3(Random.Range(-20, 0) * Time.deltaTime * 0.3f , 0, 0);
        transform.rotation *= ro;
        enemyRg.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }
    public void HpDown(float damge)
    {
        audioSource.PlayOneShot(audioClip);
        HP -= damge;
        if (HP <= 0) Destroy(gameObject, 1f);
    }
    private void Move()
    {
        transform.LookAt(target);
        float dx = target.position.x - transform.transform.position.x;
        float dy = target.position.y - transform.transform.position.y;
        float rotationZ = Mathf.Atan2(dy, dx) * 180 / Mathf.PI;
        rotationZ -= 90;
        float originRotationZ = transform.eulerAngles.z;
        float addRotationZ = rotationZ - originRotationZ;
        if (addRotationZ > 180) addRotationZ -= 360;
        transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + addRotationZ);
        enemyRg.AddForce(transform.forward * maxSpeed, ForceMode.VelocityChange);
    }
}