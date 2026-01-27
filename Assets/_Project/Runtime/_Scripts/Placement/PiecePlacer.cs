using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PiecePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject selectedPiece;
    [SerializeField] private Transform cursor;
    [SerializeField] private Transform wobbleTransform;
    [SerializeField] private PlacementSettings placementSettings;

    [Header("Organization")]
    [SerializeField] private Transform buildPartsParent;
    
    [Header("Network")]
    [SerializeField] private NetworkedPiecePlacer networkedPiecePlacer;

    // DELEGATE 
    public delegate void OnPiecePlacedDelegate(GameObject placedPiece);
    public OnPiecePlacedDelegate OnPiecePlaced;
    
    private GameObject _spawnedPiece;
    private GameObject _previewPiece;
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

    #region UNITY METHODS
    private void Awake()
    {
        _mainCamera = Camera.main;
        
        if(BuildActionMap != null)
            BuildActionMap["BuildLeftRelease"].performed += OnCanceledClick;
    }
    
    private void Update()
    {
        UpdateSpawnedPiecePosition();
        UpdateCursorPosition();
        UpdateRotateInput();
        UpdateHoldingAnimation();
        UpdatePreviewPiecePosition();
    }

    private void OnDestroy()
    {
        if(BuildActionMap != null)
            BuildActionMap["BuildLeftRelease"].performed -= OnCanceledClick;
    }

    #endregion
    
    #region PRIVATE METHODS
    
    private void OnLocalPlayerReady()
    {
        if(BuildActionMap == null) return; // safety check
        Debug.Log("Connected and ready - assigning input");
        
        
        PlayerController.OnLocalPlayerReady -= OnLocalPlayerReady;
    }

    private void UpdateHoldingAnimation()
    {
        if(_spawnedPiece == null) return; // safety check
        
        // Wobble piece side to side while holding
        float wobbleAmount = Mathf.Sin(Time.time * placementSettings.WobbleSpeed) * placementSettings.WobbleStrength;
        wobbleTransform.transform.rotation = Quaternion.Euler(wobbleAmount, wobbleTransform.transform.rotation.eulerAngles.y, 0f);
    }

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
        //if(PlayerInput == null) return; // safety check
        
        // Horizontal Rotation
        bool wasRotatePressed = BuildActionMap["Rotate_1"].WasPressedThisFrame();
        
        if (wasRotatePressed && _spawnedPiece != null)
        {
            _spawnedPiece.transform.Rotate(Vector3.up, placementSettings.HorizontalRotateStep, Space.World);
        }
        
        // Vertical Rotation
        bool wasVerticalRotatePressed = BuildActionMap["Rotate_2"].WasPressedThisFrame();
        
        if (wasVerticalRotatePressed && _spawnedPiece != null)
        {
            _spawnedPiece.transform.Rotate(Vector3.right, placementSettings.VerticalRotateStep, Space.World);
        }
        
        // Reset Rotation
        bool wasResetRotatePressed = BuildActionMap["ResetRotate"].WasPressedThisFrame();
        
        if (wasResetRotatePressed && _spawnedPiece != null)
        {
            _spawnedPiece.transform.rotation = Quaternion.Euler(Vector3.zero);
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
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, placementSettings.PlacementLayerMask))
        {
            return hitInfo.point;
        }
        
        return ray.origin + ray.direction * placementSettings.PieceProjectionDistance;
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
    
    #region PUBLIC METHODS
    
    public void SelectPiece(GameObject piecePrefab)
    {
        selectedPiece = piecePrefab;
        OnGameClick();
    }
    
    #endregion
    
    #region INPUT METHODS
    private void OnGameClick()
    {
        // Spawn Piece
        if (_spawnedPiece == null)
        {
            _spawnedPiece = Instantiate(selectedPiece, Vector3.zero, Quaternion.identity, wobbleTransform);
            _spawnedPiece.gameObject.layer = LayerMask.NameToLayer("Holding");
            
            _spawnedPiece.gameObject.name = $"Piece_{Guid.NewGuid()}";
        }
        
        // Preview Piece
        if (_previewPiece == null)
        {
            _previewPiece = Instantiate(selectedPiece, Vector3.zero, Quaternion.identity);
            
            Collider[] colliders = _previewPiece.GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                Destroy(col);
            }
            
            Renderer[] renderers = _previewPiece.GetComponentsInChildren<Renderer>();
            foreach (var rend in renderers)
            {
                rend.material = placementSettings.PreviewMaterial;
            }
        }
    }
    
    private void OnCanceledClick(InputAction.CallbackContext ctx)
    {
        if(_spawnedPiece == null) return; // safety check
        
        // Drop Piece
        // Raycast down to see if we can place the piece here
        Ray ray = new Ray(_spawnedPiece.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, ~LayerMask.GetMask("Holding", "DetectPlane")))
        {
            // Place piece on hit point
            if (networkedPiecePlacer != null)
            {
                // Notify networked placer to spawn piece for all clients
                networkedPiecePlacer.RequestSpawnPiece(selectedPiece, hitInfo.point, _spawnedPiece.transform.rotation);
            }
            
            Destroy(_spawnedPiece);
            
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