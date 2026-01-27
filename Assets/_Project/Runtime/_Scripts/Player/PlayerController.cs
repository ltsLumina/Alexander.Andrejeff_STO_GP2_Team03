using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    // DELEGATES
    public delegate void OnLocalPlayerReadyDelegate();
    public static OnLocalPlayerReadyDelegate OnLocalPlayerReady;
    
    public delegate void OnLocalPlayerDisconnectedDelegate();
    public static OnLocalPlayerDisconnectedDelegate OnLocalPlayerDisconnected;
    
    // GETTERS
    private InputActionMap PlayerActionMap
    {
        get
        {
            if (InputManager.Instance == null)
            {
                return null;
            }
            return InputManager.Instance.PlayerActionMap;
        }
    }
    
    #region UNITY METHODS
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        gameObject.name = IsOwner ? "Local Player" : "Remote Player";

        if (!IsOwner) return;
        
        if(PlayerActionMap != null)
            PlayerActionMap["ToggleManual"].performed += OnToggleManualPerformed;
        
        OnLocalPlayerReady?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        if (!IsOwner) return;
        
        if(PlayerActionMap != null)
            PlayerActionMap["ToggleManual"].performed -= OnToggleManualPerformed;
        OnLocalPlayerDisconnected?.Invoke();
    }

    #endregion
    
    #region INPUT METHODS
    
    private void OnToggleManualPerformed(InputAction.CallbackContext ctx)
    {
        if (UIManager.Instance == null) return;
        
        bool manualActive = UIManager.Instance.IsInstructionsActive;
        
        if (manualActive)
        {
            UIManager.Instance.HideInstructions();
        }
        else
        {
            UIManager.Instance.ShowInstructionsToRole();
        }
    }
    #endregion
}