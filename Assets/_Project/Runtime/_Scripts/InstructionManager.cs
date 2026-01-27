using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;
using Lumina.Essentials.Modules.Singleton;
using UnityEditor;
using Random = UnityEngine.Random;

public class InstructionManager : Singleton<InstructionManager>
{
    [Tab("Config")]
    [SerializeField] Page pagePrefab;
    [Tooltip("Pages with specific modifiers to apply.")]
    [SerializeField] SerializedDictionary<Texture2D, PageModifier> curatedPages;
    
    [Header("State")]
    [SerializeField, ReadOnly] int currentPageIndex;
    [Tooltip("Prevents navigating pages if true.")]
    [SerializeField] bool lockNavigation;

    [Header("Options")]
    [Tooltip("Whether to scramble the order of the instruction pages.")]
    [SerializeField] bool scramble;

    [Tab("Debugging")]
    [Tooltip("The images exported from figma representing each page of the instructions.")]
    [ReadOnly, NonReorderable]
    [SerializeField] List<Texture2D> pageImages = new ();
    [ReadOnly, NonReorderable]
    [SerializeField] List<Page> pages = new ();
    [Tooltip("The pages to exclude when scrambling. E.g., the first and last pages. \nThese pages also will not receive random modifiers.")]
    [SerializeField] List<Texture2D> exclusions = new ();

    [Tab("References")]
    [SerializeField] VerticalLayoutGroup layoutGroup;
    [SerializeField] GameObject content;
    [SerializeField] InputActionAsset inputActionAsset;

    [Tab("Settings"), Tooltip("Will clone the first page this many times for testing purposes.")]
    [SerializeField] int debugPages;

    InputAction switchPageAction;

    /// <summary>
    /// Event invoked when a page is flipped.
    /// <para> Parameters: (int direction, int currentPageIndex)</para>
    /// </summary>
    event Action<int, int> OnPageFlippedEvent;

    public int CurrentPageIndex => currentPageIndex;

    public void SetLockNavigation(bool locked) => lockNavigation = locked;
    
    protected override void Awake()
    {
#if UNITY_EDITOR
        if (Application.isPlaying && Selection.activeObject == this)
        {
            return; // for whatever reason this marks the object as dirty in play mode, which throws errors
        }
#endif
        
        base.Awake();
        
        switchPageAction = inputActionAsset.FindAction("SwitchPage"); // TODO: use proper input system setup
        switchPageAction.started += SwitchPage;

        OnPageFlippedEvent += OnPageFlipped;
    }

    void OnEnable() => switchPageAction?.Enable();
    void OnDisable() => switchPageAction?.Disable();

