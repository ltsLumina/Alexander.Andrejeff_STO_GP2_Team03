using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyState : NetworkBehaviour
{
    public static LobbyState Instance;

    public enum InstructionGameMode
    {
        Standard,  // Curated pages with specific modifiers. Pages are still scrambled if the option is enabled.
        Randomized // Random modifiers applied to pages, with limits on how many times each modifier can appear.
    }

    [SerializeField] InstructionGameMode gameMode;

    public NetworkVariable<int> iNetHostrole = new NetworkVariable<int>(0);
    public NetworkVariable<int> iNetClientRole = new NetworkVariable<int>(0);
    
    public int LocalRole => IsHost ? iNetHostrole.Value : iNetClientRole.Value;
    public int OtherRole => IsHost ? iNetClientRole.Value : iNetHostrole.Value;
    public InstructionGameMode GameMode
    {
        get => gameMode;
        set => gameMode = value;
    }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectRole(int role)
    {
        if (IsServer && IsHost)
        {
            iNetHostrole.Value = role;
        }
        else
        {
            RequestRoleServerRpc(role);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestRoleServerRpc(int role)
    {
        iNetClientRole.Value = role;
    }

    public bool ReadyToStart()
    {
        //Check if the host and client roles are different and not set to the default value of 0, that tells if the game is able to start or not.
        return iNetHostrole.Value > 0 && iNetClientRole.Value > 0 && iNetHostrole.Value != iNetClientRole.Value;
    }

    public void Disconnect()
    {
        if (GameManager.Instance == null)
        {
            if (NetworkManager.Singleton == null) return;

            NetworkManager.Singleton.Shutdown();
            StartCoroutine(LoadSceneNextFrame("Main-Menu"));
        }
        else
        {
            GameManager.Instance.TryDisconnect();
        }
        
    }
    
    private IEnumerator LoadSceneNextFrame(string sceneName)
    {
        yield return new WaitForEndOfFrame(); //Just waiting a frame for safety...
        SceneManager.LoadScene(sceneName);
    }
}
