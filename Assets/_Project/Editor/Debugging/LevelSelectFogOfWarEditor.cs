using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelSelectFogOfWar))]
public class LevelSelectFogOfWarEditor : Editor
{
 //   private void OnSceneGUI()
 //   {
 //       var map =  (LevelSelectFogOfWar)target;

 //       if (map.CurrentArea == null) return;

 //       Handles.color = Color.cyan;
 //       
 //       for (int j = 0; j < map.CurrentArea.Levels.Count; j++)
 //       {
 //           var area = map.CurrentArea.Levels[j];

 //           for (int i = 0; i < area.cornerPices.Length; i++)
 //           {
 //               EditorGUI.BeginChangeCheck();

 //               var fmh_26_21_639050390302278600 = Quaternion.identity; Vector3 newPos = Handles.FreeMoveHandle(
 //                   area.cornerPices[i],
 //                   10f,
 //                   Vector3.zero,
 //                   Handles.SphereHandleCap
 //               );

 //               if (EditorGUI.EndChangeCheck())
 //               {
 //                   Undo.RecordObject(map, "Move Corner Point");
 //                   area.cornerPices[i] = newPos;
 //                   EditorUtility.SetDirty(map);
 //               }
 //           }

 //           // draw lines between corners
 //           Handles.DrawAAPolyLine(area.cornerPices);
 //       }
 //   }
}//