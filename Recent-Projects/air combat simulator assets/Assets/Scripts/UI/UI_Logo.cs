using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Logo : MonoBehaviour
{
    public enum fadeStatus
    {
        fadeIn,
        fadeWaiting,
        fadeOut
    }
    public fadeStatus status;
    public Image UI_Logo_back;
    public Image UI_Logo_img;
    public Sprite[] UI_Logo_imgSpriteArr;

    [Header("等待时间")] public float waitTime;
    [Header("渐变速度")] public float fadeSpeed;

    int logoImgIndex;
    float colorAlpha;
    float timer;
    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        FadeUpdate();
    }
    private void Init()
    {
        logoImgIndex = 0;
        colorAlpha = 0;
        status = fadeStatus.fadeIn;

        if (UI_Logo_back != null) UI_Logo_back.rectTransform.sizeDelta = new Vector2(GameManager.Screen_width, GameManager.Screen_height);
        if (UI_Logo_img != null)
        {
            UI_Logo_img.color = new Color(1, 1, 1, colorAlpha);
            UI_Logo_img.sprite = UI_Logo_imgSpriteArr[logoImgIndex];
        }
    }
    public void FadeUpdate()
    {
        fadeStatus fadeStatus = status;
        switch (status)
        {
            case fadeStatus.fadeIn:
                colorAlpha += fadeSpeed * Time.deltaTime;
                break;
            case fadeStatus.fadeWaiting:
                timer += Time.deltaTime;
                if (timer >= waitTime || Input.anyKey)     //waitTime控制logo停留时间，或者按下任意键
                {
                    timer = 0f;
                    status = fadeStatus.fadeOut;
                }
                break;
            case fadeStatus.fadeOut:
                colorAlpha -= fadeSpeed * Time.deltaTime;
                if (colorAlpha <= 0)
                {
                    if (logoImgIndex < UI_Logo_imgSpriteArr.Length - 1)
                    {
                        logoImgIndex++;
                        UI_Logo_img.sprite = UI_Logo_imgSpriteArr[logoImgIndex];   //轮换图片
                        UI_Logo_img.SetNativeSize();
                        status = fadeStatus.fadeIn;
                    }
                }
                break;
            default:
                break;
        }
        if (UI_Logo_img != null)
        {
            UI_Logo_img.color = new Color(1, 1, 1, colorAlpha);
            if (colorAlpha > 1)
            {
                colorAlpha = 1f;
                status = fadeStatus.fadeWaiting;
            }
        }
        if (logoImgIndex == UI_Logo_imgSpriteArr.Length - 1 && status == fadeStatus.fadeOut && colorAlpha <= 0) //logo展示结束
        {
            GameManager.GMins.LoadStartMenu();
            Destroy(gameObject);
        }
    }
}