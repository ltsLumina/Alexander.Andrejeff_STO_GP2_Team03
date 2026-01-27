using Newtonsoft.Json.Bson;
using Unity.Netcode;
using UnityEngine;

public class PauseAllNetwork : NetworkBehaviour
{

    public void PressPause()
    {
        PauseAllServerRpc();
    }

    [ServerRpc]
    void PauseAllServerRpc()
    {
        PauseAllClientRpc();
    }

    [ClientRpc]
    void PauseAllClientRpc()
    {

    }
}
