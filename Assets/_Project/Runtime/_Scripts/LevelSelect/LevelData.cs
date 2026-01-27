using System;
using UnityEngine;




[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable_Objects/LevelData")]
public class LevelData : ScriptableObject
{

    [Header("Button Info")]
    public int LevelID;
    public string SceneName;
    public string LevelName;
    public Vector2 Position;

    
    [Header("Initialized Values")]
    public bool IsUnlocked = false;
    public GameObject LevelButtonObj {get; set;}


    [Header("Fog of War effect V1")] 
    public bool ShowDebug = false;
    public int Width;
    public int Height;
    //public Vector2 BoxCenterPos;
}
