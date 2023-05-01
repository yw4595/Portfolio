using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPitchLaddorInit : MonoBehaviour
{
    public Transform pitchLaddorRootT;

    public Image pitchLaddorImgUPL;
    public Image pitchLaddorImgUPS;
    public Image pitchLaddorImgDownL;
    public Image pitchLaddorImgDownS;

    public Text pitchLaddorText;

    float pitchLaddorDistance = 10f;
    float pitchLaddorTextDistance = 125f;
    float pitchLaddorPosX = GameManager.Screen_width / 2f;
    float pitchLaddorPosY = GameManager.Screen_height / 2f;

    private void Awake()
    {
        InitHUDPitchLaddor();
    }

    void Start()
    {

    }

    void Update()
    {

    }


    void InitHUDPitchLaddor()
    {
        if (pitchLaddorRootT == null) return;

        pitchLaddorText = Instantiate<Text>(pitchLaddorText, pitchLaddorRootT);
        pitchLaddorText.text = "0";
        pitchLaddorText.transform.position = new Vector3(pitchLaddorRootT.position.x - pitchLaddorTextDistance, pitchLaddorRootT.position.y, pitchLaddorRootT.position.z);

        pitchLaddorText = Instantiate<Text>(pitchLaddorText, pitchLaddorRootT);
        pitchLaddorText.text = "0";
        pitchLaddorText.transform.position = new Vector3(pitchLaddorRootT.position.x + pitchLaddorTextDistance, pitchLaddorRootT.position.y, pitchLaddorRootT.position.z);

        for (int i = 5; i <= 90; i += 5)
        {
            if (i % 10 == 0)
            {
                InitHUDPitchLaddorFun(i, pitchLaddorImgUPL, pitchLaddorImgDownL);
            }
            else
            {
                InitHUDPitchLaddorFun(i, pitchLaddorImgUPS, pitchLaddorImgDownS);
            }
        }
    }

    void InitHUDPitchLaddorFun(int Num, Image pitchLaddorImgUP, Image pitchLaddorImgDOWN)
    {
        pitchLaddorImgUP = Instantiate<Image>(pitchLaddorImgUP, pitchLaddorRootT);
        pitchLaddorImgUP.transform.position = new Vector3(pitchLaddorPosX, pitchLaddorPosY + Num * pitchLaddorDistance, 0);

        pitchLaddorText = Instantiate<Text>(pitchLaddorText, pitchLaddorRootT);
        pitchLaddorText.text = Num.ToString();
        pitchLaddorText.transform.position = new Vector3(pitchLaddorImgUP.transform.position.x - pitchLaddorTextDistance, pitchLaddorImgUP.transform.position.y, pitchLaddorImgUP.transform.position.z);
        pitchLaddorText = Instantiate<Text>(pitchLaddorText, pitchLaddorRootT);
        pitchLaddorText.text = Num.ToString();
        pitchLaddorText.transform.position = new Vector3(pitchLaddorImgUP.transform.position.x + pitchLaddorTextDistance, pitchLaddorImgUP.transform.position.y, pitchLaddorImgUP.transform.position.z);


        pitchLaddorImgDOWN = Instantiate<Image>(pitchLaddorImgDOWN, pitchLaddorRootT);
        pitchLaddorImgDOWN.transform.position = new Vector3(pitchLaddorPosX, pitchLaddorPosY - Num * pitchLaddorDistance, 0);

        pitchLaddorText = Instantiate<Text>(pitchLaddorText, pitchLaddorRootT);
        pitchLaddorText.text = (Num * -1f).ToString();
        pitchLaddorText.transform.position = new Vector3(pitchLaddorImgDOWN.transform.position.x - pitchLaddorTextDistance, pitchLaddorImgDOWN.transform.position.y, pitchLaddorImgDOWN.transform.position.z);
        pitchLaddorText = Instantiate<Text>(pitchLaddorText, pitchLaddorRootT);
        pitchLaddorText.text = (Num * -1f).ToString();
        pitchLaddorText.transform.position = new Vector3(pitchLaddorImgDOWN.transform.position.x + pitchLaddorTextDistance, pitchLaddorImgDOWN.transform.position.y, pitchLaddorImgDOWN.transform.position.z);
    }
}
