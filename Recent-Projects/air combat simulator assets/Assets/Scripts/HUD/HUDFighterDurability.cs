using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDFighterDurability : MonoBehaviour
{
    public Image fighterDurabilityImg;
    float maxDurability;
    float currentDurability;

    private void Start()
    {
        maxDurability = GameManager.GMins.fighter.GetComponent<FighterParameter>().durability;
        currentDurability = maxDurability;
    }
    private void Update()
    {
        currentDurability = GameManager.GMins.fighter.GetComponent<FighterSystem>().durability;
        if (fighterDurabilityImg) HUDFighterDurabilityUpdate();
    }

    void HUDFighterDurabilityUpdate()
    {
        fighterDurabilityImg.fillAmount = Mathf.Lerp(fighterDurabilityImg.fillAmount, currentDurability / maxDurability, 0.05f);
    }
}
