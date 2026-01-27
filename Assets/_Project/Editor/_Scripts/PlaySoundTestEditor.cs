using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaySoundTest))]
public class PlaySoundTestEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlaySoundTest playSoundTest = (PlaySoundTest)target;

        if (GUILayout.Button("Play Sound Effect"))
        {
            playSoundTest.PlaySoundEffect();
        }
        
        if (GUILayout.Button("Play OneShot Effect"))
        {
            playSoundTest.PlayOnShot();
        }
        
    }
    
    
    
}