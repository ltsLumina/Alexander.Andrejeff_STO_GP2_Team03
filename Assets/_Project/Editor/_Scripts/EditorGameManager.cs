using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class EditorGameManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GameManager gameManager = (GameManager)target;
        if (GUILayout.Button("Calculate Score"))
        {
            Debug.Log($"Score: {gameManager.ValidationSystem.CalculateScore()}");
        }
    }
}