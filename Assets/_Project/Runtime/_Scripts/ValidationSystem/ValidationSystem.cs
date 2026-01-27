using System.Collections.Generic;
using UnityEngine;

public class ValidationSystem : MonoBehaviour
{

    public float debuggingPercentage;

    private Dictionary<string, Transform> buildRotation = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> ghostRotation = new Dictionary<string, Transform>();

    private HashSet<string> checkedClosedParts = new HashSet<string>();
    
    // The player built thing
    public GameObject BuildParts => GameManager.Instance.BuildPartsParent;
    // The original building, and later ghost from
    public GameObject GhostParts => GameManager.Instance.GhostPartsParent;

    private void InitializeDictionary()
    {
        // Get the original rotations of all the parts and keeps them in a dictionary, for comparison later when calculating score
        for (int i = 0; i < GhostParts.transform.childCount; i++)
        {
            ghostRotation.TryAdd(BuildParts.transform.GetChild(i).name, GhostParts.transform.GetChild(i).transform);
           // ghostParts.transform.GetChild(i).gameObject.SetActive(false);
        }

        // Get the current rotation of the placed parts and add to a Dictionary
        for (int i = 0; i < BuildParts.transform.childCount; i++)
        {
            buildRotation.TryAdd(BuildParts.transform.GetChild(i).name, BuildParts.transform.GetChild(i).transform);
        }
    }
    
    /// <summary>
    /// Checking the rotation of the part with DOT product and if they are facing the right way
    /// </summary>
    /// <param name="part">The part that should be checked</param>
    /// <param name="ghostPart">The part that should be be compared to; ghostpart is the "base Object" that the player should match</param>
    /// <returns>float between [0,1]</returns>
    private static float GetRotationOffsets(Transform part, Transform ghostPart)
    {
        var dot = Vector3.Dot(ghostPart.up, part.transform.up);
        return 1 + dot;
    }

    /// <summary>
    /// Checks the Position from ghost parts to the closest build part and compares distance to calculate score
    /// </summary>
    /// <returns>Percentage value [0,1]</returns>
    private float GetPositionOffsetScore()
    {
        float sumDistance = 0;
        // Loops through ghost parts to see if a part is missing or something, IDK???
        foreach (var ghostPosKey in ghostRotation)
        {
            // stores temp values for checking the closest build parts
            string name = "";
            float cloeset = 9999999;
            Vector3 ghostPos = ghostPosKey.Value.position;
            
            // Loops through the build pieces for distance comparison
            foreach (var buildPosKey in buildRotation)
            {
                if(checkedClosedParts.Contains(buildPosKey.Key)) continue;
                
                Vector3 buildPos =  buildPosKey.Value.position;
                
                float distance = Vector3.Distance(buildPos, ghostPos);
                
                if(distance >= cloeset ) continue;
                
                name = buildPosKey.Key;
                cloeset = distance;
            }
            checkedClosedParts.Add(name);
            sumDistance -= cloeset - 0.7f;
        }
        
        return (sumDistance/ghostRotation.Count) +1;
    }

    /// <summary>
    /// Calculates score based on the correct angle towards said part,
    /// </summary>
    /// <param name="partList">The player placed building</param>
    /// <param name="ghostPartList">The original building</param>
    /// <returns>Score value</returns>
    public float CalculateScore()
    {
        InitializeDictionary();
        float totalScore = 0;
        
        var posScore = GetPositionOffsetScore();
        Debug.Log(posScore);
        
        foreach (string ds in buildRotation.Keys)
        {
            totalScore += GetRotationOffsets(buildRotation[ds], ghostRotation[ds]);
        }

        float totalRotation = (totalScore / buildRotation.Count) - 1;
        return (posScore+totalRotation)/2;
    }
    
}
