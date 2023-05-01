using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPitchLaddorTextController : MonoBehaviour
{
    Transform fighter;
    private void Awake()
    {

    }
    void Start()
    {
        fighter = GameManager.GMins.fighter.transform;
        if (!fighter) Debug.LogError("战斗机没有找到");
    }


    void Update()
    {
        if (fighter) RotateController(0, 0, fighter.localEulerAngles.z);
    }

    void RotateController(float rX, float rY, float rZ)
    {
        transform.localEulerAngles = new Vector3(rX, rY, rZ);
    }
}
