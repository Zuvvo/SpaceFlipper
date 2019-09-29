using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiFinishGameInfo : MonoBehaviour
{
    public Text HeaderText;
    public Text ScoreText;

    private string winInfo = "YOU WON!";
    private string loseInfo = "YOU LOST!";


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAgainClick();
        }
    }

    public void TryAgainClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void Init(bool isGameWon)
    {
        gameObject.SetActive(true);
        if (isGameWon)
        {
            HeaderText.text = winInfo;
        }
        else
        {
            HeaderText.text = loseInfo;
        }
        ScoreText.text = string.Format("Time: {0}s\nEnemies killed: {1}", Math.Floor(GameController.Instance.GameTime * 100) / 100, EnemyController.Instance.KilledEnemyCounter);
    }
}