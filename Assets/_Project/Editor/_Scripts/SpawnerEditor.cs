using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;



[CustomEditor(typeof(SpawningInParts))]
public class SpawnerEditor : Editor
{

    private BoxBoundsHandle _boundsHandle = new BoxBoundsHandle();

    protected virtual void OnSceneGUI()
    {
        SpawningInParts spawningInParts = target as SpawningInParts;

        _boundsHandle.center = spawningInParts.centerPos;
        _boundsHandle.size = spawningInParts.areaSize;
        
        EditorGUI.BeginChangeCheck();
        _boundsHandle.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spawningInParts, "Modified spawning parts area");
            spawningInParts.areaSize = _boundsHandle.size;
        }
        
    }


}
