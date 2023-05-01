using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject[] cameras;
    public Transform fighter;
    public int cameraNum;

    private void LateUpdate()
    {
        if (!fighter) return;
        CameraFollow();
        if (fighter.GetComponent<InputManager>().SwitchCamera()) SwitchCamera();
    }
    public void SetFighter(GameObject fighter)
    {
        this.fighter = fighter.transform;
        if (!fighter) Debug.LogError("战斗机坐标没有找到");
    }
    public void InitCameraSetting()
    {
        cameraNum = 0;
        cameras[0].gameObject.SetActive(true);
        cameras[1].gameObject.SetActive(false);
        fighter.GetComponent<HUDSystem>().crosshair.gameObject.SetActive(false);
        fighter.GetComponent<HUDSystem>().pitchLaddor.gameObject.SetActive(false);
        fighter.GetComponent<HUDSystem>().hudBox.gameObject.SetActive(false);
        fighter.GetComponent<HUDSystem>().compass.gameObject.SetActive(false);
    }
    void CameraFollow()
    {
        transform.position = fighter.position;
        transform.rotation = fighter.rotation;
    }
    void SwitchCamera()
    {
        if (cameraNum == 1)
        {
            fighter.GetComponent<HUDSystem>().crosshair.gameObject.SetActive(true);
            fighter.GetComponent<HUDSystem>().pitchLaddor.gameObject.SetActive(true);
            fighter.GetComponent<HUDSystem>().hudBox.gameObject.SetActive(true);
            fighter.GetComponent<HUDSystem>().compass.gameObject.SetActive(true);
        }
        else 
        {
            fighter.GetComponent<HUDSystem>().crosshair.gameObject.SetActive(false);
            fighter.GetComponent<HUDSystem>().pitchLaddor.gameObject.SetActive(false);
            fighter.GetComponent<HUDSystem>().hudBox.gameObject.SetActive(false);
            fighter.GetComponent<HUDSystem>().compass.gameObject.SetActive(false);
        }
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }
        cameras[cameraNum].gameObject.SetActive(true);
        cameraNum++;
        if (cameraNum == cameras.Length) cameraNum = 0;
    }
}