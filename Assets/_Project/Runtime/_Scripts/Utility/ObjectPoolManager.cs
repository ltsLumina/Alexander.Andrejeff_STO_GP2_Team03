using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool _addToDontDestoryOnLoad = false;

    private GameObject _emptyHolder;

    private static GameObject _gameObjectEmpty;
    private static GameObject _sfxObjectEmpty;
    private static GameObject _musicObjectEmpty;
    
    private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    private static Dictionary<GameObject, GameObject> _cloneToPreFabMap;


    /// <summary>
    /// Creates the different Pool type categories.
    /// If one would like to add or remove categories add a new,
    /// add new "GameObject" variable above and a new "PoolType" Variable,
    /// and add to "SetupEmpties" and "SetParentObject" function.
    /// </summary>
    public enum PoolType
    {
        GameObjects,
        SFX,
        Music,
    }
    public static PoolType PoolingType;


    /// <summary>
    /// Creates Dictionary's for ObjectPools and a clone PreFab
    /// </summary>
    private void Awake()
    {
        _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _cloneToPreFabMap = new Dictionary<GameObject, GameObject>();

        SetupEmpties();
    }

    /// <summary>
    /// Creates GameObject in hierarchy and creates sub objects for each catagories.
    /// And if Don't Destory on Load is true it changes to corosponding object.
    /// </summary>
    private void SetupEmpties()
    {
        _emptyHolder = new GameObject("Object Pools");

        _gameObjectEmpty = new GameObject("GameObjects");
        _gameObjectEmpty.transform.parent = _emptyHolder.transform;
        
        _sfxObjectEmpty = new GameObject("SFXObject");
        _sfxObjectEmpty.transform.parent = _emptyHolder.transform;
        
        _musicObjectEmpty = new GameObject("MusicObject");
        _musicObjectEmpty.transform.parent = _emptyHolder.transform;
        
        if (_addToDontDestoryOnLoad)
        {
            DontDestroyOnLoad(_emptyHolder.transform.root);
        }
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="poolType"></param>
    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        // Creates a pool object and stores the dictionary in _objectPools
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
        );
        
        _objectPools.Add(prefab, pool);
    }
    
    /// <summary>
    /// Creates Game Object to pool and then parent it under a category.
    /// </summary>
    /// <param name="prefab">Game object prefab to pool</param>
    /// <param name="pos">Spawn position</param>
    /// <param name="rot">Rotation of object</param>
    /// <param name="poolType">What pooling type to use</param>
    /// <returns>GameObject of the with its values</returns>
    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);
        
        GameObject obj = Instantiate(prefab, pos, rot);
        
        prefab.SetActive(true);

        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);

        return obj;
    }

    /// <summary>
    /// Sets GameObject's parrent
    /// </summary>
    /// <param name="poolType">The type of Pool to parrent</param>
    /// <returns>GameObject parent</returns>
    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.GameObjects:
                return _gameObjectEmpty;
            
            case PoolType.SFX:
                return _sfxObjectEmpty;
            
            case PoolType.Music:
                return _musicObjectEmpty;
            
            default:
                Debug.LogError("PoolType " + poolType + " not supported");
                return null;
        }  
    }

    /// <summary>
    /// When getting game object to something
    /// </summary>
    /// <param name="obj">GameObject to something</param>
    private static void OnGetObject(GameObject obj)
    {
        
    }
    
    /// <summary>
    /// When deactivated/ destroy game object 
    /// </summary>
    /// <param name="obj">GameObject to deactivate</param>
    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    /// <summary>
    /// Removes object from pool
    /// </summary>
    /// <param name="obj">type of object to remove from pool</param>
    private static void OnDestroyObject(GameObject obj)
    {
        if (_cloneToPreFabMap.ContainsKey(obj))
        {
            _cloneToPreFabMap.Remove(obj);
        }
    }

    /// <summary>
    /// Spawn object and moves to object pool 
    /// </summary>
    /// <param name="gameObjectToSpawn">Game object to pool</param>
    /// <param name="pos">Position to spawn at</param>
    /// <param name="rot">Rotation to spawn at</param>
    /// <param name="poolType">Pool type to sort object</param>
    /// <typeparam name="T">Type of object to spawn</typeparam>
    /// <returns>Object as a variable to use later</returns>
    private static T SpawnObject<T>(GameObject gameObjectToSpawn, Vector3 pos, Quaternion rot,
        PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!_objectPools.ContainsKey(gameObjectToSpawn))
        {
            CreatePool(gameObjectToSpawn, pos, rot, poolType);
        }
        
        GameObject obj = _objectPools[gameObjectToSpawn].Get();

        if (obj != null)
        {
            if (!_cloneToPreFabMap.ContainsKey(obj))
            {
                _cloneToPreFabMap.Add(obj, gameObjectToSpawn);
            }
            
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }
            
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Object {gameObjectToSpawn.name} doesn't have a component of type {typeof(T)}");
            }
            
            return component;
        }
        return null;
    }

    /// <summary>
    /// Spawn object and moves to object pool 
    /// </summary>
    /// <param name="typePrefab">type Prefab to pool</param>
    /// <param name="spawnPos">Position to spawn at</param>
    /// <param name="spawnRot">Rotation to spawn at</param>
    /// <param name="poolType">Pool type to sort object</param>
    /// <typeparam name="T">Type of object to spawn</typeparam>
    /// <returns>Object as a variable to use later</returns>
    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, 
        Quaternion spawnRot, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRot, poolType);
    }

    /// <summary>
    /// Spawn object and moves to object pool 
    /// </summary>
    /// <param name="gameObjectToSpawn">Game object to pool</param>
    /// <param name="spawnPos">Position to spawn at</param>
    /// <param name="spawnRot">Rotation to spawn at</param>
    /// <param name="poolType">Pool type to sort object</param>
    /// <returns>Object as a variable to use later</returns>
    public static GameObject SpawnObject(GameObject gameObjectToSpawn, Vector3 spawnPos, Quaternion spawnRot,
        PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(gameObjectToSpawn, spawnPos, spawnRot, poolType);
    }

    
    /// <summary>
    /// Spawn object and moves to object pool 
    /// </summary>
    /// <param name="gameObjectToSpawn">type Prefab to pool</param>
    /// <param name="spawnPos">Position to spawn at</param>
    /// <param name="spawnRot">Rotation to spawn at</param>
    /// <param name="poolType">Pool type to sort object</param>
    /// <param name="amountOfObjectsToSpawn">Amount of pooled objects to prewarm</param>
    public static void PreWarmPool(GameObject gameObjectToSpawn, Vector3 spawnPos, Quaternion spawnRot,
        PoolType poolType = PoolType.GameObjects, int amountOfObjectsToSpawn = 5)
    {
        GameObject[] preWarmObjects = new GameObject[amountOfObjectsToSpawn];
        
        for (int i = 0; i < amountOfObjectsToSpawn; i++)
        {
            preWarmObjects[i] = SpawnObject<GameObject>(gameObjectToSpawn, spawnPos, spawnRot, poolType);
        }

        foreach (GameObject warmObject in preWarmObjects)
        {
            ReturnObjectToPool(warmObject, poolType);
        }
    }
    
    public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
    {
        if (_cloneToPreFabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);

            if (obj.transform.parent != parentObject.transform)
            {
                obj.transform.SetParent(parentObject.transform);
            }

            if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogWarning($"Object {obj.name} doesn't have a component of type {typeof(GameObject)}");
            }
        }
        else
        {
            Debug.LogWarning($"Trying to return object of type {obj.name} to {obj.transform.parent.name} pool");
        }
    }
    
}
