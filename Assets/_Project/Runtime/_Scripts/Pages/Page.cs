#region
using System;
using JetBrains.Annotations;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;
#endregion

public class Page : MonoBehaviour
{
	[SerializeField] Texture2D texture;
	[SerializeField] PageModifier modifier;

	int pageIndex => transform.GetSiblingIndex();

	[field: SerializeField, ReadOnly] // this works as of 6.3 btw :) -- only for auto-properties
	public bool IsCurrentPage { get; private set; }

	public event Action<PageModifier> BecameActive;

	public void SetCurrentPage(bool active = true)
	{
		IsCurrentPage = active;
		if (active) BecameActive?.Invoke(modifier);
	}

#if UNITY_EDITOR
	[Button, UsedImplicitly]
	void Stain() => SetModifier(Resources.Load<PageModifier>("Page Modifiers/Stained"), true, true);

	[Button] [UsedImplicitly]
	void Rip() => SetModifier(Resources.Load<PageModifier>("Page Modifiers/Ripped"), true, true);

	[Button] [UsedImplicitly]
	void Tape() => SetModifier(Resources.Load<PageModifier>("Page Modifiers/Taped"), true, true);

	[Button] [UsedImplicitly]
	void Advertisement()
	{
		SetModifier(Resources.Load<PageModifier>("Page Modifiers/Advertisement"), true, true);
		if (IsCurrentPage) transform.GetChild(0).gameObject.SetActive(true);
	}
	
	[Button] [UsedImplicitly]
	void Paywall() => SetModifier(Resources.Load<PageModifier>("Page Modifiers/Paywall"), true, true);

	/// <summary>
	/// Sets the page modifier for debugging purposes.
	/// </summary>
	/// <param name="newModifier"></param>
	/// <param name="overwrite"> Overwrite existing modifier if any. (Destroys all children!!) </param>
	/// <param name="applyImmediately"> Applies the modifier immediately, otherwise you must call Apply() manually. </param>
	void SetModifier(PageModifier newModifier, bool overwrite = true, bool applyImmediately = false)
	{
		if (overwrite) transform.DestroyAllChildren();
		modifier = newModifier;
		if (applyImmediately) modifier?.Apply(this);

		name = $"Page {pageIndex} | \"{texture.name}\" ({(modifier != null ? modifier.name : "None")})";
	}
#endif
	
	public void SetModifier(PageModifier newModifier)
	{
		transform.DestroyAllChildren();
		modifier = newModifier;
	}

	/// <summary>
	///     Clears the current page modifier.
	///     <remarks> Destroys all children!! </remarks>
	/// </summary>
	public void ClearModifier()
	{
		transform.DestroyAllChildren();
		modifier = null;
	}

	public PageModifier Modifier => modifier;

	public bool SetRandomModifier(out PageModifier newModifier)
	{
		PageModifier[] modifiers = Resources.LoadAll<PageModifier>("Page Modifiers");

		PageModifier randomModifier = modifiers.Length > 0 ? modifiers[Random.Range(0, modifiers.Length)] : null;

		if (randomModifier?.ModifierChance < Random.value)
		{
			newModifier = null;
			return false;
		}

		modifier = randomModifier;
		newModifier = modifier;
		return true;
	}
}

public static class PageExtensions
{
	public static Vector2 GetPageBounds(this Page page, bool includeMargin = true)
	{
		const float marginWidth = 100f;
		const float marginHeight = 100f;

		var rectTransform = page.GetComponent<RectTransform>();
		var width = rectTransform.rect.width;
		var height = rectTransform.rect.height;

		if (includeMargin) return new Vector2(width / 2f - marginWidth, height / 2f - marginHeight);
		return new Vector2(width / 2f, height / 2f);
	}
}
