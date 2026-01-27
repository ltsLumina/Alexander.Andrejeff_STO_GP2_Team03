using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinCodeCanvas : PersistentCanvas
{
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private GameObject joinCodeInfoPanel;
    
    #region Join Code Handler
    
    private static string _joinCode;

    private static string JoinCode
    {
        get => _joinCode;
        set
        {
            _joinCode = value;
            Debug.Log("Join Code set to: " + _joinCode);
            OnJoinCodeChanged?.Invoke(_joinCode);
        }
    }
    
    // DELEGATE
    private delegate void JoinCodeChangedDelegate(string newJoinCode);
    private static event JoinCodeChangedDelegate OnJoinCodeChanged;
    
    public static void SetJoinCode(string joinCode)
    {
        JoinCode = joinCode;
    }
    #endregion
    
    #region UNITY METHODS

    private void Start()
    {
        joinCodeInfoPanel.SetActive(false);
    }

    private void OnEnable()
    {
        OnJoinCodeChanged += UpdateJoinCodeText;

        if (InputManager.Instance)
        {
            InputManager.Instance.InfoActionMap["Hide_Info"].performed += OnHideInfoPerformed;
        }
    }

    private void OnDisable()
    {
        OnJoinCodeChanged -= UpdateJoinCodeText;
        
        if (InputManager.Instance)
        {
            InputManager.Instance.InfoActionMap["Hide_Info"].performed -= OnHideInfoPerformed;
        }
    }
    
    #endregion
    
    private void UpdateJoinCodeText(string newJoinCode)
    {
        Debug.Log(newJoinCode);
        joinCodeText.text = newJoinCode;
        joinCodeInfoPanel.SetActive(true);
    }
    
    private void OnHideInfoPerformed(InputAction.CallbackContext ctx)
    {
        joinCodeInfoPanel.SetActive(!joinCodeInfoPanel.activeSelf);
    }
}