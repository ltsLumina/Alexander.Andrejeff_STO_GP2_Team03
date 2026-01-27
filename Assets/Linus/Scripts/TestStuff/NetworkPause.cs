using Unity.Netcode;
using UnityEngine;

public class NetworkPause : NetworkBehaviour
{
    public static NetworkPause Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Button → Server
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestPauseServerRpc()
    {
        PauseClientRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestResumeServerRpc()
    {
        ResumeClientRpc();
    }

    // Server → All Clients
    [ClientRpc]
    void PauseClientRpc()
    {
        IngameMenu.Instance.StartPauseFlow();
    }

    [ClientRpc]
    void ResumeClientRpc()
    {
        IngameMenu.Instance.StartResumeFlow();
    }
}
