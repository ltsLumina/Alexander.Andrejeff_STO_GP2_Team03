#region
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Essentials.Modules.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class InputManager : SingletonPersistent<InputManager>
{
    [Header("References")]
    [field: SerializeField] public PlayerInput PlayerInput { get; private set; }
    
    #region ACTION MAPS
    
    public InputActionMap PlayerActionMap => PlayerInput.actions.FindActionMap("Player");
    public InputActionMap UIActionMap => PlayerInput.actions.FindActionMap("UI");  
    public InputActionMap BuildActionMap => PlayerInput.actions.FindActionMap("Build");

    public InputActionMap InfoActionMap => PlayerInput.actions.FindActionMap("Info");
    
    #endregion
    
    #region UNITY METHODS
    
    private void Start()
    {
        PlayerActionMap.Enable();
        UIActionMap.Enable();
        BuildActionMap.Enable();
        InfoActionMap.Enable();
    }

    #endregion
}
