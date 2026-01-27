using System.Net;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Sequence = Lumina.Essentials.Sequencer.Sequence;

public class AdTrigger : MonoBehaviour
{
    [SerializeField] VideoPlayer video;
    [SerializeField] CanvasGroup videoGroup;
    [SerializeField] Button skipAdButton;
    [SerializeField] Image skipAdGauge;
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] float adDuration = 5f;

    Page AssociatedPage => transform.parent.GetComponent<Page>();
    
    void Awake()
    {
        AssociatedPage.BecameActive += _ =>
        {
            gameObject.SetActive(true);
            InstructionManager.Instance.SetLockNavigation(true);
        };
        
        video.loopPointReached += OnVideoEnd;
        skipAdButton.onClick.AddListener(SkipAd);
    }

    void Start()
    {
        videoGroup.alpha = 0;
        videoGroup.DOFade(1, 0.35f).SetLink(gameObject);

        skipAdButton.image.DOFade(0, 0).SetLink(gameObject);
        skipAdButton.interactable = false;

        skipAdGauge.fillAmount = 0;
        skipAdGauge.DOFillAmount(1, adDuration).SetEase(Ease.Linear).SetLink(gameObject).OnComplete(ShowSkipButton);

        video.enabled = true;
        enabled = true;
    }

    void Update()
    {
        float elapsed = (float)video.time;
        var time = GetTimeComponents(elapsed, (float)video.length);
        progressText.text = $"{time.elapsedMin}:{time.elapsedSec:00} / {time.totalMin}:{time.totalSec:00}";
    }

    void ShowSkipButton()
    {
        skipAdButton.image.DOFade(1, 0.5f).SetLink(gameObject).onComplete += () => skipAdButton.interactable = true;
    }

    public void SkipAd()
    {
        Debug.Log("Advertisement skipped.");
        InstructionManager.Instance.SetLockNavigation(false);
        Destroy(gameObject);
    }

    public void ClickAd()
    {
        InstructionManager.Instance.SetLockNavigation(false);
        
        var sequence = new Sequence(this);
        sequence.WaitThenExecute(1.5f, () =>
        {
            Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
        });
    }
    
    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Advertisement finished.");
        InstructionManager.Instance.SetLockNavigation(false);
        
        Destroy(gameObject);
    }

    static TimeComponents GetTimeComponents(float elapsed, float total)
    {
        var components = new TimeComponents();
        
        components.elapsedSec = Mathf.FloorToInt(elapsed);
        components.elapsedMin = components.elapsedSec / 60;
        components.elapsedRem = components.elapsedSec % 60;
        
        components.totalSec = Mathf.FloorToInt(total);
        components.totalMin = components.totalSec / 60;
        components.totalRem = components.totalSec % 60;

        return components;
    }
}

struct TimeComponents
{
    public int elapsedSec;
    public int elapsedMin;
    public int elapsedRem;
    public int totalSec;
    public int totalMin;
    public int totalRem;
}
