using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VInspector;

public class InstructionManager : MonoBehaviour
{
    [Tab("Config")]
    [Tooltip("The images exported from figma representing each page of the instructions."), ReadOnly, NonReorderable]
    [SerializeField] List<Texture2D> pages = new ();
    [SerializeField] int currentPageIndex; // for future use

    [Tooltip("Whether to scramble the order of the instruction pages.")]
    [SerializeField] bool scramble;
    [Tooltip("The pages to exclude when scrambling. E.g., the first and last pages.")]
    [SerializeField] List<int> excludedPages = new () { 0 };

    [Tab("References")]
    [SerializeField] VerticalLayoutGroup layoutGroup;

    [Tab("Settings"), Tooltip("Will clone the first page this many times for testing purposes.")]
    [SerializeField] int debugPages;
    
    void Start()
    {
        pages = Resources.LoadAll<Texture2D>("Instruction Pages").ToList();
        pages = pages.OrderBy(tex => tex.name).ToList();
        pages = scramble ? Scramble() : pages;

        #region Assertion / naming-convention check
        foreach (Texture2D page in pages)
        {
            Debug.Assert(page.name.ToLowerInvariant().StartsWith("page_"), $"Instruction page \"{page.name}\" is not named correctly. It should start with \"Page \" followed by its number." + 
                                                                           "\nE.g., \"Page 1\", \"Page 2\", etc.");
        }
        #endregion

        #region Debug
        if (debugPages > 0 && pages.Count > 0)
        {
            Texture2D firstPage = pages[0];
            pages.Clear();
            for (int i = 0; i < debugPages; i++)
            {
                pages.Add(firstPage);
            }
        }
        #endregion
        
        layoutGroup.transform.DestroyAllChildren();
        for (int index = 0; index < pages.Count; index++)
        {
            Texture2D texture = pages[index];
            var go = new GameObject("Instruction Page", typeof(RectTransform), typeof(RawImage), typeof(Mask)); // TODO: Make prefab :P
            var page = go.AddComponent<Page>();
            
            go.transform.SetParent(layoutGroup.transform, false);

            var rawImage = go.GetComponent<RawImage>();
            rawImage.texture = texture;

            const float targetWidth = 960f;
            const float targetHeight = 1080f;
            rawImage.rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

            if (!excludedPages.Contains(index)) // only set random modifier if not excluded
                page.SetRandomModifier();

            go.name = $"Page {index} | \"{texture.name}\" ({(page.Modifier != null ? page.Modifier.name : "None")})";
            
            page.Modifier?.Apply(page.gameObject);
        }

        RectTransform rect = (RectTransform) layoutGroup.transform;
        const int topPositionOffset = 930; // required to align the top of the content with the top of the viewport
        rect.anchoredPosition = -Vector2.up * topPositionOffset;
    }

    List<Texture2D> Scramble()
    {   
        return pages
            .Select((page, index) => new { page, index })
            .OrderBy(x => excludedPages.Contains(x.index) ? int.MinValue : UnityEngine.Random.Range(int.MinValue, int.MaxValue))
            .Select(x => x.page)
            .ToList();
    }
}
