using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PiecePrefabLibrary", menuName = "Libraries/Piece Prefab Library")]
public class PiecePrefabLibrary : ScriptableObject
{
    public Dictionary<int, GameObject> PiecePrefabsById { get; private set; }
    
    [field: SerializeField] public List<GameObject> PiecePrefabs { get; private set; }

    public void Initialize()
    {
        PiecePrefabsById = new Dictionary<int, GameObject>();
        for (int i = 0; i < PiecePrefabs.Count; i++)
        {
            PiecePrefabsById[i] = PiecePrefabs[i];
        }
    }
    
    public GameObject GetPiecePrefabById(int id)
    {
        if (PiecePrefabsById == null)
        {
            Initialize();
        }
        
        if (PiecePrefabsById.TryGetValue(id, out GameObject prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"PiecePrefabLibrary: No prefab found for ID {id}");
            return null;
        }
    }
    
    public int GetIdByPiecePrefab(GameObject prefab)
    {
        if (PiecePrefabsById == null)
        {
            Initialize();
        }
        
        foreach (var kvp in PiecePrefabsById)
        {
            if (kvp.Value == prefab)
            {
                return kvp.Key;
            }
        }
        
        Debug.LogError($"PiecePrefabLibrary: No ID found for prefab {prefab.name}");
        return -1;
    }

    private void OnValidate()
    {
        Initialize();
    }
}