using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PieceMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject testPieceToMove;
    [SerializeField] private float pieceProjectionDistance = 5f;
    [SerializeField] private Transform cursor;
    [SerializeField] private LayerMask placementLayerMask;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Material previewMaterial;
    
    // TEMP
    private PlayerInput _playerInput;
    private GameObject _spawnedPiece;
    private GameObject _previewPiece;
    private Camera _mainCamera;

    #region UNITY METHODS
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        
        _playerInput.actions.Enable();
        _playerInput.actions["GameClick"].performed += OnGameClick;
        _playerInput.actions["GameClick"].canceled += OnCanceledClick;
        
        _mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        _playerInput.actions["GameClick"].performed -= OnGameClick;
        _playerInput.actions["GameClick"].canceled -= OnCanceledClick;
    }


    private void Update()
    {
        UpdateSpawnedPiecePosition();
        UpdateCursorPosition();
        UpdateRotateInput();
        UpdatePreviewPiecePosition();
    }
    
    #endregion
    
    #region PRIVATE METHODS

    private void UpdateSpawnedPiecePosition()
    {
        if(_spawnedPiece == null) return; // safety check

        _spawnedPiece.transform.position = GetMouseWorldPosition(); // Arbitrary distance from camera
    }

    private void UpdateCursorPosition()
    {
        // Update cursor position
        if (cursor != null)
        {
            cursor.position = GetMouseWorldPosition();
        }
    }

    private void UpdateRotateInput()
    {
        // Rotate piece if rotate input is being held.
        bool wasRotatePressed = _playerInput.actions["Rotate"].IsPressed();
        
        if (wasRotatePressed && _spawnedPiece != null)
        {
            _spawnedPiece.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
    
    private void UpdatePreviewPiecePosition()
    {
        if(_previewPiece == null) return; // safety check

        _previewPiece.transform.position = GetPositionOnGround(GetMouseWorldPosition()) - Vector3.up * 0.1f;
        _previewPiece.transform.rotation = _spawnedPiece != null ? _spawnedPiece.transform.rotation : Quaternion.identity;
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, placementLayerMask))
        {
            return hitInfo.point;
        }
        
        return ray.origin + ray.direction * pieceProjectionDistance;
    }

    private Vector3 GetPositionOnGround(Vector3 position)
    {
        Ray ray = new Ray(_spawnedPiece.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, ~LayerMask.GetMask("Holding", "DetectPlane")))
        {
            return hitInfo.point;
        }
        
        return position;
    }
    
    #endregion
    
    #region INPUT METHODS
    private void OnGameClick(InputAction.CallbackContext ctx)
    {
        // Spawn Piece
        if (_spawnedPiece == null)
        {
            _spawnedPiece = Instantiate(testPieceToMove, Vector3.zero, Quaternion.identity);
            _spawnedPiece.gameObject.layer = LayerMask.NameToLayer("Holding");
        }
        
        // Preview Piece
        if (_previewPiece == null)
        {
            _previewPiece = Instantiate(testPieceToMove, Vector3.zero, Quaternion.identity);
            
            Collider[] colliders = _previewPiece.GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                Destroy(col);
            }
            
            Renderer[] renderers = _previewPiece.GetComponentsInChildren<Renderer>();
            foreach (var rend in renderers)
            {
                rend.material = previewMaterial;
            }
        }
    }
    
    private void OnCanceledClick(InputAction.CallbackContext ctx)
    {
        // Drop Piece
        
        // Raycast down to see if we can place the piece here
        
        Ray ray = new Ray(_spawnedPiece.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, ~LayerMask.GetMask("Holding", "DetectPlane")))
        {
            // Place piece on hit point
            _spawnedPiece.transform.position = hitInfo.point;
        }
        else
        {
            // No valid placement, destroy piece
            Destroy(_spawnedPiece);
        }
        
        _spawnedPiece = null;
        
        // Destroy preview piece
        if (_previewPiece != null)
        {
            Destroy(_previewPiece);
            _previewPiece = null;
        }
    }
    
    #endregion
}