using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lumina.Essentials.Modules.Singleton;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    // SINGLETON (NETWORKED)
    public static GameManager Instance { get; private set; }
    
    [Header("System References")]
    [field: SerializeField] public ValidationSystem ValidationSystem { get; set; }
    [Header("Part References")]
    [field: SerializeField] public GameObject BuildPartsParent { get; private set; }
    [field: SerializeField] public GameObject GhostPartsParent { get; private set; }

    public List<Transform> GhostParts => GhostPartsParent.GetComponentsInChildren<Transform>().ToList();
    public List<Transform> BuildParts => BuildPartsParent.GetComponentsInChildren<Transform>().ToList();

    [HideInInspector] public List<Transform> BuildPieces = new List<Transform>();

    [field: SerializeField] public PiecePrefabLibrary PiecePrefabLibrary { get; private set; }
    
    
    [Header("End Screen")]
    [field: SerializeField] public GameObject EndScreenParent { get; private set; }
    private GameObject _endScreenObject;
    [SerializeField] private Transform ShowGhostPos; 
    [SerializeField] private Transform ShowBuiltPos; 
    
    
    // DELEGATES
    public delegate void OnLevelEnterDelegate();
    public OnLevelEnterDelegate OnLevelEnter;
    
    public delegate void OnLevelExitDelegate();
    public OnLevelExitDelegate OnLevelExit;
    
    public delegate void OnPiecePlacedDelegate();
    public OnPiecePlacedDelegate OnPiecePlaced;
    
    public delegate void OnGameEndedDelegate();
    public OnGameEndedDelegate OnGameEnded;
    
    // GETTERS
    public bool InLevel { get; private set; } // Debounce bool, since OnLevelEnter can be called before some systems are ready (for example systems already in the level scene).

    private void Awake()
    {
        Instance = this;
        
        PiecePrefabLibrary.Initialize();
    }
    
    #region PUBLIC METHODS
    public void NotifyLevelEnter()
    {
        OnLevelEnter?.Invoke();
        InLevel = true;
    }
    
    public void NotifyLevelExit()
    {
        OnLevelExit?.Invoke();
        InLevel = false;
    }


    public void RegisterPieceAdded(Transform piece)
    {
        BuildPieces.Add(piece);
        OnPiecePlaced?.Invoke();
        
        Debug.Log($"Ghost Parts {GhostParts.Count}");
        Debug.Log($"Added build parts {BuildPieces.Count}");

        if (BuildPieces.Count >= GhostParts.Count - 1)
        {
            // adds a confirm button for the player who is building the structure
            TrySpawnEndScreen();
            
           // TryEndGameFromServer();
        }
        
    }
    
    /// <summary>
    /// Will notify all other clients that game has ended and end the game from the server.
    /// </summary>
    public void TryEndGameFromServer()
    {
        Debug.Log("IsHost: " + IsHost);
        Debug.Log("IsServer: " + IsServer);
        Debug.Log("IsClient: " + IsClient);
        if (!IsHost) return;
        
        // Run on the server
        // Server calculates score
        float score = ValidationSystem.CalculateScore();
        
        OnGameEndClientRpc(score);
    }
    
    #endregion


    #region PRIVATE METHODS

    
    private void TrySpawnEndScreen()
    {
        SpawnEndScreenClientRpc();
    }

    [ClientRpc]
    private void SpawnEndScreenClientRpc()
    {
        _endScreenObject = Instantiate(EndScreenParent);
        if (!IsHost)
        {
            _endScreenObject.transform.GetChild((int)EndScreenChilds.ConfirmButton).gameObject.SetActive(false);
        }
    }
    
    
    [ClientRpc]
    private void OnGameEndClientRpc(float score)
    {
        OnGameEnded?.Invoke();
        
        // hides Instructions
        UIManager.Instance.HideInstructions();  
        
        // Show only SelectButtons for Host 
        if(IsHost)
            _endScreenObject.transform.GetChild((int)EndScreenChilds.SelectButtons).gameObject.SetActive(true);
        
        _endScreenObject.transform.GetChild((int)EndScreenChilds.AccText).gameObject.SetActive(true);
        var canvasText =  _endScreenObject.GetComponentInChildren<TMP_Text>();
        canvasText.text = $"Accuracy: {Math.Round((score*100), 2)}%";
        
        // For showcasing the player build objects
        GameObject parentBuildPartsParent = BuildPartsParent.transform.parent.gameObject;
        RotateObject(parentBuildPartsParent, ShowBuiltPos.position);

        // enabels meshes to be rendered for the complete building
        var d = GhostPartsParent.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in d)
        {
            meshRenderer.enabled = true;
        }
        RotateObject(GhostPartsParent, ShowGhostPos.position);

        // add so buildings rotates when showcasing 
        parentBuildPartsParent.AddComponent<ShowCaseBuildings>();
        GhostPartsParent.AddComponent<ShowCaseBuildings>();


    }

    private void RotateObject(GameObject obj, Vector3 pos)
    {
        obj.transform.DOMove(pos, 1f);
        obj.transform.DORotate(new Vector3(180, 0, 0), 1f, RotateMode.WorldAxisAdd);
    }
    
    #endregion
    
    #region  Disconnect Handler

    public override void OnNetworkPreDespawn()
    {
        base.OnNetworkPreDespawn();
        
        StartCoroutine(LoadSceneNextFrame("Main-Menu"));
    }

    public void TryDisconnect()
    {
        try
        {
            if (!IsHost)
            {
                DisconnectHostServerRpc();
            }
            else
            {
                Disconnect();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error during disconnect: " + e.Message);
            StartCoroutine(LoadSceneNextFrame("Main-Menu"));
        }
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void DisconnectHostServerRpc()
    {
        Disconnect();
    }
    
    private void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private IEnumerator LoadSceneNextFrame(string sceneName)
    {
        yield return new WaitForEndOfFrame(); //Just waiting a frame for safety...
        SceneManager.LoadScene(sceneName);
    }

    #endregion
    
    
    
}
