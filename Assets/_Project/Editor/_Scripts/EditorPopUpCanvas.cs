using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PopUpCanvas))]
public class EditorPopUpCanvas : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Trigger Test Pop-Up"))
        {
            PopUpCanvas.TriggerPopUpMessage("This is a test pop-up message from the Editor!", PopUpAlignment.Top, PopUpType.Warning);
        }
    }
}