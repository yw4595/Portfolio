using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InputManager : MonoBehaviour
{
    enum InputType
    {
        gamepad,
        desktop,
        touch
    }
    [Header("灵敏度")] public float sensitivity = 50f;
    [Header("控制开关")] public bool canControl = true;
    [Header("是否反转X轴")] public bool isReverseX;
    [Header("是否反转Y轴")] public bool isReverseY;
    [Header("是否保持输入")] public bool isKeepInput;
    [Header("是否环绕观察")] public bool isLookAround;
    [Header("自动驾驶")] public bool isAutoPilot;

    #region 飞机姿态控制，鼠标控制俯仰和翻滚，A和D控制偏航
    public float Pitch()
    {
        if (isReverseY)
        {
            if (isKeepInput)
            {
                var tmp = (Input.mousePosition.y - UnityEngine.Screen.height * 0.5f) * -1f;
                return tmp * sensitivity * 0.005f;
            }
            return (Input.GetAxis("Mouse Y") * -sensitivity);
        }
        else
        {
            if (isKeepInput)
            {
                var tmp = Input.mousePosition.y - UnityEngine.Screen.height * 0.5f;
                return tmp * sensitivity * 0.005f;
            }
            return Input.GetAxis("Mouse Y") * sensitivity;
        }
    }
    public float Roll()
    {
        if (isReverseX)
        {
            if (isKeepInput)
            {
                var tmp = (Input.mousePosition.x - UnityEngine.Screen.width * 0.5f) * -1f;
                return tmp * sensitivity * 0.001f;
            }
            return (Input.GetAxis("Mouse X") * -sensitivity);
        }
        else
        {
            if (isKeepInput)
            {
                var tmp = Input.mousePosition.x - UnityEngine.Screen.width * 0.5f;
                return tmp * sensitivity * 0.001f;
            }
            return Input.GetAxis("Mouse X") * sensitivity;
        }
    }
    public bool YawLeft()
    {
        if (Input.GetKey(KeyCode.A)) return true;
        return false;
    }
    public bool YawRight()
    {
        if (Input.GetKey(KeyCode.D)) return true;
        return false;
    }
    #endregion
    #region 飞机引擎控制，W控制加力，S控制减速，Shift+W使用额外推进
    public bool Accelerate()
    {
        if (Input.GetKey(KeyCode.W)) return true;
        return false;
    }
    public bool Slowdown()
    {
        if (Input.GetKey(KeyCode.S)) return true;
        return false;
    }
    public bool Afterburner()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift)) return true;
        return false;
    }
    #endregion
    #region 武器控制
    public bool MainWeaponFire()        //主武器开火
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }
        return false;
    }
    public bool SecondWeaponFire()      //副武器开火
    {
        if (Input.GetMouseButton(0)) return true;
        return false;
    }
    public bool SwitchWeapons()     //C切换主武器
    {
        if (Input.GetKeyDown(KeyCode.C)) return true;
        return false;
    }
    public bool SwitchTargets()     //Tab切换锁定目标
    {
        if (Input.GetKeyDown(KeyCode.Tab)) return true;
        return false;
    }
    #endregion
    #region 系统功能
    public bool SwitchCamera()          //F使用热焰弹
    {
        if (Input.GetKeyDown(KeyCode.C)) return true;
        return false;
    }
    public bool UseFlare()          //F使用热焰弹
    {
        if (Input.GetKeyDown(KeyCode.F)) return true;
        return false;
    }
    public bool SwitchMapType()     //M切换地图显示模式
    {
        if (Input.GetKeyDown(KeyCode.M)) return true;
        return false;
    }
    public bool LookAround()        //摄像机环绕模式
    {
        if (Input.GetMouseButton(0)) return true;
        return false;
    }
    #endregion
}