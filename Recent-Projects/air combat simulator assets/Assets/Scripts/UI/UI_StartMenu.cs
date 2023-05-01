using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartMenu : MonoBehaviour
{
    public Image black;
    public InputField inputEnemyNum;
    public Toggle isReverseY;
    public Toggle isKeepInput;
    float alpha = 1;

    void Update()
    {
        if (alpha > 0) shouUI();
    }
    void shouUI()
    {
        if (alpha > 0)
        {
            alpha -= Time.deltaTime;
            black.color = new Color(0, 0, 0, alpha);
            if (alpha <= 0) alpha = 0;
        }
    }
    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        GameManager.GMins.isGameRunning = true;
        GameManager.GMins.CreatFighterPlayer();
        if (GameManager.GMins.fighter)
        {
            GameManager.GMins.fighter.GetComponent<InputManager>().isKeepInput = isKeepInput.isOn;
            GameManager.GMins.fighter.GetComponent<InputManager>().isReverseY = isReverseY.isOn;
        }
        GameManager.GMins.CreatAIfighter(GameManager.GMins.enemyNum);
        black.color = new Color(0, 0, 0, 1);
        GameManager.GMins.sfxOnStart.Play();
        GameManager.GMins.bgmOnStart.Play();
        Destroy(this.gameObject);
    }
    public void InputEnemyNum()
    {
        if (inputEnemyNum.text != "")
        {
            GameManager.GMins.enemyNum = int.Parse(inputEnemyNum.text);
        }
        else if (inputEnemyNum.text == "" || int.Parse(inputEnemyNum.text) == 0)
        {
            GameManager.GMins.enemyNum = 1;
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}