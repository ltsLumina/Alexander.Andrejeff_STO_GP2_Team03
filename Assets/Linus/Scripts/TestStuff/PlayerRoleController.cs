using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerRoleController : NetworkBehaviour
{
    [Header("Positions")]
    [SerializeField] Vector3 vRoleASpawnPosition;
    [SerializeField] Vector3 vRoleBSpawnPosition;

    [Header("Local Stuff")]
    int role;

    public override void OnNetworkSpawn()
    {
        role = GetRole();
        Vector3 spawnPos;

        switch (role)
        {
            case 1:
                spawnPos = vRoleASpawnPosition;
                break;
            case 2:
                spawnPos = vRoleBSpawnPosition;
                break;
            default:
                Debug.LogError("Invalid role!");
                return;
        }

        transform.position = spawnPos;
    }

    private int GetRole()
    {
        Debug.Log("GetRole()");
        if (IsServer) 
            return LobbyState.Instance.iNetHostrole.Value;
        else
            return LobbyState.Instance.iNetClientRole.Value;
    }

}
