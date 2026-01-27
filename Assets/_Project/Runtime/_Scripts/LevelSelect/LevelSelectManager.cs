using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{

    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public TMP_Text AreaHeaderText;
    public AreaData CurrentArea;

    public Image mapImage;
    
    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();

    
    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> _buttonPositions = new Dictionary<GameObject, Vector3>();

    [Header("Debugging")]
    [Tooltip("For debugging the position of the fog of war effect")] public bool isDebugging = false;

    private void Start()
    {
        AssignAreaInfo();
        LoadUnlockedLevels();
        CreateLevelButtons();
    }

    private void AssignAreaInfo()
    {
        AreaHeaderText.text = CurrentArea.AreaName;
        mapImage.sprite = CurrentArea.AreaImage;
    }

    private void LoadUnlockedLevels()
    {
        foreach (LevelData level in CurrentArea.Levels)
        {
            if (level.IsUnlocked)
                UnlockedLevelIDs.Add(level.LevelName);
        }
    }

    private void CreateLevelButtons()
    {
        for (int i = 0; i < CurrentArea.Levels.Count; i++)
        {
            GameObject buttonGo = Instantiate(LevelButtonPrefab, LevelParent);
            _buttonObjects.Add(buttonGo);
            buttonGo.name = CurrentArea.Levels[i].LevelName;
            
            RectTransform buttonRect = buttonGo.GetComponent<RectTransform>();
            buttonRect.localPosition = CurrentArea.Levels[i].Position;
            
            CurrentArea.Levels[i].LevelButtonObj = buttonGo;
            
            LevelButton levelbutton =  buttonGo.GetComponent<LevelButton>();
            levelbutton.Setup(CurrentArea.Levels[i], UnlockedLevelIDs.Contains(CurrentArea.Levels[i].LevelName));
            
        }
    }

    
    private void OnDrawGizmos()
    {
        if (!isDebugging) return;
        
        
        var rectTransform = LevelParent.GetComponent<RectTransform>();
        if(rectTransform == null) return;
        
        for (int i = 0; i < CurrentArea.Levels.Count; i++)
        {
            var area = CurrentArea.Levels[i];

            if (area.ShowDebug)
            {
                Vector3 buttonPos = LevelParent.TransformPoint(new Vector3(area.Position.x, area.Position.y, 0));
            //    Vector3 centPosition = LevelParent.TransformPoint(new Vector3(area.BoxCenterPos.x, area.BoxCenterPos.y, 0));
                
                Gizmos.color = Color.darkOrange;
                Gizmos.DrawSphere(buttonPos, 15f);
                
            //    Gizmos.color = Color.orangeRed;
            //    Gizmos.DrawSphere(centPosition, 15f);
            //    
            //    Gizmos.color = Color.orange;
            //    Gizmos.DrawWireCube(centPosition, new Vector3(area.Width, area.Height, 0.2f));
                
                
                Gizmos.color = Color.orange;
                Gizmos.DrawWireCube(buttonPos, new Vector3(area.Width, area.Height, 0.2f));
            }
        }
        
        
    }
}
