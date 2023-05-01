using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 默认速度为600
/// </summary>

public class FighterSystem : MonoBehaviour
{
    FighterParameter fighterParameter;
    InputManager inputManager;
    FighterSound fighterSound;
    Rigidbody fighterRigidbody;


    [HideInInspector] public float durability;                  //机体耐久

    [HideInInspector] public float controllability;             //可控性
    [HideInInspector] public float cruiseOutput;                //巡航出力
    [HideInInspector] public float pitchPerformance;            //俯仰性能
    [HideInInspector] public float rollPerformance;             //翻滚性能
    [HideInInspector] public float yawPerformance;              //偏航性能
    [HideInInspector] public float accelerationPerformance;     //加速性能
    [HideInInspector] public float decelerationPerformance;     //减速性能
    [HideInInspector] public float fighterMaxSpeed;             //最大速度
    [HideInInspector] public float fighterMixSpeed;             //最小速度
    [HideInInspector] public float takeOffSpeed;                //起飞速度

    [HideInInspector] public float inputRoll, inputPitch, inputYaw;
    float modifiedCoefficientRoll = 0.06f;
    float modifiedCoefficientPitch = 0.015f;
    float modifiedCoefficientYaw = 0.0025f;
    Quaternion mainRot = Quaternion.identity;
    Quaternion addRot = Quaternion.identity;
    Vector3 velocityTarget = Vector3.zero;
    [HideInInspector] public float engineOutput = 0;
    float flightSpeed = 0;


    public void InitBaseComponent()
    {
        fighterParameter = GetComponent<FighterParameter>();
        inputManager = GetComponent<InputManager>();
        fighterRigidbody = GetComponent<Rigidbody>();
        fighterSound = GetComponent<FighterSound>();
        if (!inputManager) { Debug.LogError("无法找到输入控制器"); return; }
        if (!fighterParameter) { Debug.LogError("无法找到战斗机数据"); return; }
        if (!fighterSound) { Debug.LogError("无法找到战斗机声音管理器"); return; }
        if (fighterParameter.isTemplateFighter) fighterParameter.InitTemplateFighter();
        if (fighterSound) fighterSound.InitBaseComponent();
        if (fighterRigidbody.constraints == RigidbodyConstraints.FreezePositionX) Debug.LogWarning(fighterParameter.fighterName + "的刚体被冻结了，所以无法对齐施加任何力");
        if (fighterRigidbody.constraints == RigidbodyConstraints.FreezePositionY) Debug.LogWarning(fighterParameter.fighterName + "的刚体被冻结了，所以无法对齐施加任何力");
        if (fighterRigidbody.constraints == RigidbodyConstraints.FreezePositionZ) Debug.LogWarning(fighterParameter.fighterName + "的刚体被冻结了，所以无法对齐施加任何力");



        durability = fighterParameter.durability;
        controllability = fighterParameter.controllability;
        cruiseOutput = fighterParameter.cruiseOutput;
        pitchPerformance = fighterParameter.pitchPerformance;
        rollPerformance = fighterParameter.rollPerformance;
        yawPerformance = fighterParameter.yawPerformance;
        accelerationPerformance = fighterParameter.accelerationPerformance;
        decelerationPerformance = fighterParameter.decelerationPerformance;
        fighterMaxSpeed = fighterParameter.fighterMaxSpeed;
        fighterMixSpeed = fighterParameter.fighterMixSpeed;
        takeOffSpeed = fighterParameter.takeOffSpeed;
        #region 判断赋值是否成功，如果未能赋值则赋值0.0001，保证程序运行，并报错
        if (controllability == 0) { controllability = 0.0001f; Debug.LogWarning("controllability的值可能为0"); }
        if (cruiseOutput == 0) { cruiseOutput = 0.0001f; Debug.LogWarning("cruiseOutput的值可能为0"); }
        if (pitchPerformance == 0) { pitchPerformance = 0.0001f; Debug.LogWarning("pitchPerformance的值可能为0"); }
        if (rollPerformance == 0) { rollPerformance = 0.0001f; Debug.LogWarning("rollPerformance的值可能为0"); }
        if (yawPerformance == 0) { yawPerformance = 0.0001f; Debug.LogWarning("yawPerformance的值可能为0"); }
        if (accelerationPerformance == 0) { accelerationPerformance = 0.0001f; Debug.LogWarning("accelerationPerformance的值可能为0"); }
        if (decelerationPerformance == 0) { decelerationPerformance = 0.0001f; Debug.LogWarning("decelerationPerformance的值可能为0"); }
        if (fighterMaxSpeed == 0) { fighterMaxSpeed = 0.0001f; Debug.LogWarning("engineMaxOutput的值可能为0"); }
        if (fighterMixSpeed == 0) { fighterMixSpeed = 0.0001f; Debug.LogWarning("engineMixOutput的值可能为0"); }
        if (takeOffSpeed == 0) { takeOffSpeed = 0.0001f; Debug.LogWarning("takeOffSpeed的值可能为0"); }
        #endregion
    }
    public void InputControl()
    {
        if (!inputManager.canControl) return;
        #region 油门和刹车
        if (inputManager.Accelerate()) Accelerator();
        if (inputManager.Slowdown()) Slowdown();
        #endregion
        #region 俯仰和翻滚
        inputRoll = Mathf.Lerp(inputRoll, Mathf.Clamp(InputRoll(), rollPerformance * -1, rollPerformance) * modifiedCoefficientRoll, Time.fixedDeltaTime);
        inputPitch = Mathf.Lerp(inputPitch, Mathf.Clamp(InputPitch(), pitchPerformance * -1, pitchPerformance) * modifiedCoefficientPitch, Time.fixedDeltaTime);
        #endregion
        #region 偏航
        if (inputManager.YawLeft()) inputYaw -= Time.fixedDeltaTime * yawPerformance * modifiedCoefficientYaw;
        if (inputManager.YawRight()) inputYaw += Time.fixedDeltaTime * yawPerformance * modifiedCoefficientYaw;
        inputYaw = Mathf.Lerp(inputYaw, 0, Time.fixedDeltaTime);
        #endregion
    }

