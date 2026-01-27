using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawningInParts : MonoBehaviour
{

    [Header("Build Object")]
    public GameObject buildingObject;
    
    [Header("Part Placement Area")] 
    public Vector3 centerPos = new Vector3(0, 0, 0);
    public Vector3 areaSize = new Vector3(5f, 1f, 5f);
    public float padding = 0.3f;

    [Header("Spawning Directions")]
    [SerializeField] private bool constrainToX = true;
    [SerializeField] private bool constrainToZ = false;
    [SerializeField] private bool constrainToY = false;
    
    

    private List<GameObject> objectParts = new List<GameObject>();
    
    private void Start()
    {
        if (buildingObject == null) return;


        var dad = Instantiate(buildingObject);
        dad.transform.position = transform.position;
        dad.transform.rotation = new Quaternion(180, 0, 0, 1);


        for (int i = 0; i < dad.transform.childCount; i++)
        {
            objectParts.Add(dad.transform.GetChild(i).gameObject);
        }
        
        
        PlacePartsInArea(objectParts);
        
    }


    
    private void PlacePartsInArea(List<GameObject> parts)
    {
        int partAmount = parts.Count;
        
        float xMargin = (areaSize.x-padding*2)/partAmount;
        float xLeft = -(areaSize.x-padding)/2;
        float zMargin = (areaSize.z-padding*2)/partAmount;
        float zLeft = -(areaSize.z-padding)/2;
        float yMargin = (areaSize.y-padding*2)/partAmount;
        float yLeft = -(areaSize.y-padding)/2;

        for (int i = 1; i < parts.Count; i++)
        {
            Vector3 newPos = Vector3.zero;
            if (constrainToX)
            {
                xLeft += xMargin;
                newPos.x = xLeft;
            }
            
            if (constrainToZ)
            {
                zLeft += zMargin;
                newPos.z = zLeft;
            }
            
            if (constrainToY)
            {
                yLeft += yMargin;
                newPos.y = yLeft;
            }
            
            parts[i].transform.position = newPos + centerPos;
            
            //TODO:
            // Rotate object 90 degree 
            //
            
            var dsa = Vector3.Angle(parts[i].transform.up, parts[i].transform.forward);
            parts[i].transform.rotation = Quaternion.AngleAxis(dsa, Vector3.right);
            Debug.Log(dsa);
            
            
            
            


        }
    }
    
}