    void Start()
    {
        int level = 1; // TODO: get actual level from game/level manager

        pageImages = Resources.LoadAll<Texture2D>($"Instruction Pages/Level {level}").ToList();
        pageImages.AddRange(Resources.LoadAll<Texture2D>("Instruction Pages/Bloat"));
        pageImages = pageImages.OrderBy(tex => tex.name).ToList();
        pageImages = scramble ? Scramble() : pageImages;

        #region Assertion / naming-convention check
        foreach (Texture2D page in pageImages)
        {
            if (!page.name.ToLowerInvariant().StartsWith("page_"))
            {
                if (page.name.ToLowerInvariant().StartsWith("bloat_")) continue;
                
                Debug.LogError($"Instruction page \"{page.name}\" is not named correctly. It should start with \"page_\" or \"bloat_\", followed by its number." 
                               + "\nE.g., \"Page_1\", \"Bloat_2\", etc.", page);
            }
        }
        #endregion

        #region Debug
        if (debugPages > 0 && pageImages.Count > 0)
        {
            Texture2D firstPageTexture = pageImages[0];
            pageImages.Clear();
            for (int i = 0; i < debugPages; i++) { pageImages.Add(firstPageTexture); }
        }
        #endregion

        GeneratePages();

        AlignLayout();

        var firstPage = pages.First();
        firstPage.SetCurrentPage();

        return;
        List<Texture2D> Scramble()
        {
            var texturesToScramble = pageImages;
            return texturesToScramble.Select((texture, index) => new { Texture = texture, index })
                             .OrderBy(x => exclusions.Contains(x.Texture) ? -1 : Random.Range(0, int.MaxValue)) // exclusions keep their order at start/end
                             .Select(x => x.Texture)
                             .ToList();
        }

        void GeneratePages()
        {
            layoutGroup.transform.DestroyAllChildren();

            for (int index = 0; index < pageImages.Count; index++)
            {
                Texture2D texture = pageImages[index];
                var page = Instantiate(pagePrefab, layoutGroup.transform, false);
                page.SetCurrentPage(false);
                pages.Add(page);

                var rawImage = page.GetComponent<RawImage>();
                rawImage.texture = texture;

                const float targetWidth = 960f;
                const float targetHeight = 1080f;
                rawImage.rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

                if (!exclusions.Contains(texture))
                {
                    LobbyState.InstructionGameMode gameMode = LobbyState.Instance != null ? LobbyState.Instance.GameMode : LobbyState.InstructionGameMode.Standard;
                    switch (gameMode)
                    {
                        case LobbyState.InstructionGameMode.Standard:
                            if (curatedPages.TryGetValue(texture, out var curatedModifier) && curatedModifier != null) 
                                curatedModifier.Apply(page);
                            break;

                        case LobbyState.InstructionGameMode.Randomized:
                            bool success = page.SetRandomModifier(out var newModifier);
                            if (success)
                            {
                                if (!modifierApplicationCounts.TryAdd(newModifier, 1)) modifierApplicationCounts[newModifier]++;

                                if (CanApply(page)) page.Modifier?.Apply(page);
                                else page.ClearModifier();
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                page.name = $"Page {index} | \"{texture.name}\" ({(page.Modifier != null ? page.Modifier.name : "None")})";
            }
        }

        void AlignLayout()
        {
            var rect = (RectTransform) layoutGroup.transform;
            const int topPositionOffset = 930; // required to align the top of the content with the top of the viewport
            rect.anchoredPosition = -Vector2.up * topPositionOffset;
        }

        bool CanApply(Page page)
        {
            if (page.Modifier == null) return false;
            return modifierApplicationCounts[page.Modifier] <= page.Modifier.MaxInstances;
        }
    }
    
    Dictionary<PageModifier, int> modifierApplicationCounts = new ();
    int previousDirection;
    Queue<int> queuedPages;
    Coroutine tweenQueueCoroutine;
    
    void SwitchPage(InputAction.CallbackContext ctx)
    {
        if (lockNavigation)
        {
            Debug.LogError("Navigation is locked. Cannot switch pages.");
            return;
        }

        int direction = (int)ctx.ReadValue<float>();
        
        if (tween.IsActive() && tween.IsPlaying() && direction != previousDirection) // if a tween is in progress and the direction is changing, queue the page flip
        {
            if (queuedPages is { Count: >= 1 }) return; // limit queue size to prevent excessive queuing (wont feel good)
            
            queuedPages ??= new ();
            previousDirection = direction;
            queuedPages.Enqueue(currentPageIndex + direction);
            Debug.Log($"Queued page index: {currentPageIndex + direction}");

            tweenQueueCoroutine ??= StartCoroutine(TweenQueue());
            return;
        }

        switch (direction)
        {
            case > 0:
                GoToNextPage();
                break;

            case < 0:
                GoToPreviousPage();
                break;
        }

        return;
        IEnumerator TweenQueue() // feels better to have a queue for multiple rapid inputs. Capped at 1 queued input, as too many feels bad.
        {
            while (queuedPages.Count > 0) 
            {
                yield return new WaitUntil(() => !tween.IsActive() || !tween.IsPlaying());
                var nextPageIndex = queuedPages.Dequeue();
                GoToPage(nextPageIndex);
                
                if (lockNavigation)
                {
                    tween.Kill();
                    queuedPages = null;
                    break; // if the page we arrive at locks navigation, stop processing the queue
                }
            }

            queuedPages = null;
            tweenQueueCoroutine = null;
        }
    }
    
    public int GoToNextPage()
    {
        if (tween.IsActive() && tween.IsPlaying()) return currentPageIndex;
        
        if (currentPageIndex < pageImages.Count - 1) currentPageIndex++;
        else currentPageIndex = 0; // last page goes to first page
        OnPageFlippedEvent?.Invoke(1, currentPageIndex);
        
        return currentPageIndex;
    }
    
    public int GoToPreviousPage()
    {
        if (tween.IsActive() && tween.IsPlaying()) return currentPageIndex;
        
        if (currentPageIndex > 0) currentPageIndex--;
        else currentPageIndex = pageImages.Count - 1; // first page goes to last page
        OnPageFlippedEvent?.Invoke(-1, currentPageIndex);
        
        return currentPageIndex;
    }
    
    /// <summary>
    /// Goes to the specified page index.
    /// </summary>
    /// <param name="pageIndex"> Zero-based index of the page to go to.
    /// <para>Entering -1 will go to a random page, while entering any out-of-range value will go to the last page. </para></param>
    public Page GoToPage(int pageIndex)
    {
        if (pageIndex == -1) pageIndex = Random.Range(0, pageImages.Count);                   // go to random page
        if (pageIndex < 0 || pageIndex >= pageImages.Count) pageIndex = pageImages.Count - 1; // out of range goes to last page

        int direction = pageIndex > currentPageIndex ? 1 : -1;
        currentPageIndex = pageIndex;
        OnPageFlippedEvent?.Invoke(direction, currentPageIndex);
        
        return GetCurrentPage();
    }
    
    public Page GoToPage(Page page)
    {
        int pageIndex = pages.IndexOf(page);
        return GoToPage(pageIndex);
    }

    TweenerCore<Vector2, Vector2, VectorOptions> tween;
    
    void OnPageFlipped(int direction, int _)
    {
        TweenCallback callback = () =>
        {
            var page = GetCurrentPage();
            page.SetCurrentPage();
        };
        
        tween = content.GetComponent<RectTransform>().DOAnchorPosY(1080 * currentPageIndex, 0.5f).SetEase(Ease.InOutQuad).OnComplete(callback);
        var previousPage = GetPageAtIndex(currentPageIndex - direction);
        if (previousPage != null) previousPage.SetCurrentPage(false);
    }
    
    public Page GetCurrentPage()
    {
        layoutGroup = FindAnyObjectByType<VerticalLayoutGroup>(); // unity breaks without this loloolollolol
        
        if (layoutGroup.transform.childCount == 0) return null;
        if (currentPageIndex < 0 || currentPageIndex >= layoutGroup.transform.childCount) return null; // out-of-range
        return layoutGroup.transform.GetChild(currentPageIndex).GetComponent<Page>();
    }
    
    public Page GetPageAtIndex(int index)
    {
        layoutGroup = FindAnyObjectByType<VerticalLayoutGroup>(); // unity breaks without this loloolollolol
        
        if (layoutGroup.transform.childCount == 0) return null;
        
        // if out of range, wrap around
        if (index < 0) index = layoutGroup.transform.childCount - 1;
        if (index >= layoutGroup.transform.childCount) index = 0;
        return layoutGroup.transform.GetChild(index).GetComponent<Page>();
    }
}
