using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceOptionUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private PiecePlacer _piecePlacer;
    
    [SerializeField] private GameObject piecePrefab;

    private void Awake()
    {
        _piecePlacer = GameObject.FindGameObjectWithTag("GrabPiece").GetComponent<PiecePlacer>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (LobbyState.Instance == null || LobbyState.Instance.LocalRole != 1) return;
        Debug.Log("Piece option clicked: " + gameObject.name);
        
        _piecePlacer.SelectPiece(piecePrefab);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (LobbyState.Instance == null || LobbyState.Instance.LocalRole != 1) return;
        Debug.Log("Piece option released: " + gameObject.name);
        
        //_piecePlacer.DeselectPiece();
    }
}