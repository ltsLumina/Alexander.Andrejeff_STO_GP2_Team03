using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject preConnectPanel;
    [SerializeField] private GameObject levelSelectionMap;
    
    // CACHE
    private string _joinCode;

    private void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            SceneManager.LoadScene("NetworkInit");
            return;
        }
        
        preConnectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        levelSelectionMap.SetActive(false);
        
        
        inputField.onValueChanged.AddListener(OnInputFieldChanged);
        
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
    }

    private void OnClientStarted()
    {
        preConnectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        levelSelectionMap.SetActive(true);
        
        LoadingCanvas.SetLoadingStatus(false);
        
        
        NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
    }

    public void StartLoading()
    {
        LoadingCanvas.SetLoadingStatus(true);
    }

    private void OnInputFieldChanged(string value)
    {
        _joinCode = value;
    }

    public async void StartHost()
    {
        StartLoading();
        
        #if UNITY_EDITOR && UNITY_RELAY
        
        try
        {
            // Create allocation for host, and save join code.
            string joinCode = await RelayManager.Instance.CreateAllocation(2);
            
            Debug.Log("Host Join Code: " + joinCode);
            JoinCodeCanvas.SetJoinCode(joinCode);
            
            NetworkManager.Singleton.StartHost();
        }catch (System.Exception e)
        {
            Debug.LogWarning("Failed to start host: " + e.Message);
        }
        #else
        
        NetworkManager.Singleton.StartHost();
        
        #endif
    }

    public void StartClient()
    {
        StartLoading();
        
#if UNITY_EDITOR && UNITY_RELAY
        try
        {
            // Join allocation using join code from input field.
            RelayManager.Instance.JoinAllocation(_joinCode, OnClientJoined, OnClientFailed);
            Debug.Log("Pressed ClientButton");
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to start client: " + e.Message);
        }
#else
        NetworkManager.Singleton.StartClient();
        #endif
    }

    private void OnClientJoined()
    {
        Debug.Log("Successfully joined allocation with join code: " + _joinCode);
        
        NetworkManager.Singleton.StartClient();
    }
    
    private void OnClientFailed()
    {
        Debug.LogWarning("Failed to join allocation with join code: " + _joinCode);

        SceneManager.LoadScene("Main-Menu");
    }
}