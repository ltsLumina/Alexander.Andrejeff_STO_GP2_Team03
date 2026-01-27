using System;
using Unity.Netcode;
using UnityEngine;

public class AudioEventSystem : NetworkBehaviour
{
    private GameManager _gameManager;
    private AudioManager _audioManager;
    private AudioClips _audioClips;
    
    /// <summary>
    /// If subscribing to new events
    /// remember to unsubscribe said event
    /// </summary>
    
    private void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _audioClips = _audioManager.audioSO;
        
        _gameManager.OnPiecePlaced += TryPlayOnPiecePlaced;
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        _gameManager.OnPiecePlaced -= TryPlayOnPiecePlaced;
    }
    
    private void TryPlayOnPiecePlaced()
    {
        OnPiecePlacedClientRpc();
    }

    [ClientRpc]
    private void OnPiecePlacedClientRpc()
    {
        Debug.Log($"Sound Plays at");
        _audioManager.PlaySound(_audioClips.Pickup2);
    }
}
