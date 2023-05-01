using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTT : MonoBehaviour
{
    public Image iii;
    public Transform self;
    public Transform target;
    public float angle;
    Quaternion originRot;
    public float speed;
    public bool isR;

    public float x1;
    public float y1;
    public float x2;
    public float y2;

    public Text text;
    bool isRotate = true;
    void Start()
    {
        originRot = transform.rotation;
    }
    private void Update()
    {
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 90), 0.05f);
        iii.fillAmount = Mathf.Lerp(iii.fillAmount, x1, 0.01f);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
      //  checkd();
      //  self.GetComponent<Rigidbody>().AddForce(self.forward * speed);
    }


    void lookro()
    {
        if (isRotate)
        {
            Vector3 vec = (target.position - self.position);
            Quaternion rotate = Quaternion.LookRotation(vec);
            transform.localRotation = Quaternion.Slerp(self.localRotation, rotate, 0.6f);
            //if (Vector3.Angle(vec, self.forward) < 0.1f)
            //{
            //    isRotate = false;
            //}
        }
    }


    void zhixiang2()
    {
        self.LookAt(target);
        float dx = target.position.x - self.transform.position.x;
        float dy = target.position.y - self.transform.position.y;
        float rotationZ = Mathf.Atan2(dy, dx) * 180 / Mathf.PI;
        rotationZ -= 90;
        float originRotationZ = self.eulerAngles.z;
        float addRotationZ = rotationZ - originRotationZ;
        if (addRotationZ > 180) addRotationZ -= 360;
        self.localEulerAngles = new Vector3(self.eulerAngles.x, self.eulerAngles.y, self.eulerAngles.z + addRotationZ);
    }

    public void checkTargetDirForMe(Transform target)
    {
        ////xuqiTest：  target.position = new Vector3(3, 0, 5);
        //Vector3 dir = target.position - transform.position; //位置差，方向
        ////方式1   点乘
        ////点积的计算方式为: a·b =| a |·| b | cos < a,b > 其中 | a | 和 | b | 表示向量的模 。
        //float dot = Vector3.Dot(transform.forward, dir.normalized);//点乘判断前后   //dot >0在前  <0在后 
        //float dot1 = Vector3.Dot(transform.right, dir.normalized);//点乘判断左右    //dot1>0在右  <0在左                                               
        //float angle = Mathf.Acos(Vector3.Dot(transform.forward.normalized, dir.normalized)) * Mathf.Rad2Deg;//通过点乘求出夹角

        ////方式2   叉乘
        ////叉乘满足右手准则  公式：模长|c|=|a||b|sin<a,b>  
        //Vector3 cross = Vector3.Cross(transform.forward, dir.normalized); 点乘判断左右  // cross.y>0在左  <0在右 
        // Vector3 cross1 = Vector3.Cross(transform.right, dir.normalized); 点乘判断前后  // cross.y>0在前  <0在后 
        //  angle = Mathf.Asin(Vector3.Distance(Vector3.zero, Vector3.Cross(transform.forward.normalized, dir.normalized))) * Mathf.Rad2Deg;

    }
    public void checkd()
    {
        Vector3 dir = target.position - self.position;
        float dotFB = Vector3.Dot(self.up, dir.normalized);
        if (dotFB != 0)
        {
            self.Rotate(new Vector3(-dotFB, 0, 0));
        }
        float dotLR = Vector3.Dot(transform.right, dir.normalized);
        if (dotLR != 0)
        {
            self.localEulerAngles = new Vector3(self.eulerAngles.x, self.eulerAngles.y, self.eulerAngles.z + -dotLR);
        }

        if(dotLR == 0)
        {

        }
        float angle = self.localEulerAngles.z;

    }
}
