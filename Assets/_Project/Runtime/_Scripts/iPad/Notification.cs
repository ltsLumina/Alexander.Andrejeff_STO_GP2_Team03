using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subjectText;
    [SerializeField] TextMeshProUGUI messageText;

    public void SetContent(StatusBarManager.NotificationEntry entry)
    {
        StatusBarManager statusBar = StatusBarManager.Instance;
        
        subjectText.text = entry.Subject;
        
        bool hasTimeTag = entry.Message.Contains("{Time}");
        if (hasTimeTag) entry.Message = entry.Message.Replace("{Time}", statusBar.EndWorldTime);

        (float minutes, float seconds) realCurrentTime = statusBar.WorldToRealTime(StatusClock.Instance.WorldTime);
        (float minutes, float seconds) realEndTime = statusBar.WorldToRealTime(statusBar.EndWorldTime);
        float realMinutesUntilEnd = Mathf.RoundToInt(realEndTime.minutes - realCurrentTime.minutes);
        float realSecondsUntilEnd = Mathf.RoundToInt(realEndTime.seconds - realCurrentTime.seconds);
        
        bool isMinutes = realMinutesUntilEnd >= 1;
        float realTimeUntilEnd = isMinutes ? realMinutesUntilEnd : realSecondsUntilEnd;
        
        bool isPlural = !Mathf.Approximately(realTimeUntilEnd, 1);
        string noun = isMinutes ? isPlural ? "minutes" : "minute" : isPlural ? "seconds" : "second";

        string realTimeMsg = $"<b>(In {realTimeUntilEnd} {noun})</b>";
        realTimeMsg = hasTimeTag ? realTimeMsg : string.Empty; // hide real time if the time tag is not used.
        
        messageText.text = $"{entry.Message} {realTimeMsg}";
    }
}
