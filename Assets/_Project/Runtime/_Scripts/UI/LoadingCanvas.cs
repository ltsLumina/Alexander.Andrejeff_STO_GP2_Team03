using UnityEngine;

public class LoadingCanvas : PersistentCanvas
{
    [Header("Loading Canvas")]
    [SerializeField] private GameObject loadingPanel;

    #region LOADING HANDLER
    
    // DELEGATE 
    public delegate void LoadingDelegate(bool isLoading);
    public static LoadingDelegate OnLoadingStatusChanged;
    
    private static bool _isLoading = false;
    public static bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnLoadingStatusChanged?.Invoke(_isLoading);
            }
        }
    }
    #endregion

    #region STATIC METHODS
    
    public static void SetLoadingStatus(bool isLoading)
    {
        IsLoading = isLoading;
    }
    
    #endregion
    
    private void Start()
    {
        HideLoadingPanel();
        
        OnLoadingStatusChanged += HandleLoadingStatusChanged;
    }
    
    private void OnDisable()
    {
        OnLoadingStatusChanged -= HandleLoadingStatusChanged;
    }

    private void HandleLoadingStatusChanged(bool isLoading)
    {
        if (isLoading)
        {
            ShowLoadingPanel();
        }
        else
        {
            HideLoadingPanel();
        }
    }

    private void ShowLoadingPanel()
    {
        loadingPanel.SetActive(true);
    }

    private void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }
}
