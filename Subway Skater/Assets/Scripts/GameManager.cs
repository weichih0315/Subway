using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    public static GameManager instance;    

    PlayerMotor playerMotor;

    private bool isGameStarted = false;

    private float modifierScore = 0;

    private GameData gameData = new GameData();

    private bool playerIsDead = false;

    private float tempScore;

    private int tempCoin;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        Load();
        GameUI.instance.UpdateHighScoreUI(gameData.highScore);
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.S) || Input.touchCount >= 1) && !isGameStarted && !IsPointerOverUIObject())
        {
            Play();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            GameUI.instance.ToogleQuitMenu();
        }

        if (isGameStarted && !playerIsDead)
        {
            tempScore += (Time.deltaTime * modifierScore);
            GameUI.instance.UpdateScoreUI(tempScore);
        }

    }

    public void Play()
    {
        tempScore = 0;
        tempCoin = 0;
        isGameStarted = true;
        playerMotor.StartRun();
        GameUI.instance.Play();
    }

    public void PlayerDead()
    {
        playerIsDead = true;
        GameUI.instance.GameOver();

        Save();

        GameUI.instance.UpdateHighScoreUI(gameData.highScore);
    }

    public void UpdateModifierScore(float modifier)
    {
        modifierScore = modifier;
    }

    public void GetCoin()
    {
        tempCoin += 1;
        GameUI.instance.UpdateCoinUI(tempCoin);
    }

    public void Load()
    {
        string loadString = PlayerPrefs.GetString("GameData", "");

        if (loadString == "")
        {
            string saveString = JsonUtility.ToJson(gameData);
            PlayerPrefs.SetString("GameData", saveString);
            return;
        }

        gameData = JsonUtility.FromJson<GameData>(loadString);
    }

    public void Save()
    {
        Load();
        gameData.coin += tempCoin;

        if (tempScore > gameData.highScore)
            gameData.highScore = tempScore;

        string saveString = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString("GameData", saveString);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
