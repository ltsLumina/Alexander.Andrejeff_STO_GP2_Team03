using System;
using Lumina.Essentials.Modules.Singleton;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Instructions")]
    [field: SerializeField] public GameObject InstructionsPrefab { get; private set; }
    
    private GameObject _instructionsInstance;
    
    
    // GETTER
    private bool IsLocalRoleB => LobbyState.Instance.LocalRole == 2;
    public bool IsInstructionsActive => _instructionsInstance != null && _instructionsInstance.activeSelf;

    private void Start()
    {
        ShowInstructionsToRole();
    }

    #region PUBLIC METHODS

    public void ShowInstructionsToRole()
    {
        if (!IsLocalRoleB) return;

        if (_instructionsInstance == null)
        {
            _instructionsInstance = Instantiate(InstructionsPrefab);
            GameManager.Instance.OnLevelEnter -= ShowInstructionsToRole;
        }
        else
        {
            _instructionsInstance.SetActive(true);
        }
    }
    
    public void HideInstructions()
    {
        if (!IsLocalRoleB) return;
        if (_instructionsInstance != null)
        {
            _instructionsInstance.SetActive(false);
        }
    }
    
    #endregion
}