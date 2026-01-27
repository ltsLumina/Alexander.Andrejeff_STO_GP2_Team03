using System.Collections.Generic;
using UnityEngine;

public class FogMaskController : MonoBehaviour
{
    [SerializeField] private RectTransform fogRect;
    [SerializeField] private GameObject fogMaskPrefab;

    [SerializeField] private LevelSelectInfo[] levelSelectInfoArray;
    [SerializeField] private int unlockedLevels = 0;


    private void Start()
    {
        if (levelSelectInfoArray == null || levelSelectInfoArray.Length == 0) return;
        
        BuildFog();
    }

    private void BuildFog()
    {
        ClearFog();

        foreach (var levelInfo in levelSelectInfoArray)
        {
            if(levelInfo.levelIndex > unlockedLevels) continue;

            CreateReveal(levelInfo);


        }
    }

    private void CreateReveal(LevelSelectInfo levelInfo)
    {
        GameObject fogMask = Instantiate(fogMaskPrefab, fogRect);
        
        RectTransform fogMaskTransform = fogMask.GetComponent<RectTransform>();
        
        fogMaskTransform.anchorMin = fogMaskTransform.anchorMax = new Vector2(0.5f, 0.5f);
        fogMaskTransform.pivot = new Vector2(0.5f, 0.5f);
        
        fogMaskTransform.anchoredPosition = transform.position + levelInfo.centerPosition;
        fogMaskTransform.sizeDelta = new Vector2(levelInfo.Width, levelInfo.Height);
        fogMaskTransform.localScale = Vector3.one;
        fogMaskTransform.localRotation = Quaternion.identity;
    }


    private void ClearFog()
    {
        for (int i = 0; i < fogRect.childCount; i++)
        {
            Destroy(fogRect.GetChild(i).gameObject);
        }
    }
    
    
    
    public void OnDrawGizmosSelected()
    {
        if (levelSelectInfoArray.Length <= 0) return;

        for (int i = 0; i < levelSelectInfoArray.Length; i++)
        {
            var room = levelSelectInfoArray[i];

            Gizmos.color = Color.orangeRed;
            Gizmos.DrawSphere(transform.position + room.centerPosition, 20);

            Gizmos.color = Color.darkOrange;
            Gizmos.DrawWireCube(transform.position + room.centerPosition, new Vector3(room.Width, room.Height, 2f));


        }
        
        
    }
    
    
    
}

