using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusMessage
{
    None,
    WrongKey
}


public class StatusDisplay : MonoBehaviour
{
    [SerializeField]
    private Text statusText;
    [SerializeField]
    CanvasGroup canvasGroup;
    [SerializeField]
    private float displayTime = 2.0f;

    private Coroutine coroutine;

    private void Start()
    {
        statusText.text = string.Empty;
    }

    public void DisplayMessage(StatusMessage status)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        canvasGroup.alpha = 1.0f;

        if (status == StatusMessage.WrongKey)
        {
            statusText.text = "You are not holding the correct key.";
        }

        coroutine = StartCoroutine(FadeMessage());
    }

    private IEnumerator QueueMessageClear()
    {
        yield return new WaitForSeconds(displayTime);

        statusText.text = string.Empty;
    }

    private IEnumerator FadeMessage()
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
