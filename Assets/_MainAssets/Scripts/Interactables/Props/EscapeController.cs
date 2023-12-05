using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeController : MonoBehaviour
{
    [Header("Settings")]
    public bool canEscape = false;
    public bool timesUp = false;
    [SerializeField]
    private float missionDuration = 20f;
    [SerializeField]
    private float timeRemaining;
    [SerializeField]
    private float endMissionStatusDuration = 3f;
 

    [Header("References")]
    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private StatusDisplay statusDisplay;

    public void Start()
    {
        

        if (statusDisplay == null)
        {
            statusDisplay = GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<StatusDisplay>();
        }

        TimerManager.Instance.StartTimer(missionDuration, timerText, OnMissionTimerFinish);
    }

    public void ChecklistFilled()
    {
        canEscape = true;
        EnableVanInteraction();
    }

    private void EnableVanInteraction()
    {
        gameObject.tag = "Interactable";
        gameObject.AddComponent<EscapeVan>();
    }

    private void OnMissionTimerFinish()
    {
        EscapeFailed();
    }

    public void EscapeFailed()
    {
        TimerManager.Instance.PauseTimer();
        statusDisplay.DisplayMessage(StatusMessage.YouLose, endMissionStatusDuration);

        TimerManager.Instance.StartTimer(endMissionStatusDuration, null, ReturnToMenu);

    }

    public void EscapeSuccess()
    {
        TimerManager.Instance.PauseTimer();
        statusDisplay.DisplayMessage(StatusMessage.YouWin, endMissionStatusDuration);

        TimerManager.Instance.StartTimer(endMissionStatusDuration, null, ReturnToMenu);
    }
    
    public void ReturnToMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MenuScene");
    }
}
