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
    ToolMismatch
}


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

        coroutine = StartCoroutine(FadeMessage(displayTime));
    }

    private IEnumerator QueueMessageClear(float displayTime)
    {
        yield return new WaitForSeconds(displayTime);

        statusText.text = string.Empty;
    }

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
