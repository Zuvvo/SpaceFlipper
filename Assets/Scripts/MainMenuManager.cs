using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI PlayerInfo;


    public void QuickStartClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitClick()
    {
        Application.Quit();
    }

}