using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterParameter : MonoBehaviour
{// 更改飞机性能时应修改加减速性能和最大出力性能，巡航速度为游戏常规属性应该固定在166f
    [Header("是否为模板机")] public bool isTemplateFighter;
    [Header("机体ID")] public int fighterId;
    [Header("机体名称")] public string fighterName;

    [Header("机体耐久")] public float durability;
    [Header("索敌距离")] public float radarPerformanceDistance;
    [Header("索敌角度")] public float radarPerformanceAngle;
    [Header("隐身性能")] public float stealthPerformance;

    [Header("可控性")] public float controllability;
    [Header("巡航出力")] public float cruiseOutput;
    [Header("俯仰性能")] public float pitchPerformance;
    [Header("翻滚性能")] public float rollPerformance;
    [Header("偏航性能")] public float yawPerformance;
    [Header("加速性能")] public float accelerationPerformance;
    [Header("减速性能")] public float decelerationPerformance;
    [Header("最大速度")] public float fighterMaxSpeed;
    [Header("最小速度")] public float fighterMixSpeed;
    [Header("起飞速度")] public float takeOffSpeed;

    [Header("机体模组")] public string fighterPrefab;
    private void Awake()
    {
        if (isTemplateFighter) InitTemplateFighter();
    }
    public void InitTemplateFighter()
    {
        fighterId = 1000;
        fighterName = "原型机";
        durability = 500f;
        radarPerformanceDistance = 1300f;
        radarPerformanceAngle = 20f;
        stealthPerformance = 100f;
        controllability = 1000f;
        cruiseOutput = 166f;
        pitchPerformance = 100f;
        rollPerformance = 100f;
        yawPerformance = 100f;
        accelerationPerformance = 30f;
        decelerationPerformance = accelerationPerformance * 0.5f;
        fighterMaxSpeed = 2900f;
        fighterMixSpeed = 180f;
        takeOffSpeed = 55f;
        fighterPrefab = "";
    }
}
