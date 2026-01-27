using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalPauseHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private GameObject goPausePanel;
    [SerializeField] private GameObject goPauseButton;
    [SerializeField] private GameObject goResumeButton;
    [SerializeField] private Button disconnectButton;
    private void Start()
    {
        NetworkGameState.Instance.CountdownValue.OnValueChanged += OnCountDownChanged;
        NetworkGameState.Instance.IsPaused.OnValueChanged += OnPauseChanged;

        goPausePanel.SetActive(false);
        countDownText.gameObject.SetActive(false);
        
        disconnectButton.onClick.AddListener(Disconnect);
    }

    private void OnDestroy()
    {
        if (NetworkGameState.Instance != null)
        {
            NetworkGameState.Instance.CountdownValue.OnValueChanged -= OnCountDownChanged;
            NetworkGameState.Instance.IsPaused.OnValueChanged -= OnPauseChanged;
        }
        
        disconnectButton.onClick.RemoveListener(Disconnect);
    }


    public void PauseTheGame()
    {
        NetworkGameState.Instance.RequestPauseServerRpc();
    }

    public void ResumeTheGame()
    {
        NetworkGameState.Instance.RequestResumeServerRpc();
    }

    public void Disconnect()
    {
        if(LobbyState.Instance != null)
            LobbyState.Instance.Disconnect();
    }

    private void OnCountDownChanged(int oldValue, int newValue)
    {
        if (newValue > 0)
        {
            ShowCountDownNumber(newValue);
        }
        else
        {
            HideCountDownUI();
        }
    }

    private void OnPauseChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            ApplyPauseLocally();
        }
        else
        {
            RemovePauseLocally();
        }
    }

    private void ShowCountDownNumber(int value)
    {
        countDownText.text = value.ToString();
        countDownText.gameObject.SetActive(true);
    }

    private void HideCountDownUI()
    {
        countDownText.gameObject.SetActive(false);
    }

    private void ApplyPauseLocally()
    {
        goPausePanel.SetActive(true);
        Time.timeScale = 0f;

        goPauseButton.SetActive(false);
        goResumeButton.SetActive(true);
    }

    private void RemovePauseLocally()
    {
        goPausePanel.SetActive(false);
        Time.timeScale = 1f;

        goPauseButton.SetActive(true);
        goResumeButton.SetActive(false);
    }

}
