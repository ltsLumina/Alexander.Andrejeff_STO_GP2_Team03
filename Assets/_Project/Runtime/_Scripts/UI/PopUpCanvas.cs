using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PopUpAlignment
{
    Top,
    Middle,
    Bottom
}

public enum PopUpType
{
    Info,
    Warning,
    Error
}

public class PopUpCanvas : PersistentCanvas
{
    [SerializeField] private Image topSection;
    [SerializeField] private Image middleSection;
    [SerializeField] private Image bottomSection;
    
    [SerializeField] private Color infoColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color errorColor = Color.red;
    
    #region PopUp Message Handlers
    
    // DELEGATE 
    private delegate void PopUpAction(string message, PopUpAlignment alignment = PopUpAlignment.Top, PopUpType type = PopUpType.Info);
    private static event PopUpAction OnPopUpMessage;
    
    private static string _currentMessage = "";
    public static void TriggerPopUpMessage(string message, PopUpAlignment alignment = PopUpAlignment.Top, PopUpType type = PopUpType.Info)
    {
        _currentMessage = message;
        OnPopUpMessage?.Invoke(message, alignment, type);
    }
    
    #endregion

    private void Start()
    {
        HidePopUp();
        
        OnPopUpMessage += HandlePopUpMessage;
    }
    
    private void OnDisable()
    {
        OnPopUpMessage -= HandlePopUpMessage;
    }

    private void HandlePopUpMessage(string message, PopUpAlignment alignment = PopUpAlignment.Top, PopUpType type = PopUpType.Info)
    {
        ShowPopUp(alignment);
        
        Transform popUpSectionTransform = alignment switch
        {
            PopUpAlignment.Top => topSection.transform,
            PopUpAlignment.Middle => middleSection.transform,
            PopUpAlignment.Bottom => bottomSection.transform,
            _ => topSection.transform
        };
        
        TMP_Text sectionText = popUpSectionTransform.GetComponentInChildren<TMP_Text>();
        sectionText.text = message;
        sectionText.color = type switch
        {
            PopUpType.Info => infoColor,
            PopUpType.Warning => warningColor,
            PopUpType.Error => errorColor,
            _ => infoColor
        };
        
        // Tween PopUpSection
        popUpSectionTransform.localScale = Vector3.zero;
        
        Sequence popUpSequence = DOTween.Sequence();
        popUpSequence.Append(popUpSectionTransform.DOScale(Vector3.one * 1.25f, 0.2f));
        popUpSequence.Append(popUpSectionTransform.DOScale(Vector3.one, 0.1f));
        popUpSequence.Play();
        
        // Hide after delay
        float displayDuration = Mathf.Clamp(message.Length * 0.1f, 2f, 5f);
        DOVirtual.DelayedCall(displayDuration, () =>
        {
            Sequence hideSequence = DOTween.Sequence();
            hideSequence.Append(popUpSectionTransform.DOScale(Vector3.one * 1.25f, 0.1f));
            hideSequence.Append(popUpSectionTransform.DOScale(Vector3.zero, 0.2f));
            hideSequence.OnComplete(HidePopUp);
            hideSequence.Play();
        });
    }

    private void ShowPopUp(PopUpAlignment alignment = PopUpAlignment.Top)
    {
        Transform popUpSectionTransform = alignment switch
        {
            PopUpAlignment.Top => topSection.transform,
            PopUpAlignment.Middle => middleSection.transform,
            PopUpAlignment.Bottom => bottomSection.transform,
            _ => topSection.transform
        };
        
        popUpSectionTransform.gameObject.SetActive(true);
    }

    private void HidePopUp()
    {
        topSection.transform.gameObject.SetActive(false);
        middleSection.transform.gameObject.SetActive(false);
        bottomSection.transform.gameObject.SetActive(false);
    }
}
