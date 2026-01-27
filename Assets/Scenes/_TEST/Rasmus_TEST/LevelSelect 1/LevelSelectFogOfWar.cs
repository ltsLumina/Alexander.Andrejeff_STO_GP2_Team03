using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public struct LevelSelectInfo
{
    [Header("Level unlock area")]
    [Tooltip("The level that the room unlocks in.")]    public int levelIndex;
    [Tooltip("Rooms center position.")]                 public Vector3 centerPosition;
    [Tooltip("The height of the room.")]                public int Height;
    [Tooltip("The width of the room.")]                 public int Width;
}


public class LevelSelectFogOfWar : MonoBehaviour
{
    
    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public TMP_Text AreaHeaderText;
    public TMP_Text LevelHeaderText;
    public AreaData CurrentArea;
    public GameObject FogOfWarPrefab;
    
    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();

    private Camera _camera;
    
    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> _buttonPositions = new Dictionary<GameObject, Vector3>();

    [Header("Debugging")]
    [Tooltip("For debugging the position of the fog of war effect")] public bool isDebugging = false;
    
    
    [Header("Masking")]
    public Canvas mapCanvas;
    public Image mapSprite;
    private Image mapMaskParent;
    private Texture2D mapMaskTexture;
    
    [field:SerializeField] public List<LevelSelectInfo>  levelSelectInfoArray =  new List<LevelSelectInfo>();
    
    private HashSet<Vector2> maskablePositions = new HashSet<Vector2>();

    

    public void Awake()
    {
        if (levelSelectInfoArray.Count <= 0 || mapCanvas == null)
        {
            Debug.LogWarning("No level select info Data is found or no map canvas has been set.");
            return;
        } 
        
        _camera = Camera.main;
        
        mapSprite = mapCanvas.GetComponentInChildren<Image>();


       // mapMaskTexture = CreateMaskTexture(mapSprite.sprite.texture);
        
        var s = CreateImageMaskCanvasObject();

    //    foreach (var levelInfo in levelSelectInfoArray)
    //    {
    //        mapMaskTexture = MaskOutMapArea(mapMaskTexture, levelInfo);
    //        
    //    }
        
       // s.GetComponent<Image>().sprite = Sprite.Create(mapMaskTexture, new Rect(0, 0, mapMaskTexture.width, mapMaskTexture.height), Vector2.zero);
        
    }

    
    
    private GameObject CreateImageMaskCanvasObject()
    {
        GameObject newImageObject = new GameObject("LevelMask");
        newImageObject.transform.SetParent(mapCanvas.transform);
        
        mapMaskParent = newImageObject.AddComponent<Image>();
        mapMaskParent.sprite = Sprite.Create(mapMaskTexture, new Rect(0, 0, mapMaskTexture.width, mapMaskTexture.height), Vector2.zero);
        
        var recTransform = newImageObject.GetComponent<RectTransform>();
        recTransform.anchorMin = Vector2.zero;
        recTransform.anchorMax = Vector2.one;
        recTransform.pivot = new Vector2(0.5f, 0.5f);
        recTransform.sizeDelta = Vector2.zero;
        recTransform.offsetMin = Vector2.zero;
        recTransform.offsetMax = Vector2.zero;

        return newImageObject;
    }

//    private Texture2D CreateMaskTexture(Texture2D originalSprite)
//    {
//        
//        int width = originalSprite.width;
//        int height = originalSprite.height;
//        
//        Texture2D newMaskTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
//
//        HashSet<Vector3> dsa = GetGrayedOutPixelsd();
//        
//        Debug.Log(dsa.Count);
//        
//
//        for (int y = 0; y < height; y++)
//        {
//            for (int x = 0; x < width; x++)
//            {
//                Color color = new Color(0,0,0,0);
//                if(dsa.Contains(new Vector3(x, y, 0)))
//                    color = new Color(1, 1, 1, 0.5f);
//                newMaskTexture.SetPixel(x,y, color);
//            }
//        }
//        
//        
//        return newMaskTexture;
//    }
//
//    private HashSet<Vector2> GetGrayedOutPixels()
//    {
//        HashSet<Vector2> grayedOutArea = new HashSet<Vector2>();
//        
//        if(mapSprite == null || mapSprite.sprite == null) return grayedOutArea;
//        
//        
//        Texture2D texture = mapSprite.sprite.texture;
//        RectTransform mapRect = mapSprite.rectTransform;
//        
//        var texWidth = texture.width;
//        var texHeight = texture.height;
//        
//        Debug.Log(CurrentArea.Levels.Count);
//        
//        for (int i = 0; i < CurrentArea.Levels.Count; i++)
//        {
//            var area = CurrentArea.Levels[i];
//
//            if (area.IsUnlocked) continue;
//            
//            // get the area by Vector3[] of cornerPieces add add to pixel placement
//            Vector3[] corners = area.cornerPices;
//            
//            
//            Vector2[] pixelCorners = new Vector2[corners.Length];
//            for (int pC = 0; pC < corners.Length; pC++)
//            {
//                Vector3 local = mapRect.InverseTransformPoint(corners[pC]);
//                float pX = (local.x + mapRect.rect.width * 0.5f) / mapRect.rect.width * texWidth;
//                float pY = (local.y + mapRect.rect.height * 0.5f) / mapRect.rect.height * texHeight;
//                pixelCorners[pC] = new Vector2(pX, pY);
//            }
//            
//            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
//            foreach (Vector2 pC in pixelCorners)
//            {
//                if(pC.x < minX) minX = pC.x;
//                if(pC.x > maxX) maxX = pC.x;
//                if(pC.y < minY) minY = pC.y;
//                if(pC.y > maxY) maxY = pC.y;
//            }
//
//
//            for (int x = Mathf.FloorToInt(minX); x < Mathf.CeilToInt(maxX); x++)
//            {
//                for (int y = Mathf.FloorToInt(minY); y < Mathf.CeilToInt(maxY); y++)
//                {
//                    Vector2 p = new Vector2(x+0.5f, y+0.5f);
//                    if (PointInPolygon(p, pixelCorners))
//                        grayedOutArea.Add(new Vector2(x, y));
//
//                }
//            }
//            
//        }
//        return grayedOutArea;
//    }
//

//private HashSet<Vector3> GetGrayedOutPixelsd()
//{
//    HashSet<Vector3> grayedOutPixels = new HashSet<Vector3>();
//
//    if (mapSprite == null || mapSprite.sprite == null) return grayedOutPixels;
//
//    Texture2D texture = mapSprite.sprite.texture;
//    RectTransform mapRect = mapSprite.rectTransform;
//
//    int texWidth = texture.width;
//    int texHeight = texture.height;
//
//    for (int i = 0; i < CurrentArea.Levels.Count; i++)
//    {
//        var area = CurrentArea.Levels[i];
//
//        if (area.IsUnlocked) continue; // skip unlocked
//
//        Vector3[] corners = area.cornerPices;
//
//        // Convert corners from world/local space to texture pixel space
//        Vector2[] pixelCorners = new Vector2[corners.Length];
//        for (int c = 0; c < corners.Length; c++)
//        {
//            Vector3 local = mapRect.InverseTransformPoint(corners[c]);
//            float px = (local.x + mapRect.rect.width * 0.5f) / mapRect.rect.width * texWidth;
//            float py = (local.y + mapRect.rect.height * 0.5f) / mapRect.rect.height * texHeight;
//            pixelCorners[c] = new Vector2(px, py);
//        }
//
//        // Get bounding box
//        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
//        foreach (var p in pixelCorners)
//        {
//            if (p.x < minX) minX = p.x;
//            if (p.x > maxX) maxX = p.x;
//            if (p.y < minY) minY = p.y;
//            if (p.y > maxY) maxY = p.y;
//        }
//
//        // Iterate over bounding box
//        for (int x = Mathf.FloorToInt(minX); x <= Mathf.CeilToInt(maxX); x++)
//        {
//            for (int y = Mathf.FloorToInt(minY); y <= Mathf.CeilToInt(maxY); y++)
//            {
//                Vector2 p = new Vector2(x + 0.5f, y + 0.5f); // center of pixel
//                if (PointInPolygon(p, pixelCorners))
//                {
//                    grayedOutPixels.Add(new Vector3(x, y, 0));
//                }
//            }
//        }
//    }
//
//    return grayedOutPixels;
//}
//
    
    
    private bool PointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool inside = false;
        int j = polygon.Length - 1;
        for (int i = 0; i < polygon.Length; i++)
        {
            if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) +
                    polygon[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
        
    }

