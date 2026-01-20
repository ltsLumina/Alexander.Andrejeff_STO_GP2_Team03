using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a struct for keeping values to store in a list or dic
/// </summary>
public struct PartComponent
{
    public int id;
    public string partName;
    
    public Vector3 localPosition;
    public Quaternion localRotation;

    public GameObject wholeObject;
}

[CreateAssetMenu(fileName = "Objects", menuName = "Scriptable_Objects/Objects")]
public class ObjectsSO : ScriptableObject
{
    // the prefab object with all the parts it should use
    public GameObject prefab;
    
    /// <summary>
    /// Creas a dic with all the child objects and values 
    /// </summary>
    /// <returns>a dictionary</returns>
    public Dictionary<string, PartComponent> GetPartsDic()
    {
        if (prefab == null) return null;
        var newDic = new Dictionary<string, PartComponent>();
        
        for(int i = 0; i < prefab.transform.childCount; i++)
        {
            GameObject part = prefab.transform.GetChild(i).gameObject;
            var newPart = new PartComponent();
            
            var partName = part.name;
            
            newPart.partName = partName;
            newPart.id = part.GetInstanceID();
            newPart.localPosition = part.transform.localPosition;
            newPart.localRotation = part.transform.localRotation;
            newPart.wholeObject = prefab;
            newDic.Add(part.name + Random.Range(0,100), newPart);
        }

        return newDic;
    }
    
}
