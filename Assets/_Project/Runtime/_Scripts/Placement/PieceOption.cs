using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PieceOption : MonoBehaviour
{
    private PiecePlacer _piecePlacer;
    [SerializeField] private GameObject piecePrefab;

    private Camera _mainCamera;

    private InputActionMap BuildActionMap
    {
        get
        {
            if (InputManager.Instance == null)
            {
                return null;
            }
            else
            {
                return InputManager.Instance.BuildActionMap;
            }
        }
    }
    
    private void Start()
    {
        _mainCamera = Camera.main;
        _piecePlacer = GameObject.FindGameObjectWithTag("GrabPiece").GetComponent<PiecePlacer>();

        BuildActionMap["BuildLeftClick"].performed += OnMouseClick;
    }

    private void OnDestroy()
    {
        if(BuildActionMap != null)
            BuildActionMap["BuildLeftClick"].performed -= OnMouseClick;
    }

    private void OnMouseClick(InputAction.CallbackContext ctx)
    {
        if (LobbyState.Instance == null || LobbyState.Instance.LocalRole != 1) return;
        if (_mainCamera == null) return;
        RaycastHit[] hits;
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        hits = Physics.RaycastAll(ray, 100.0f);
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform)
            {
                Debug.Log("Piece option clicked: " + gameObject.name);
                _piecePlacer.SelectPiece(piecePrefab);
            }
        }
    }
    
}