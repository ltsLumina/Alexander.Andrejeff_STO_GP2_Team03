using System;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class NetworkedPiecePlacer : NetworkBehaviour
{
    public void RequestSpawnPiece(GameObject piecePrefab, Vector3 position, Quaternion rotation)
    {
        int prefabId = GameManager.Instance.PiecePrefabLibrary.GetIdByPiecePrefab(piecePrefab);
        SpawnPieceServerRpc(prefabId, position, rotation);
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SpawnPieceServerRpc(int piecePrefab, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = GameManager.Instance.PiecePrefabLibrary.GetPiecePrefabById(piecePrefab);
        GameObject spawnedPiece = Instantiate(prefab, position, rotation, GameManager.Instance.BuildPartsParent.transform);
        spawnedPiece.GetComponent<NetworkObject>().Spawn();

        spawnedPiece.GetComponent<NetworkObject>().TrySetParent(GameManager.Instance.BuildPartsParent.transform);
        spawnedPiece.gameObject.name = $"Piece_{Guid.NewGuid()}";

        GameManager.Instance.RegisterPieceAdded(spawnedPiece.transform);
    }
    
    /*[ClientRpc] 
    private void SpawnPieceClientRpc(int piecePrefab)
    {
        
    }*/
}