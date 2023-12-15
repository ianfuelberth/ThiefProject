using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Used to create and manage timers
public class TimerManager : MonoBehaviour
{
    private static TimerManager instance;

    private Coroutine activeCoroutine;
    private float remainingTime;

    public static TimerManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject timerManagerObject = new GameObject("TimerManager");
                instance = timerManagerObject.AddComponent<TimerManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start a timer that lasts for duration seconds. if provided, displays the timer on a TMP_Text.
    // If provided, will also call a provided method upon the timer's completion.
    public void StartTimer(float duration, TMP_Text timerText = null, System.Action onFinishCallback = null)
    {
        remainingTime = duration;

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(RunTimer(timerText, onFinishCallback));
    }

    public void PauseTimer()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
    }

    public void ResumeTimer()
    {
        if (activeCoroutine != null)
        {
            activeCoroutine = StartCoroutine(RunTimer(null, null));
        }
    }

    // Used for running a coroutine timer
    private IEnumerator RunTimer(TMP_Text timerText, System.Action onFinishCallback)
    {
        while (remainingTime > 0)
        {
            if (timerText != null)
            {
                UpdateTimerText(remainingTime, timerText);
            }

            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        // Timer finished, execute the callback if provided
        onFinishCallback?.Invoke();
    }

    // Displays the time remaining on the TMP_Text provided.
    private void UpdateTimerText(float time, TMP_Text timerText)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
