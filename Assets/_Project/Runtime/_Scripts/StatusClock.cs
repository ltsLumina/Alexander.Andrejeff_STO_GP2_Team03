#region
using System;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules.Singleton;
using TMPro;
using UnityEngine;
#endregion

public class StatusClock : Singleton<StatusClock>
{
    [Header("Reference"), Space(10)]
    [SerializeField] TextMeshProUGUI timerText;
    
    [Header("Timer Settings")]
    [SerializeField, ReadOnly] string worldTime = "14:30"; // HH:MM
    [SerializeField, ReadOnly] float gameTime; // in seconds

    public delegate void TimerEnded();
    public static event TimerEnded OnTimerEnded;
    
    public static event Action<string> OnTimerUpdated; // world time
    
    // -- Properties --
    public string WorldTime => worldTime;
    public float GameTime => gameTime;

    float CurrentTime
    {
        get => gameTime;
        set => gameTime = value;
    }

    void Update()
    {
        if (timerText == null) return;
        
        IncreaseTime(Time.deltaTime);

        UpdateTimerText();
    }

    #region Timer Methods
    void IncreaseTime(float delta)
    {
        CurrentTime += delta;
        OnTimerUpdated?.Invoke(WorldTime);
    }

    public void SetTimer(float newTime) => CurrentTime = newTime;
    
    public void ResetTimer() => CurrentTime = 0;
    #endregion

    bool finished;

    void UpdateTimerText()
    {
        // set the timer text to 00:00 format
        int totalSeconds = Mathf.CeilToInt(CurrentTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        worldTime = $"{minutes}:{seconds:D2}";
        gameTime = CurrentTime;
        timerText.text = worldTime;
    
        // If the timer has finished, invoke the OnTimerEnded event
        if (CurrentTime >= StatusBarManager.Instance.WorldToRealTime(StatusBarManager.Instance.EndWorldTime).seconds && !finished)
        {
            finished = true;
            
            OnTimerEnded?.Invoke();
            Debug.Log("iPad Clock: Timer ended.");
            OnTimerEnd();
        }
    }
    
    //[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Everyone)] // probably want to do something like this i assume
    void OnTimerEnd()
    {
        // // sine timeScale to zero and back to 1 over 2 seconds
        // var sequence = DOTween.Sequence();
        // sequence.SetUpdate(true);
        // sequence.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 1f).SetEase(Ease.InOutSine));
        // sequence.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 1f).SetEase(Ease.InOutSine));
    }
}