    private Texture2D MaskOutMapArea(Texture2D texture, LevelSelectInfo areaInfo)
    {
        RectTransform mapRect = mapSprite.rectTransform;
        Vector2 normalizedCenter = Rect.PointToNormalized(mapRect.rect, areaInfo.centerPosition);
        
        int centerX = Mathf.RoundToInt(normalizedCenter.x * texture.width);
        int centerY = Mathf.RoundToInt(normalizedCenter.y * texture.height);
        
        int halfW = Mathf.RoundToInt((areaInfo.Width / mapRect.rect.width) * texture.width * 0.5f);
        int halfH = Mathf.RoundToInt((areaInfo.Height / mapRect.rect.height) * texture.height * 0.5f);

        for (int y = centerY-halfH; y < centerY + halfH; y++)
        {
            for (int x = centerX-halfW; x < centerX + halfW; x++)
            {
                if(x < 0 || x >= texture.width || y < 0 || y >= texture.height) continue;
                
                texture.SetPixel(x,y, new Color(1,1,1,0));
            }
        }
        
        texture.Apply();
        return texture;
    }


    #region Set up Buttons
    
    private void Start()
    {
        AssignAreaText();
        LoadUnlockedLevels();
        CreateLevelButtons();
    }

    private void AssignAreaText()
    {
        AreaHeaderText.text = CurrentArea.AreaName;
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

    #endregion

    public void OnDrawGizmos()
    {
        if (levelSelectInfoArray.Count <= 0 || mapSprite == null) return;

        
        RectTransform mapRect = mapSprite.rectTransform;
        Texture2D texture = mapSprite.sprite.texture;
        
        for (int i = 0; i < levelSelectInfoArray.Count; i++)
        {
            var room = levelSelectInfoArray[i];
            
            Vector3 worldCenterPos = mapRect.TransformPoint(room.centerPosition);

            Vector3 worldSize = new Vector3(room.Width * mapRect.localScale.x,
                room.Height * mapRect.localScale.y, 2f);


        //    for (int j = 0; j < CurrentArea.Levels.Count; j++)
        //    {
        //        var area = CurrentArea.Levels[j];
        //        
        //        Gizmos.DrawLineStrip(new ReadOnlySpan<Vector3>(area.cornerPices), true);
        //        
        //        
        //    }
            
            

            Gizmos.color = Color.orangeRed;
            Gizmos.DrawSphere(worldCenterPos, 20);

            Gizmos.color = Color.darkOrange;
            Gizmos.DrawWireCube(worldCenterPos, worldSize);


        }
        
        
    }
}
