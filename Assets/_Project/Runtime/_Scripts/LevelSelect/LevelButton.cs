using System;
using System.Threading;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text _levelNameText;
    [SerializeField] private Button _button;
    [SerializeField] private Image _imageButton;
    
    public LevelData LevelData {get; set;}

    private Image _glowEffectSprite;
    
    public Color ReturnColor {get; set;}

    private void Awake()
    {
        _glowEffectSprite = GetComponent<Image>();
        ReturnColor = Color.gray;
    }

    private void OnDestroy()
    {
        _button?.onClick?.RemoveAllListeners();
    }

    public void Setup(LevelData level, bool isUnlocked)
    {
        LevelData = level;
        _levelNameText.text = level.LevelName;
        
        _button.interactable = isUnlocked;

        if (isUnlocked)
        {
           Unlock();
        }
        else
        {
            ReturnColor = Color.gray;
            _imageButton.color = ReturnColor;
        }
    }


    public void Unlock()
    {
        _glowEffectSprite.enabled = true;
        _glowEffectSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(LevelData.Width, LevelData.Height);
        
        _button.interactable = true;
        _button.onClick.AddListener(LoadLevel);
        ReturnColor = Color.aliceBlue;
        _imageButton.color = ReturnColor;
    }

    private void LastUnlock()
    {
        // For showing level that the player unlocked 
    }
    

    private void LoadLevel()
    {
       //  Load scene by network thinig
       Debug.Log($"Loading Level {LevelData.LevelName}");
       NetworkManager.Singleton.SceneManager.LoadScene(LevelData.SceneName, LoadSceneMode.Single);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // When hovering over button and is unlocked
        if(LevelData.IsUnlocked)
            _button.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(LevelData.IsUnlocked)
            _button.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
    }
}