    float InputRoll()
    {
        if (!inputManager.canControl || !inputManager) return 0;
        return inputManager.Roll();
    }
    float InputPitch()
    {
        if (!inputManager.canControl || !inputManager) return 0;
        return inputManager.Pitch();
    }


    public void EngineControl()
    {
        flightSpeed = cruiseOutput + engineOutput;
        velocityTarget = fighterRigidbody.rotation * Vector3.forward * flightSpeed;
        fighterRigidbody.AddForce((velocityTarget - fighterRigidbody.velocity), ForceMode.VelocityChange);
        if (!inputManager.Accelerate() && !inputManager.Slowdown())
        {
            if (engineOutput != 0)
            {
                engineOutput = Mathf.Lerp(engineOutput, 0, Time.fixedDeltaTime * 0.1f);
                if (engineOutput > -0.5f && engineOutput < 0.5) engineOutput = 0;
            }
        }
        if (!inputManager.Slowdown() && fighterRigidbody.drag != 1)
        {
            fighterRigidbody.drag = Mathf.Lerp(fighterRigidbody.drag, 1, Time.fixedDeltaTime * 0.5f);
            if (fighterRigidbody.drag < 1.1) fighterRigidbody.drag = 1;
        }
    }

    public void AttitudeControl(float pitch, float yaw, float roll)
    {
        addRot.eulerAngles = new Vector3(pitch, yaw, -roll);
        mainRot *= addRot;
        fighterRigidbody.rotation = Quaternion.Lerp(fighterRigidbody.rotation, mainRot, Time.fixedDeltaTime * controllability);
    }
    public void Accelerator()
    {
        if (fighterRigidbody.velocity.magnitude * 3.69f < fighterMaxSpeed) engineOutput += accelerationPerformance * Time.fixedDeltaTime;
    }
    public void Slowdown()
    {
        if (engineOutput > 0) engineOutput -= decelerationPerformance * Time.fixedDeltaTime;
        if (fighterRigidbody.drag < 30) fighterRigidbody.drag += Time.fixedDeltaTime * 3f;
    }

    public void DurabilityDown(float damge)
    {
        durability -= damge;
        if (durability <= 0 && gameObject.CompareTag("Enemy"))
        {
            for (int i = GameManager.GMins.fighter.GetComponent<WeaponsSystem>().targetArr.Count - 1; i >= 0; i--)
            {
                if (GameManager.GMins.fighter.GetComponent<WeaponsSystem>().targetArr[i] == gameObject)
                {
                    GameManager.GMins.fighter.GetComponent<WeaponsSystem>().targetArr.Remove(GameManager.GMins.fighter.GetComponent<WeaponsSystem>().targetArr[i]);
                    Destroy(gameObject);
                }
            }
        }
        else if (durability <= 0 && !gameObject.CompareTag("Enemy"))
        {
            for (int i = GameManager.GMins.playerArray.Count - 1; i >= 0; i--)
            {
                if (GameManager.GMins.playerArray[i] == gameObject)
                {
                    GameManager.GMins.playerArray.Remove(GameManager.GMins.playerArray[i]);
                    Destroy(gameObject);
                }
            }
        }
    }
}