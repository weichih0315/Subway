using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public GameObject quitMenuUI;

    public Animator gameStatusAnimator;
    public CanvasGroup statusCanvasGroup;

    public Animator gameOverUIAnimator;
    public Animator menuUIAnimator;

    public Text highScore;

    [Header("Status")]
    public Text scoreText;
    public Text coinText;
    public Text speedText;

    [Header("GameOverStatus")]
    public Text gameOverScoreText;
    public Text gameOverCoinText;

    [Header("volume")]
    public Slider[] volumeSliders;

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        //volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        //volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;
    }

    public void Play ()
    {
        gameStatusAnimator.SetTrigger("Show");
        menuUIAnimator.SetTrigger("Hide");
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void ToogleQuitMenu()
    {
        quitMenuUI.SetActive (!quitMenuUI.activeSelf);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    public void GameOver()
    {
        gameOverScoreText.text = scoreText.text;
        gameOverCoinText.text = coinText.text;

        gameOverUIAnimator.SetTrigger("Dead");
        gameStatusAnimator.SetTrigger("Hide");
    }

    public void UpdateScoreUI(float score)
    {
        scoreText.text = score.ToString("0");
    }

    public void UpdateCoinUI(float coin)
    {
        coinText.text = coin.ToString("0");
    }

    public void UpdateSpeedUI(float speed)
    {
        speedText.text = speed.ToString("0.0");
    }

    public void UpdateHighScoreUI(float score)
    {
        highScore.text = score.ToString("0");
    }
}
