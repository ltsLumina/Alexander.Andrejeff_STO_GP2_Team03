using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Easier to keep track of which canvas to enable/disable
/// </summary>
public enum EndScreenChilds
{
    AccText,
    SelectButtons,
    ConfirmButton,
}

public class ScoreScreenUI : NetworkBehaviour
{
    public string sceneName;

    [Header("UI Buttons")]
    public Button LevelSelectButton;
    public Button RetryLevelSelectButton;
    public Button ConfirmDoneSelectButton;


    private void Awake()
    {
        if (LevelSelectButton == null || RetryLevelSelectButton == null || ConfirmDoneSelectButton == null)
        {
            Debug.LogError($"{nameof(LevelSelectButton)} is null");
            return;
        }

        LevelSelectButton.onClick.AddListener(OnLevelSelectClick); 
        RetryLevelSelectButton.onClick.AddListener(OnRestartLevel); 
        ConfirmDoneSelectButton.onClick.AddListener(OnConfirmDoneSelect);
    }

    private void OnDisable()
    {
        LevelSelectButton.onClick.RemoveListener(OnLevelSelectClick); 
        RetryLevelSelectButton.onClick.RemoveListener(OnRestartLevel); 
        ConfirmDoneSelectButton.onClick.RemoveListener(OnConfirmDoneSelect);
    }

    private void OnLevelSelectClick()
    {

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"{sceneName} is null");
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single); 
    }

    private void OnRestartLevel()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single); 
    }

    private void OnConfirmDoneSelect()
    {
        // when pressed start the End sequence
        GameManager.Instance.TryEndGameFromServer();
        transform.GetChild((int)EndScreenChilds.ConfirmButton).gameObject.SetActive(false);
    }
    
    
}
