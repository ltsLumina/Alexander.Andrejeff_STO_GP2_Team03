using UnityEditor;
using UnityEngine;

// for adding buttons for debugging
[CustomEditor(typeof(ValidationSystem))]
public class ValidationSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var myTarget = (ValidationSystem)target;


        if (GUILayout.Button("Print Parts"))
        {
            myTarget.PrintPartsDic();
        }

        if (GUILayout.Button("Print Parts Current Position"))
        {
            myTarget.PrintPartsCurrentPosition();
        }
        
        if (GUILayout.Button("Validate positions"))
        {
            myTarget.ValidateParts();
        }
        
    }
    
    
}
