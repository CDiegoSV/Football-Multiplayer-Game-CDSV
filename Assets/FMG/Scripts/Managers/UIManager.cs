using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager Instance;

    private void Awake()
    {
        if(Instance != null &&  Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    #region References
    [Header("TextMesh References")]
    [SerializeField] private TextMeshProUGUI blueTeamPointTextMesh;
    [SerializeField] private TextMeshProUGUI greenTeamPointTextMesh;
    [SerializeField] private TextMeshProUGUI timerTextMesh;

    [Header("Panel References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("Color References")]
    [SerializeField] private Color blueTeamColor;
    [SerializeField] private Color greenTeamColor;

    #endregion
    #region Public Methods

    public void PausePanelToggle()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public void UpdateTeamPoints(bool blueTeam, int points)
    {
        if (blueTeam)
        {
            blueTeamPointTextMesh.text = points.ToString();
        }
        else
        {
            greenTeamPointTextMesh.text= points.ToString();
        }
    }

    public void TimerTextUpdate(float secondsLeft)
    {
        timerTextMesh.text = TimeSpan.FromSeconds(secondsLeft).ToString(@"m\:ss");
    }

    public void VictoryPanel(bool draw, bool blueTeamWins)
    {
        victoryPanel.SetActive(true);
        if (!draw)
        {
            if (blueTeamWins)
            {
                victoryPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().colorGradient = 
                    new VertexGradient(blueTeamColor, blueTeamColor, blueTeamColor, Color.white);
                victoryPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Blue Team Wins";
            }
            else
            {
                victoryPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().GetComponentInChildren<TextMeshProUGUI>().colorGradient = 
                    new VertexGradient(greenTeamColor, greenTeamColor, greenTeamColor, Color.white);
                victoryPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Green Team Wins";
            }
        }
        else
        {
            victoryPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().GetComponentInChildren<TextMeshProUGUI>().colorGradient = 
                new VertexGradient(Color.red, Color.red, blueTeamColor, greenTeamColor);
                victoryPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Draw";
        }
    }

    public void MenuButton()
    {
        LevelNetworkManager.Instance?.DisconnectCurrentRoom();
    }

    public void QuitButton()
    {
        Application.Quit();
    }
    #endregion
}