using UnityEngine;
using System.Collections;

public class WaveCountdownTimer : MonoBehaviour
{
    [Header("Timer Configuration")]
    [SerializeField] private Timer dialTimer;
    [SerializeField] private float countdownDuration = 4f;
    
    // Events
    public System.Action OnCountdownComplete;
    public System.Action<int> OnCountdownTick; // Sends remaining seconds
    
    private bool isCountdownActive = false;
    
    private void Awake()
    {
        // Find Timer component if not assigned
        if (dialTimer == null)
        {
            dialTimer = GetComponent<Timer>();
        }
        
        if (dialTimer == null)
        {
            dialTimer = GetComponentInChildren<Timer>();
        }
        
        if (dialTimer != null)
        {
            // Configure the timer for countdown
            ConfigureTimer();
        }
        else
        {
            Debug.LogError("Timer component not found! Make sure the DialSeconds prefab has a Timer component.");
        }
    }
    
    private void Start()
    {
        // Initially hide the timer
        gameObject.SetActive(false);
    }
    
    private void ConfigureTimer()
    {
        // Set timer to countdown mode
        dialTimer.countMethod = Timer.CountMethod.CountDown;
        
        // Set timer to dial output type
        dialTimer.outputType = Timer.OutputType.Dial;
        
        // Disable auto start - we'll control it manually
        dialTimer.startAtRuntime = false;
        
        // Set display options (seconds only for countdown)
        dialTimer.hoursDisplay = false;
        dialTimer.minutesDisplay = false;
        dialTimer.secondsDisplay = true;
        
        // Set countdown duration
        SetCountdownDuration(countdownDuration);
        
        // Subscribe to timer end event
        dialTimer.onTimerEnd.AddListener(OnTimerComplete);
    }
    
    public void SetCountdownDuration(float duration)
    {
        countdownDuration = duration;
        
        if (dialTimer != null)
        {
            // Set the timer duration (convert to hours, minutes, seconds)
            int totalSeconds = Mathf.CeilToInt(duration);
            dialTimer.hours = 0;
            dialTimer.minutes = 0;
            dialTimer.seconds = totalSeconds;
        }
    }
    
    public void StartCountdown()
    {
        if (dialTimer == null) return;
        
        isCountdownActive = true;
        gameObject.SetActive(true);
        
        // Reset and start the timer
        dialTimer.StopTimer(); // Reset if running
        dialTimer.StartTimer();
        
        Debug.Log($"Wave countdown started: {countdownDuration} seconds");
        
        // Start coroutine to track countdown ticks
        StartCoroutine(CountdownTickCoroutine());
    }
    
    public void StopCountdown()
    {
        if (dialTimer == null) return;
        
        isCountdownActive = false;
        dialTimer.StopTimer();
        gameObject.SetActive(false);
        
        Debug.Log("Wave countdown stopped");
    }
    
    private IEnumerator CountdownTickCoroutine()
    {
        int lastSecond = -1;
        
        while (isCountdownActive && dialTimer.GetRemainingSeconds() > 0)
        {
            int currentSecond = Mathf.CeilToInt((float)dialTimer.GetRemainingSeconds());
            
            // Only fire event when second changes
            if (currentSecond != lastSecond && currentSecond > 0)
            {
                OnCountdownTick?.Invoke(currentSecond);
                lastSecond = currentSecond;
            }
            
            yield return null; // Wait one frame
        }
    }
    
    private void OnTimerComplete()
    {
        isCountdownActive = false;
        gameObject.SetActive(false);
        
        Debug.Log("Wave countdown completed");
        OnCountdownComplete?.Invoke();
    }
    
    public bool IsCountdownActive()
    {
        return isCountdownActive;
    }
    
    public float GetRemainingTime()
    {
        if (dialTimer != null)
        {
            return (float)dialTimer.GetRemainingSeconds();
        }
        return 0f;
    }
    
    // Method to manually trigger completion (for testing)
    [ContextMenu("Test Complete Countdown")]
    public void TestCompleteCountdown()
    {
        if (dialTimer != null)
        {
            dialTimer.StopTimer();
            OnTimerComplete();
        }
    }
}