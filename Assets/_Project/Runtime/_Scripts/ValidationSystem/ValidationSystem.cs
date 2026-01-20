using System.Collections.Generic;
using UnityEngine;


public struct PartValues
{
    public Vector3 position;
    public Quaternion rotation;
}



public class ValidationSystem : MonoBehaviour
{


    [Header("Scoring")]
    [SerializeField] private float BaseScore = 100f;

    public float DebuginggotScore;
    
    // gets the Assebly object to check for the level
    public GameObject prefabObject;
    
    // mabye use Dictionary instead with int or HashSets, same as the List
    public Dictionary<string, PartComponent> partsDic;
    private Dictionary<string, List<Vector3>> testPositions = new Dictionary<string, List<Vector3>>();
    private Dictionary<string, List<PartValues>> partValuesStuff = new Dictionary<string, List<PartValues>>();

    private Dictionary<string, Transform> buildRotation = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> ghostRotation = new Dictionary<string, Transform>();

    public GameObject buildParts;
    public GameObject ghostParts;
    
    
    
    private void Start()
    {
        var objectInstantiate = Instantiate(prefabObject);
        objectInstantiate.transform.position += Vector3.forward*20;

        for (int i = 0; i < buildParts.transform.childCount; i++)
        {
            buildRotation.Add(buildParts.transform.GetChild(i).name, buildParts.transform.GetChild(i).transform);
        }
        
        for (int i = 0; i < ghostParts.transform.childCount; i++)
        {
            ghostRotation.Add(buildParts.transform.GetChild(i).name, ghostParts.transform.GetChild(i).transform);
           // ghostParts.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        DebuginggotScore = CalculateScore(buildRotation, ghostRotation);
        
        GetPartsDic();


    //    if (partValuesStuff.TryGetValue("4x6", out var value))
    //    {
    //        foreach (var item in value)
    //        {
    //            Debug.Log($"Position: {item.position}");
    //            Debug.Log($"Rotation: {item.rotation}");
    //        }
    //    }
    }

    
    /// <summary>
    /// Checking the rotation of the part with DOT product and if they are facing the right way
    /// </summary>
    /// <param name="part">The part that should be checked</param>
    /// <param name="ghostPart">The part that should be be compared to; ghostpart is the "base Object" that the player should match</param>
    /// <returns>float between [0,1]</returns>
    private float GetAllRotationOffsets(Transform part, Transform ghostPart)
    {
        float totalAngleDiff = 0;
        
        var dot = Vector3.Dot(ghostPart.up, part.transform.up);
        // change to pos
        if (dot < 0) dot *= -1;

        totalAngleDiff += dot;

        Debug.Log(part.name);
        Debug.Log(dot);
        
        return totalAngleDiff;
    }

    private float CalculateScore(Dictionary<string, Transform> partList, Dictionary<string, Transform> ghostPartList)
    {
        float totalScore = 0;
        foreach (string ds in partList.Keys)
        {
            totalScore += BaseScore * GetAllRotationOffsets(partList[ds], ghostPartList[ds]);
        }
        return totalScore;
    }
    
    
    //TODO: 
    // Get all the parts and check a value to store as an index for a dictionary 
    // the Key should be the part index/name 
    // the Values should be the correct transform and rotation for a valid placement 
    private void GetPartsDic()
    {
        if (prefabObject == null) return;
        
        for(int i = 0; i < prefabObject.transform.childCount; i++)
        {
            GameObject part = prefabObject.transform.GetChild(i).gameObject;
            var component = new PartValues();

            component.position = part.transform.localPosition;
            component.rotation = part.transform.localRotation;
            
            if(!partValuesStuff.ContainsKey(part.name))
                partValuesStuff.Add(part.name, new List<PartValues>());
            
            partValuesStuff[part.name].Add(component);
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    /// <summary>
    /// Prints the dic with its values for easier debuging
    /// </summary>
    public void PrintPartsDic()
    {
        if (partsDic == null) {Debug.LogWarning("No parts found"); return; }
        
        foreach (var part in partsDic)
        {
            Debug.Log($"ID/Key: {part.Key}");
            Debug.Log($"Name: {part.Value.partName}");
            Debug.Log($"Transform: {part.Value.localPosition}");
            Debug.Log($"Rotation: {part.Value.localRotation}");
            Debug.Log($"Whole Object: {part.Value.wholeObject}");
        }
    }
    
    /// <summary>
    /// Searches for all the parts from the dic/list and checks the transform
    /// </summary>
    public void PrintPartsCurrentPosition()
    {
        if (partsDic == null) {Debug.LogWarning("No parts found"); return; }

        // Somehow check the id per part and check said transform
        foreach (var part in partsDic)
        {
            Debug.Log($"ID: {part.Key}");
            Debug.Log($"Name: {part.Value.partName}");
            var currentPos = GameObject.Find(part.Key.ToString());
            if(currentPos == null) {Debug.LogWarning("No parts found"); return; }
            Debug.Log($"Current local Position: {part.Value.localPosition}");
            Debug.Log($"Current local Rotation: {part.Value.localRotation}");
        }

    }
    
    /// <summary>
    /// The validation part of the code, check how far or the rotation of part with the original pos
    /// </summary>
    public void ValidateParts()
    {
        if (partsDic == null) {Debug.LogWarning("No parts found"); return; }
        int score = 0;
        
        foreach (var part in partsDic)
        {
            Debug.Log($"<Color=red>ID: {part.Key}</Color>");
            
            Vector3 orgPos = part.Value.localPosition;
            
            var worldPartObject = GameObject.Find(part.Value.id.ToString());
            if(worldPartObject == null) {Debug.LogWarning("No parts found"); return; }

            Debug.Log($"Current local Position: {worldPartObject.transform.localPosition}");
            Debug.Log($"Current world Position: {worldPartObject.transform.position}");
            
            var distanceTo =  worldPartObject.transform.localPosition - orgPos;
            
            Debug.Log($"Distance: {distanceTo.magnitude}");
            
            // Temp scoring value
            if(distanceTo.magnitude <= 0.2f)
                score++;
        }
        
        Debug.Log($"PartAmount: {partsDic.Count}");
        Debug.Log($"Score: {score}");
    }
}
