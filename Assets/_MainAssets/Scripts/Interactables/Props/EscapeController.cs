using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Controller for the escape conditions of the mission. 
public class EscapeController : MonoBehaviour
{
    [Header("Settings")]
    public bool canEscape = false;
    public bool timesUp = false;
    public bool escaped = false;
    [SerializeField]
    private float missionDuration = 300;
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

        // Start the mission timer. Once it has run out, call OnMissionTimerFinish().
        TimerManager.Instance.StartTimer(missionDuration, timerText, OnMissionTimerFinish);
    }

    // Called by the Checklist Controller once the checklist has been completed. Enables Escape
    public void ChecklistFilled()
    {
        canEscape = true;
        EnableVanInteraction();
    }

    public float GetMissionDuration()
    {
        return missionDuration;
    }

    // makes the escape van interactable. see EscapeVan.
    private void EnableVanInteraction()
    {
        gameObject.tag = "Interactable";
        gameObject.AddComponent<EscapeVan>();
    }

    // Called upon mission timer completion. 
    private void OnMissionTimerFinish()
    {
        // Timer now counts down to AI activation instead of instant-gameover. See Navigation.
        // EscapeFailed();
        
        // Displays status message that the authorities have arrived. Play Alarm
        statusDisplay.DisplayMessage(StatusMessage.AuthoritiesArrived, 5.0f);
        SoundManager.PlaySound(gameObject, SoundEffect.Alarm_TimeLow, 0.5f);

        // Activate All AI to begin chasing the player.
        GameObject[] aiObjects = GameObject.FindGameObjectsWithTag("AI");
        foreach (GameObject aiObj in aiObjects)
        {
            if (aiObj.GetComponent<Navigation>() != null)
            {
                aiObj.GetComponent<Navigation>().ActivateAI();
            }
        }
       
    }

    // Call if escape fails (if player is caught).
    public void EscapeFailed()
    {
        if (!escaped)
        {
            // Display the loss message. Start a timer to return to start screen.
            TimerManager.Instance.PauseTimer();
            statusDisplay.DisplayMessage(StatusMessage.YouLose, endMissionStatusDuration);

            TimerManager.Instance.StartTimer(endMissionStatusDuration, null, ReturnToMenu);
        }
    }

    // Call on mission success
    public void EscapeSuccess()
    {
        // Display the win message. Start a time to return to the start screen.
        TimerManager.Instance.PauseTimer();
        statusDisplay.DisplayMessage(StatusMessage.YouWin, endMissionStatusDuration);
        escaped = true;

        TimerManager.Instance.StartTimer(endMissionStatusDuration, null, ReturnToMenu);
    }
    
    // Return to start screen. re-enables cursor movement and visibility.
    public void ReturnToMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MenuScene");
    }
}
