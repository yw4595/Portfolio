using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("是否开启游戏运行")] [SerializeField] bool isRuning;
    [Header("创建敌人数量")] [SerializeField] public int enemyNum;

    [Header("关卡高度限制")] public static float heightLimit = 12000;
    [Header("关卡海平限制")] public static float minimumLimit = 300;
    [Header("关卡范围限制")] public static float regionalLimit = 30000;

    [HideInInspector] public static GameManager GMins;
    [HideInInspector] public static float Screen_width = UnityEngine.Screen.width;
    [HideInInspector] public static float Screen_height = UnityEngine.Screen.height;
    [HideInInspector] public bool isGameRunning;
    [HideInInspector] public GameObject fighter;

    [HideInInspector] public List<GameObject> playerArray;
    [HideInInspector] public List<GameObject> enemyArray;

    public Transform cloudsUp;
    public Transform cloudsDown;

    public AudioSource bgmOnStart;
    public AudioSource sfxOnStart;

    float timer;
    float tmpTimer;

    public Transform uiBase;
    public Transform originPoint;

    GameObject cameraController;
    GameObject enemy;

    UI_Logo logo;
    UI_StartMenu startMenu;

    private void Awake()
    {
        GMins = this;
        Debug.Log("AC青春版");
        if (!isRuning) Debug.LogWarning("检查GameManager是否激活运行");
        InitGM();
        LoadLogo();
    }
    private void Update()
    {
        if (isGameRunning) timer += Time.deltaTime;
        if (fighter)
        {
            cloudsUp.position = new Vector3(Mathf.Lerp(cloudsUp.position.x, fighter.transform.position.x, 0.1f), fighter.transform.position.y, Mathf.Lerp(cloudsUp.position.z, fighter.transform.position.z, 0.1f));
            cloudsDown.position = new Vector3(Mathf.Lerp(cloudsDown.position.x, fighter.transform.position.x, 0.1f), fighter.transform.position.y, Mathf.Lerp(cloudsDown.position.z, fighter.transform.position.z, 0.1f));
        }
        EndGame();
    }

    void InitGM()
    {
        cameraController = GameObject.Find("Camera");
        fighter = Resources.Load<GameObject>("Prefabs/Fighter/fighter");
        if (!fighter) Debug.LogError("战斗机载入失败");
        enemy = Resources.Load<GameObject>("Prefabs/Fighter/fighter");
        if (!enemy) Debug.LogError("敌人载入失败");
        logo = Resources.Load<UI_Logo>("Prefabs/UI/UI_Logo");
        if (!logo) Debug.LogError("Logo载入失败");
        startMenu = Resources.Load<UI_StartMenu>("Prefabs/UI/UI_StartMenu");
        if (!startMenu) Debug.LogError("StartMenu载入失败");
    }
    public void LoadLogo()
    {
        if (!isRuning) return;
        if (!logo) return;
        Instantiate<UI_Logo>(logo, uiBase);
    }
    public void LoadStartMenu()
    {
        if (!startMenu) return;
        Instantiate<UI_StartMenu>(startMenu, uiBase);
        timer = 0;
    }
    public void CreatFighterPlayer()
    {
        fighter.GetComponent<FighterController>().Type = FighterController.controllerType.Player;
        fighter = Instantiate<GameObject>(fighter);
        cameraController.GetComponent<CameraController>().SetFighter(fighter);
        cameraController.GetComponent<CameraController>().InitCameraSetting();
        playerArray.Add(fighter);
    }
    public void CreatAIfighter(int enemyNum)
    {
        if (enemyNum <= 0 || !enemy) return;
        for (int i = 0; i < enemyNum; i++)
        {
            enemy.GetComponent<FighterController>().Type = FighterController.controllerType.AI;
            enemy = Instantiate<GameObject>(enemy);
            enemy.transform.position = new Vector3(Random.Range(500, -500), Random.Range(500, -500), Random.Range(500, -500));
            enemy.transform.localEulerAngles = new Vector3(Random.Range(90, -90), Random.Range(90, -90), Random.Range(90, -90));
            enemy.name = "F22";
            enemy.transform.tag = "Enemy";
            fighter.GetComponent<WeaponsSystem>().targetArr.Add(enemy);
            enemyArray.Add(enemy);
        }
    }

    void EndGame()
    {
        if (!isGameRunning) return;
        if (timer < 5) return;
        if (!fighter || fighter.GetComponent<WeaponsSystem>().targetArr.Count == 0 || playerArray.Count == 0)
        {
            tmpTimer += Time.deltaTime;
            if (tmpTimer >= 5)
            {
                isGameRunning = false;
                LoadStartMenu();
                for (int i = enemyArray.Count - 1; i >= 0; i--) 
                {
                    if (enemyArray[i] != null) Destroy(enemyArray[i].gameObject);
                    enemyArray.RemoveAt(i);
                }
                for (int i = GameManager.GMins.playerArray.Count - 1; i >= 0; i--)
                {
                    Destroy(GameManager.GMins.playerArray[i].gameObject);
                    GameManager.GMins.playerArray.Remove(GameManager.GMins.playerArray[i]);
                }
                fighter = null;
                InitGM();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cloudsUp.position = Vector3.zero;
                cloudsDown.position = Vector3.zero;
            }
        }
    }
}
