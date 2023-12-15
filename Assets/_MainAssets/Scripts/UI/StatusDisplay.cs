using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusMessage
{
    None,
    YouWin,
    YouLose,
    WrongKey,
    LockedSide,
    ToolMismatch,
    AuthoritiesArrived
}

// Handles the display of status messages for the player.
public class StatusDisplay : MonoBehaviour
{
    [SerializeField]
    private Text statusText;
    [SerializeField]
    CanvasGroup canvasGroup;

    private Coroutine coroutine;

    private void Start()
    {
        statusText.text = string.Empty;
    }

    // Display Status Message To Player
    public void DisplayMessage(StatusMessage status, float displayTime = 2.0f)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        canvasGroup.alpha = 1.0f;

        if (status == StatusMessage.YouWin)
        {
            statusText.text = "You completed the mission successfully!";
        }
        else if (status == StatusMessage.YouLose)
        {
            statusText.text = "You failed the mission...";
        }
        else if (status == StatusMessage.WrongKey)
        { 
            statusText.text = "You are not holding the correct key.";
        }
        else if (status == StatusMessage.ToolMismatch)
        {
            statusText.text = "You are already holding an object.";
        }
        else if (status == StatusMessage.LockedSide)
        {
            statusText.text = "The door is locked from this side.";
        }
        else if (status == StatusMessage.AuthoritiesArrived)
        {
            statusText.text = "The authorities have arrived!";
        }

        coroutine = StartCoroutine(FadeMessage(displayTime));
    }

    private IEnumerator QueueMessageClear(float displayTime)
    {
        yield return new WaitForSeconds(displayTime);

        statusText.text = string.Empty;
    }

    // fades out the current status message over time
    private IEnumerator FadeMessage(float displayTime)
    {
        float startTime = Time.time;

        while (Time.time < startTime + displayTime)
        {
            float normalizedTime = (Time.time - startTime) / displayTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, normalizedTime);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        statusText.text = string.Empty;
    }

}
