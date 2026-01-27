using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Lumina.Essentials.Modules;
using Lumina.Essentials.Modules.Singleton;
using UnityEngine;
using VInspector;

/// <summary>
/// not proud of this at all lol
/// -alex
/// </summary>
public class StatusBarManager : Singleton<StatusBarManager>
{
    [Tab("Status")]
    [SerializeField] string startWorldTime = "14:30"; // HH:MM
    [SerializeField] string endWorldTime = "17:00"; // HH:MM
    [SerializeField] float timeScale = 60f; // 1 real minute = 1 in-game hour
    [Tooltip("Use the system's current time as the start time.")]
    [SerializeField] bool useEarthTime;
    [Tooltip("Automatically begin the status clock on Start.")]
    [SerializeField] bool autoStart;

    public string StartWorldTime => startWorldTime;
    public string EndWorldTime => endWorldTime;

    [Tab("Notifications")]
    [SerializeField] Notification prefab;
    [Tooltip("Key: Notification Entry, Value: World time that the notification should appear at")]
    [SerializeField] List<NotificationEntry> notifications;
    [SerializeField] float notificationDisplayDuration = 5f;

    [Tab("References")]
    [SerializeField] GameObject statusBar;
    [SerializeField] Canvas notificationContainer;

#if UNITY_EDITOR
    void OnGUI()
    {
        // timeScale slider
        GUILayout.BeginArea(new Rect(10, 10, 120, 100));
        GUILayout.Label($"Time Scale: {Time.timeScale:F1}");
        Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0f, 100f);
        GUILayout.EndArea();
    }
#endif
    
    void Start()
    {
        if (autoStart) Begin();
    }

    [Button, UsedImplicitly]
    public void Begin()
    {
        // using System.DateTime, get the current time of day in seconds
        string currentEarthTime = DateTime.Now.ToString("HH:mm");
        
        if (useEarthTime) StatusClock.Instance.SetTimer(WorldToRealTime(currentEarthTime).minutes * 60);
        else StatusClock.Instance.SetTimer(WorldToRealTime(startWorldTime).seconds - 1);
        
        StatusClock.OnTimerUpdated += worldTime =>
        {
            for (int index = notifications.Count - 1; index >= 0; index--) // Iterate backwards to allow removal
            {
                NotificationEntry entry = notifications[index];

                if (entry.WorldTime == worldTime)
                {
                    AddNotification(entry);
                    entry.WorldTime = "--:--"; // Mark as shown
                    notifications[index] = entry; // Update the list
                }
            }
        };
    }

    public (float minutes, float seconds) WorldToRealTime(string worldTime)
    {
        string[] timeParts = worldTime.Split(':');
        int hours = int.Parse(timeParts[0]);
        int minutes = int.Parse(timeParts[1]);
        int totalInGameMinutes = hours * 60 + minutes;
        float realTimeMinutes = totalInGameMinutes / timeScale;
        float realTimeSeconds = realTimeMinutes * 60;
        return (realTimeMinutes, realTimeSeconds);
    }

    public void AddNotification(NotificationEntry entry)
    {
        notificationContainer.transform.DestroyAllChildren();
        
        var notification = Instantiate(prefab, notificationContainer.transform);
        ((RectTransform)notification.transform).anchoredPosition = new (0, 115); 
        notification.SetContent(entry);
        
        var rect = notification.GetComponent<RectTransform>();
        var sequence = DOTween.Sequence();
        sequence.SetLink(notification.gameObject); 
        sequence.Append(rect.DOAnchorPosY(-45, 1f).SetEase(Ease.OutCubic));
        sequence.AppendInterval(notificationDisplayDuration);
        sequence.Append(rect.DOAnchorPosY(rect.rect.height + 100, 1f).SetEase(Ease.InCubic));
        sequence.OnComplete(() => Destroy(notification.gameObject));
    }

    [Serializable]
    public struct NotificationEntry
    {
        [Tooltip("The world time (HH:MM) at which the notification should appear.")]
        public string WorldTime;
        public string Subject;
        [Tooltip("Use {Time} as a placeholder for the converted time string.")]
        public string Message;
        
        public NotificationEntry(string worldTime, string subject = "Guests", string message = "We expect to arrive by {Time}.")
        {
            WorldTime = worldTime;
            Subject = subject;
            Message = message;
        }
    }
}
