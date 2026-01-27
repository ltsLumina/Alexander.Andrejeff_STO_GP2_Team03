using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaData", menuName = "Scriptable_Objects/AreaData")]
public class AreaData : ScriptableObject
{
    [Header("Level area Info")]
    public string AreaName;
    public Sprite AreaImage;
    public List<LevelData> Levels = new List<LevelData>();
}
